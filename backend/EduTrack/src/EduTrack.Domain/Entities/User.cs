using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;
using EduTrack.Domain.Events;
using EduTrack.Domain.ValueObjects;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// User aggregate root representing a system user account for authentication and authorization
    /// </summary>
    public class User : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private string _username = null!;
        private Email _email = null!;
        private string _passwordHash = null!;
        private readonly List<Guid> _roleIds = new();

        /// <summary>
        /// Unique username for login
        /// </summary>
        public string Username
        {
            get => _username;
            private set => _username = ValidateUsername(value);
        }

        /// <summary>
        /// User's email address (used for login and notifications)
        /// </summary>
        public Email Email
        {
            get => _email;
            private set => _email = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Bcrypt or Argon2 hashed password – never store plain text
        /// </summary>
        public string PasswordHash
        {
            get => _passwordHash;
            private set => _passwordHash = !string.IsNullOrWhiteSpace(value)
                ? value
                : throw new ArgumentException("Password hash cannot be empty.");
        }

        /// <summary>
        /// Optional display name for the user
        /// </summary>
        public FullName? FullName { get; private set; }

        /// <summary>
        /// Current account status
        /// </summary>
        public UserStatus Status { get; private set; }

        /// <summary>
        /// Whether the email has been verified
        /// </summary>
        public bool IsEmailVerified { get; private set; }

        /// <summary>
        /// Date and time of the last successful login
        /// </summary>
        public DateTime? LastLoginAt { get; private set; }

        /// <summary>
        /// Hashed refresh token for JWT renewal
        /// </summary>
        public string? RefreshTokenHash { get; private set; }

        /// <summary>
        /// Expiry date of the current refresh token
        /// </summary>
        public DateTime? RefreshTokenExpiry { get; private set; }

        /// <summary>
        /// Number of consecutive failed login attempts
        /// </summary>
        public int FailedLoginAttempts { get; private set; }

        /// <summary>
        /// Date and time the account was locked (if locked)
        /// </summary>
        public DateTime? LockedUntil { get; private set; }

        /// <summary>
        /// IDs of roles assigned to this user
        /// </summary>
        public IReadOnlyCollection<Guid> RoleIds => _roleIds.AsReadOnly();

        // Private constructor for EF Core
        private User() : base() { }

        /// <summary>
        /// Create a new user account
        /// </summary>
        /// <param name="username">Unique username</param>
        /// <param name="email">User's email address</param>
        /// <param name="passwordHash">Hashed password</param>
        /// <param name="fullName">Optional display name</param>
        /// <returns>New user instance in PendingVerification status</returns>
        public static User Create(string username, Email email, string passwordHash, FullName? fullName = null)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                FullName = fullName,
                Status = UserStatus.PendingVerification,
                IsEmailVerified = false,
                FailedLoginAttempts = 0
            };

            user.AddDomainEvent(new UserCreatedEvent(user.Id, username, email.Value));
            return user;
        }

        /// <summary>
        /// Mark the user's email as verified and activate the account
        /// </summary>
        public void VerifyEmail()
        {
            if (IsEmailVerified)
                throw new InvalidOperationException("Email is already verified.");

            IsEmailVerified = true;
            Status = UserStatus.Active;
            MarkAsUpdated();
        }

        /// <summary>
        /// Record a successful login
        /// </summary>
        public void RecordLogin()
        {
            if (Status != UserStatus.Active)
                throw new InvalidOperationException("Only active accounts can log in.");

            LastLoginAt = DateTime.UtcNow;
            FailedLoginAttempts = 0;
            MarkAsUpdated();
        }

        /// <summary>
        /// Record a failed login attempt; lock account after 5 consecutive failures
        /// </summary>
        public void RecordFailedLogin()
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= 5)
            {
                Status = UserStatus.Locked;
                LockedUntil = DateTime.UtcNow.AddMinutes(30);
            }

            MarkAsUpdated();
        }

        /// <summary>
        /// Unlock the account (admin action or lock duration expired)
        /// </summary>
        public void Unlock()
        {
            if (Status != UserStatus.Locked)
                throw new InvalidOperationException("Account is not locked.");

            Status = UserStatus.Active;
            FailedLoginAttempts = 0;
            LockedUntil = null;
            MarkAsUpdated();
        }

        /// <summary>
        /// Suspend the user account
        /// </summary>
        /// <param name="suspendedBy">Admin performing the action</param>
        public void Suspend(string suspendedBy)
        {
            if (Status == UserStatus.Suspended)
                throw new InvalidOperationException("Account is already suspended.");

            Status = UserStatus.Suspended;
            MarkAsUpdated(suspendedBy);
        }

        /// <summary>
        /// Deactivate the user account
        /// </summary>
        public void Deactivate()
        {
            if (Status == UserStatus.Inactive)
                throw new InvalidOperationException("Account is already inactive.");

            Status = UserStatus.Inactive;
            MarkAsUpdated();
        }

        /// <summary>
        /// Reactivate a suspended or inactive account
        /// </summary>
        /// <param name="reactivatedBy">Admin performing the action</param>
        public void Reactivate(string reactivatedBy)
        {
            if (Status == UserStatus.Active)
                throw new InvalidOperationException("Account is already active.");
            if (!IsEmailVerified)
                throw new InvalidOperationException("Email must be verified before reactivating.");

            Status = UserStatus.Active;
            MarkAsUpdated(reactivatedBy);
        }

        /// <summary>
        /// Update the hashed password
        /// </summary>
        /// <param name="newPasswordHash">New bcrypt/argon2 password hash</param>
        public void UpdatePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            RefreshTokenHash = null;
            RefreshTokenExpiry = null;
            MarkAsUpdated();
        }

        /// <summary>
        /// Store a new refresh token hash for JWT renewal
        /// </summary>
        /// <param name="refreshTokenHash">Hashed refresh token</param>
        /// <param name="expiry">Token expiry date/time</param>
        public void SetRefreshToken(string refreshTokenHash, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenHash))
                throw new ArgumentException("Refresh token hash cannot be empty.", nameof(refreshTokenHash));
            if (expiry <= DateTime.UtcNow)
                throw new ArgumentException("Refresh token expiry must be in the future.", nameof(expiry));

            RefreshTokenHash = refreshTokenHash;
            RefreshTokenExpiry = expiry;
            MarkAsUpdated();
        }

        /// <summary>
        /// Revoke the current refresh token (e.g., on logout)
        /// </summary>
        public void RevokeRefreshToken()
        {
            RefreshTokenHash = null;
            RefreshTokenExpiry = null;
            MarkAsUpdated();
        }

        /// <summary>
        /// Assign a role to this user
        /// </summary>
        /// <param name="roleId">ID of the role to assign</param>
        public void AssignRole(Guid roleId)
        {
            if (roleId == Guid.Empty)
                throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));
            if (_roleIds.Contains(roleId))
                throw new InvalidOperationException("Role is already assigned to this user.");

            _roleIds.Add(roleId);
            MarkAsUpdated();
        }

        /// <summary>
        /// Remove a role from this user
        /// </summary>
        /// <param name="roleId">ID of the role to remove</param>
        public void RemoveRole(Guid roleId)
        {
            if (!_roleIds.Contains(roleId))
                throw new InvalidOperationException("Role is not assigned to this user.");

            _roleIds.Remove(roleId);
            MarkAsUpdated();
        }

        // Private validation helpers
        private static string ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.");
            if (username.Length < 3 || username.Length > 50)
                throw new ArgumentException("Username must be between 3 and 50 characters.");
            return username.Trim().ToLowerInvariant();
        }
    }
}
