namespace EduTrack.Domain.Enums;

/// <summary>
/// Department status enumeration
/// </summary>
public enum DepartmentStatus
{
    /// <summary>
    /// Department is active and operational
    /// </summary>
    Active = 1,

    /// <summary>
    /// Department is inactive or suspended
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Department is being merged with another department  
    /// </summary>
    Merging = 3,

    /// <summary>
    /// Department has been closed permanently
    /// </summary>
    Closed = 4
}
