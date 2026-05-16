using System.Text.RegularExpressions;

namespace EduTrack.Domain.ValueObjects
{
    /// <summary>
    /// Email value object that encapsulates email validation and business logic
    /// </summary>
    public sealed class Email : IEquatable<Email>
    {
        private static readonly Regex EmailPattern = new(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The email value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the domain part of the email (after @)
        /// </summary>
        public string Domain => Value.Split('@')[1];

        /// <summary>
        /// Gets the local part of the email (before @)
        /// </summary>
        public string LocalPart => Value.Split('@')[0];

        /// <summary>
        /// Private constructor to enforce factory method usage
        /// </summary>
        private Email(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a new Email instance with validation
        /// </summary>
        /// <param name="email">The email string to validate</param>
        /// <returns>A valid Email instance</returns>
        /// <exception cref="ArgumentException">Thrown when email format is invalid</exception>
        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty or whitespace", nameof(email));

            // Normalize the email
            email = email.Trim().ToLowerInvariant();

            if (!IsValidEmailFormat(email))
                throw new ArgumentException($"Invalid email format: {email}", nameof(email));

            if (email.Length > 254) // RFC 5321 limit
                throw new ArgumentException("Email address is too long (maximum 254 characters)", nameof(email));

            return new Email(email);
        }

        /// <summary>
        /// Validates the email format using regex
        /// </summary>
        private static bool IsValidEmailFormat(string email)
        {
            return EmailPattern.IsMatch(email);
        }

        /// <summary>
        /// Checks if this is a university email (ends with .edu)
        /// </summary>
        public bool IsUniversityEmail() => Domain.EndsWith(".edu", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Checks if this is a corporate email (not common free email providers)
        /// </summary>
        public bool IsCorporateEmail()
        {
            var freeProviders = new[] { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "live.com" };
            return !freeProviders.Contains(Domain, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a new Email with a different domain
        /// </summary>
        public Email WithDomain(string newDomain)
        {
            if (string.IsNullOrWhiteSpace(newDomain))
                throw new ArgumentException("Domain cannot be empty", nameof(newDomain));

            return Create($"{LocalPart}@{newDomain}");
        }

        /// <summary>
        /// Value equality comparison
        /// </summary>
        public bool Equals(Email? other)
        {
            return other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Object equality comparison
        /// </summary>
        public override bool Equals(object? obj) => Equals(obj as Email);

        /// <summary>
        /// Gets hash code for the email value
        /// </summary>
        public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

        /// <summary>
        /// String representation of the email
        /// </summary>
        public override string ToString() => Value;

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(Email? left, Email? right) => Equals(left, right);

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(Email? left, Email? right) => !Equals(left, right);

        /// <summary>
        /// Implicit conversion to string for convenience
        /// </summary>
        public static implicit operator string(Email email) => email.Value;
    }
}
