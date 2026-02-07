namespace EduTrack.Domain.ValueObjects;

/// <summary>
/// Represents a unique student identifier value object
/// </summary>
public sealed class StudentId : IEquatable<StudentId>
{
    /// <summary>
    /// Gets the unique identifier value
    /// </summary>
    public Guid Value { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework
    /// </summary>
    private StudentId() { }

    /// <summary>
    /// Private constructor for creating new instances
    /// </summary>
    /// <param name="value">The student ID value</param>
    private StudentId(Guid value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new StudentId with a unique identifier
    /// </summary>
    /// <returns>A new StudentId instance</returns>
    public static StudentId Create()
    {
        return new StudentId(Guid.NewGuid());
    }

    /// <summary>
    /// Creates a StudentId from an existing Guid
    /// </summary>
    /// <param name="value">The existing Guid value</param>
    /// <returns>A StudentId instance</returns>
    /// <exception cref="ArgumentException">Thrown when the value is empty</exception>
    public static StudentId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(value));

        return new StudentId(value);
    }

    /// <summary>
    /// Creates a StudentId from a string representation
    /// </summary>
    /// <param name="value">The string representation of the Guid</param>
    /// <returns>A StudentId instance</returns>
    /// <exception cref="ArgumentException">Thrown when the value is null, empty, or invalid</exception>
    public static StudentId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Student ID cannot be null or empty", nameof(value));

        if (!Guid.TryParse(value, out Guid parsedValue))
            throw new ArgumentException("Invalid Student ID format", nameof(value));

        return Create(parsedValue);
    }

    /// <summary>
    /// Implicit conversion to Guid
    /// </summary>
    /// <param name="studentId">The StudentId instance</param>
    public static implicit operator Guid(StudentId studentId) => studentId.Value;

    /// <summary>
    /// Implicit conversion to string
    /// </summary>
    /// <param name="studentId">The StudentId instance</param>
    public static implicit operator string(StudentId studentId) => studentId.Value.ToString();

    /// <summary>
    /// Explicit conversion from Guid
    /// </summary>
    /// <param name="value">The Guid value</param>
    public static explicit operator StudentId(Guid value) => Create(value);

    /// <summary>
    /// Explicit conversion from string
    /// </summary>
    /// <param name="value">The string value</param>
    public static explicit operator StudentId(string value) => Create(value);

    /// <summary>
    /// String representation of the StudentId
    /// </summary>
    /// <returns>The string representation of the Guid</returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Value equality comparison
    /// </summary>
    /// <param name="other">The other StudentId instance to compare</param>
    /// <returns>True if the values are equal</returns>
    public bool Equals(StudentId? other)
    {
        return other is not null && Value.Equals(other.Value);
    }

    /// <summary>
    /// Object equality comparison
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if the objects are equal</returns>
    public override bool Equals(object? obj) => Equals(obj as StudentId);

    /// <summary>
    /// Gets the hash code for the StudentId
    /// </summary>
    /// <returns>The hash code</returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns>True if equal</returns>
    public static bool operator ==(StudentId? left, StudentId? right) => Equals(left, right);

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns>True if not equal</returns>
    public static bool operator !=(StudentId? left, StudentId? right) => !Equals(left, right);
}