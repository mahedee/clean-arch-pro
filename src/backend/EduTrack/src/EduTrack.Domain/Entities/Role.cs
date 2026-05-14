using EduTrack.Domain.Common;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Role aggregate root representing a named set of permissions assigned to users
    /// </summary>
    public class Role : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private string _name = null!;
        private readonly List<Guid> _permissionIds = new();

        /// <summary>
        /// Unique role name (e.g., "Admin", "Student", "Teacher")
        /// </summary>
        public string Name
        {
            get => _name;
            private set => _name = ValidateName(value);
        }

        /// <summary>
        /// Human-readable description of the role's purpose
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Whether this is a system-defined role that cannot be deleted
        /// </summary>
        public bool IsSystemRole { get; private set; }

        /// <summary>
        /// IDs of permissions included in this role
        /// </summary>
        public IReadOnlyCollection<Guid> PermissionIds => _permissionIds.AsReadOnly();

        // Private constructor for EF Core
        private Role() : base() { }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="name">Unique role name</param>
        /// <param name="description">Role description</param>
        /// <param name="isSystemRole">Whether this role is system-defined</param>
        /// <returns>New role instance</returns>
        public static Role Create(string name, string description, bool isSystemRole = false)
        {
            return new Role
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description?.Trim() ?? string.Empty,
                IsSystemRole = isSystemRole
            };
        }

        /// <summary>
        /// Update the role's description
        /// </summary>
        /// <param name="description">New description</param>
        public void UpdateDescription(string description)
        {
            if (IsSystemRole)
                throw new InvalidOperationException("System roles cannot be modified.");

            Description = description?.Trim() ?? string.Empty;
            MarkAsUpdated();
        }

        /// <summary>
        /// Rename the role
        /// </summary>
        /// <param name="newName">New role name</param>
        public void Rename(string newName)
        {
            if (IsSystemRole)
                throw new InvalidOperationException("System roles cannot be renamed.");

            Name = newName;
            MarkAsUpdated();
        }

        /// <summary>
        /// Grant a permission to this role
        /// </summary>
        /// <param name="permissionId">ID of the permission to grant</param>
        public void GrantPermission(Guid permissionId)
        {
            if (permissionId == Guid.Empty)
                throw new ArgumentException("Permission ID cannot be empty.", nameof(permissionId));
            if (_permissionIds.Contains(permissionId))
                throw new InvalidOperationException("Permission is already granted to this role.");

            _permissionIds.Add(permissionId);
            MarkAsUpdated();
        }

        /// <summary>
        /// Revoke a permission from this role
        /// </summary>
        /// <param name="permissionId">ID of the permission to revoke</param>
        public void RevokePermission(Guid permissionId)
        {
            if (!_permissionIds.Contains(permissionId))
                throw new InvalidOperationException("Permission is not granted to this role.");

            _permissionIds.Remove(permissionId);
            MarkAsUpdated();
        }

        // Private validation helpers
        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty.");
            if (name.Length > 100)
                throw new ArgumentException("Role name cannot exceed 100 characters.");
            return name.Trim();
        }
    }
}
