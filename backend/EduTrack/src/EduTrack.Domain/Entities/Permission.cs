using EduTrack.Domain.Common;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Permission aggregate root representing a fine-grained access control entry
    /// defining what action can be performed on a given resource
    /// </summary>
    public class Permission : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private string _name = null!;
        private string _resource = null!;
        private string _action = null!;

        /// <summary>
        /// Unique permission name following the convention "Resource.Action"
        /// (e.g., "Students.Read", "Grades.Write")
        /// </summary>
        public string Name
        {
            get => _name;
            private set => _name = ValidateName(value);
        }

        /// <summary>
        /// The resource this permission controls access to (e.g., "Students", "Courses")
        /// </summary>
        public string Resource
        {
            get => _resource;
            private set => _resource = ValidateField(value, nameof(Resource));
        }

        /// <summary>
        /// The action allowed on the resource (e.g., "Read", "Write", "Delete", "Approve")
        /// </summary>
        public string Action
        {
            get => _action;
            private set => _action = ValidateField(value, nameof(Action));
        }

        /// <summary>
        /// Human-readable description of what this permission allows
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Whether this is a system-defined permission that cannot be deleted
        /// </summary>
        public bool IsSystemPermission { get; private set; }

        // Private constructor for EF Core
        private Permission() : base() { }

        /// <summary>
        /// Create a new permission
        /// </summary>
        /// <param name="resource">Resource the permission applies to</param>
        /// <param name="action">Action permitted on the resource</param>
        /// <param name="description">Human-readable description</param>
        /// <param name="isSystemPermission">Whether this is a system-defined permission</param>
        /// <returns>New permission instance</returns>
        public static Permission Create(string resource, string action, string description, bool isSystemPermission = false)
        {
            var name = $"{resource.Trim()}.{action.Trim()}";

            return new Permission
            {
                Id = Guid.NewGuid(),
                Resource = resource,
                Action = action,
                Name = name,
                Description = description?.Trim() ?? string.Empty,
                IsSystemPermission = isSystemPermission
            };
        }

        /// <summary>
        /// Update the permission description
        /// </summary>
        /// <param name="description">New description</param>
        public void UpdateDescription(string description)
        {
            if (IsSystemPermission)
                throw new InvalidOperationException("System permissions cannot be modified.");

            Description = description?.Trim() ?? string.Empty;
            MarkAsUpdated();
        }

        // Private validation helpers
        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Permission name cannot be empty.");
            if (name.Length > 150)
                throw new ArgumentException("Permission name cannot exceed 150 characters.");
            return name.Trim();
        }

        private static string ValidateField(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} cannot be empty.");
            if (value.Length > 100)
                throw new ArgumentException($"{fieldName} cannot exceed 100 characters.");
            return value.Trim();
        }
    }
}
