namespace EduTrack.Domain.ValueObjects
{
    /// <summary>
    /// GPA (Grade Point Average) value object that encapsulates GPA validation and business logic
    /// </summary>
    public sealed class GPA : IEquatable<GPA>, IComparable<GPA>
    {
        /// <summary>
        /// The GPA value (0.0 to 4.0 scale)
        /// </summary>
        public decimal Value { get; }

        /// <summary>
        /// Minimum allowed GPA value
        /// </summary>
        public static decimal MinValue => 0.0m;

        /// <summary>
        /// Maximum allowed GPA value
        /// </summary>
        public static decimal MaxValue => 4.0m;

        /// <summary>
        /// Private constructor to enforce factory method usage
        /// </summary>
        private GPA(decimal value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a new GPA instance with validation
        /// </summary>
        /// <param name="value">The GPA value to validate (0.0 to 4.0)</param>
        /// <returns>A valid GPA instance</returns>
        /// <exception cref="ArgumentException">Thrown when GPA value is out of range</exception>
        public static GPA Create(decimal value)
        {
            if (value < MinValue || value > MaxValue)
                throw new ArgumentException($"GPA must be between {MinValue} and {MaxValue}", nameof(value));

            // Round to 2 decimal places for consistency
            var roundedValue = Math.Round(value, 2);

            return new GPA(roundedValue);
        }

        /// <summary>
        /// Creates a GPA from a percentage (0-100) by converting to 4.0 scale
        /// </summary>
        public static GPA FromPercentage(decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Percentage must be between 0 and 100", nameof(percentage));

            // Convert percentage to 4.0 scale (assuming 60% = 0.0, 100% = 4.0)
            var gpaValue = percentage >= 60 ? (percentage - 60) / 10 : 0;
            return Create(gpaValue);
        }

        /// <summary>
        /// Checks if the GPA qualifies for honors (typically 3.5 or higher)
        /// </summary>
        public bool IsHonorsLevel => Value >= 3.5m;

        /// <summary>
        /// Checks if the GPA is a passing grade (typically 2.0 or higher)
        /// </summary>
        public bool IsPassingGrade => Value >= 2.0m;

        /// <summary>
        /// Checks if the GPA qualifies for Dean's List (typically 3.7 or higher)
        /// </summary>
        public bool IsDeansListEligible => Value >= 3.7m;

        /// <summary>
        /// Checks if the GPA is on academic probation (typically below 2.0)
        /// </summary>
        public bool IsOnProbation => Value < 2.0m;

        /// <summary>
        /// Gets the letter grade equivalent
        /// </summary>
        public string LetterGrade => Value switch
        {
            >= 3.7m => "A",
            >= 3.3m => "A-",
            >= 3.0m => "B+",
            >= 2.7m => "B",
            >= 2.3m => "B-",
            >= 2.0m => "C+",
            >= 1.7m => "C",
            >= 1.3m => "C-",
            >= 1.0m => "D",
            _ => "F"
        };

        /// <summary>
        /// Gets the academic standing description
        /// </summary>
        public string AcademicStanding => Value switch
        {
            >= 3.7m => "Summa Cum Laude",
            >= 3.5m => "Magna Cum Laude",
            >= 3.3m => "Cum Laude",
            >= 3.0m => "Good Standing",
            >= 2.0m => "Satisfactory",
            _ => "Academic Probation"
        };

        /// <summary>
        /// Gets the GPA quality description
        /// </summary>
        public string QualityDescription => Value switch
        {
            >= 3.8m => "Excellent",
            >= 3.5m => "Very Good",
            >= 3.0m => "Good",
            >= 2.5m => "Satisfactory",
            >= 2.0m => "Acceptable",
            _ => "Below Standards"
        };

        /// <summary>
        /// Calculates weighted GPA with additional credit hours
        /// </summary>
        public GPA CalculateWeightedGPA(GPA otherGPA, int currentCreditHours, int otherCreditHours)
        {
            if (currentCreditHours <= 0 || otherCreditHours <= 0)
                throw new ArgumentException("Credit hours must be positive");

            var totalCreditHours = currentCreditHours + otherCreditHours;
            var weightedValue = (Value * currentCreditHours + otherGPA.Value * otherCreditHours) / totalCreditHours;
            
            return Create(weightedValue);
        }

        /// <summary>
        /// Applies a bonus to the GPA (e.g., for honors courses)
        /// </summary>
        public GPA ApplyBonus(decimal bonus)
        {
            return Create(Math.Min(MaxValue, Value + bonus));
        }

        /// <summary>
        /// Checks if this GPA meets a minimum requirement
        /// </summary>
        public bool MeetsRequirement(GPA minimumGPA) => Value >= minimumGPA.Value;

        /// <summary>
        /// Gets the percentage equivalent (rough conversion)
        /// </summary>
        public decimal ToPercentage() => Math.Round(60 + (Value * 10), 1);

        /// <summary>
        /// Value equality comparison
        /// </summary>
        public bool Equals(GPA? other)
        {
            return other is not null && Value == other.Value;
        }

        /// <summary>
        /// Object equality comparison
        /// </summary>
        public override bool Equals(object? obj) => Equals(obj as GPA);

        /// <summary>
        /// Gets hash code for the GPA value
        /// </summary>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// String representation of the GPA
        /// </summary>
        public override string ToString() => $"{Value:F2}";

        /// <summary>
        /// Formatted string with letter grade
        /// </summary>
        public string ToStringWithGrade() => $"{Value:F2} ({LetterGrade})";

        /// <summary>
        /// Comparison for sorting
        /// </summary>
        public int CompareTo(GPA? other)
        {
            if (other is null) return 1;
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(GPA? left, GPA? right) => Equals(left, right);

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(GPA? left, GPA? right) => !Equals(left, right);

        /// <summary>
        /// Greater than operator
        /// </summary>
        public static bool operator >(GPA left, GPA right) => left.Value > right.Value;

        /// <summary>
        /// Less than operator
        /// </summary>
        public static bool operator <(GPA left, GPA right) => left.Value < right.Value;

        /// <summary>
        /// Greater than or equal operator
        /// </summary>
        public static bool operator >=(GPA left, GPA right) => left.Value >= right.Value;

        /// <summary>
        /// Less than or equal operator
        /// </summary>
        public static bool operator <=(GPA left, GPA right) => left.Value <= right.Value;

        /// <summary>
        /// Implicit conversion to decimal for convenience
        /// </summary>
        public static implicit operator decimal(GPA gpa) => gpa.Value;
    }
}
