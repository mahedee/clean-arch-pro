using System.Text.RegularExpressions;

namespace EduTrack.Domain.ValueObjects
{
    /// <summary>
    /// FullName value object that encapsulates name validation and business logic
    /// </summary>
    public sealed class FullName : IEquatable<FullName>
    {
        private static readonly Regex NamePattern = new(
            @"^[a-zA-Z\s\-\.\']+$",
            RegexOptions.Compiled);

        /// <summary>
        /// The complete full name
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the first name (first word)
        /// </summary>
        public string FirstName => GetNameParts()[0];

        /// <summary>
        /// Gets the last name (last word)
        /// </summary>
        public string LastName => GetNameParts().Last();

        /// <summary>
        /// Gets the middle name(s) if any
        /// </summary>
        public string? MiddleName
        {
            get
            {
                var parts = GetNameParts();
                if (parts.Length <= 2) return null;
                
                return string.Join(" ", parts.Skip(1).Take(parts.Length - 2));
            }
        }

        /// <summary>
        /// Gets initials (first letter of each name part)
        /// </summary>
        public string Initials
        {
            get
            {
                return string.Join(".", GetNameParts().Select(part => part[0].ToString().ToUpperInvariant())) + ".";
            }
        }

        /// <summary>
        /// Private constructor to enforce factory method usage
        /// </summary>
        private FullName(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a new FullName instance with validation
        /// </summary>
        /// <param name="fullName">The full name string to validate</param>
        /// <returns>A valid FullName instance</returns>
        /// <exception cref="ArgumentException">Thrown when name format is invalid</exception>
        public static FullName Create(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty or whitespace", nameof(fullName));

            // Normalize the name
            fullName = NormalizeName(fullName);

            if (!IsValidNameFormat(fullName))
                throw new ArgumentException("Invalid name format. Only letters, spaces, hyphens, periods, and apostrophes are allowed", nameof(fullName));

            if (fullName.Length < 2)
                throw new ArgumentException("Name must be at least 2 characters long", nameof(fullName));

            if (fullName.Length > 100)
                throw new ArgumentException("Name cannot exceed 100 characters", nameof(fullName));

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                throw new ArgumentException("Full name must contain at least first and last name", nameof(fullName));

            return new FullName(fullName);
        }

        /// <summary>
        /// Factory method to create FullName from separate parts
        /// </summary>
        public static FullName Create(string firstName, string lastName, string? middleName = null)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));

            var fullName = string.IsNullOrWhiteSpace(middleName) 
                ? $"{firstName} {lastName}"
                : $"{firstName} {middleName} {lastName}";

            return Create(fullName);
        }

        /// <summary>
        /// Normalizes the name by trimming, removing extra spaces, and proper casing
        /// </summary>
        private static string NormalizeName(string name)
        {
            // Remove extra whitespace and normalize
            name = Regex.Replace(name.Trim(), @"\s+", " ");
            
            // Convert to proper case (Title Case)
            var words = name.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpperInvariant(words[i][0]) + 
                              (words[i].Length > 1 ? words[i][1..].ToLowerInvariant() : "");
                }
            }
            
            return string.Join(" ", words);
        }

        /// <summary>
        /// Validates the name format using regex
        /// </summary>
        private static bool IsValidNameFormat(string name)
        {
            return NamePattern.IsMatch(name);
        }

        /// <summary>
        /// Gets the name parts as an array
        /// </summary>
        private string[] GetNameParts()
        {
            return Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets the display name in "Last, First" format
        /// </summary>
        public string GetDisplayName() => $"{LastName}, {FirstName}";

        /// <summary>
        /// Gets the formal name with title
        /// </summary>
        public string GetFormalName(string? title = null)
        {
            return string.IsNullOrWhiteSpace(title) ? Value : $"{title} {Value}";
        }

        /// <summary>
        /// Checks if the name contains a specific part
        /// </summary>
        public bool Contains(string namePart)
        {
            return Value.Contains(namePart, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Value equality comparison
        /// </summary>
        public bool Equals(FullName? other)
        {
            return other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Object equality comparison
        /// </summary>
        public override bool Equals(object? obj) => Equals(obj as FullName);

        /// <summary>
        /// Gets hash code for the name value
        /// </summary>
        public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

        /// <summary>
        /// String representation of the name
        /// </summary>
        public override string ToString() => Value;

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(FullName? left, FullName? right) => Equals(left, right);

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(FullName? left, FullName? right) => !Equals(left, right);

        /// <summary>
        /// Implicit conversion to string for convenience
        /// </summary>
        public static implicit operator string(FullName fullName) => fullName.Value;
    }
}
