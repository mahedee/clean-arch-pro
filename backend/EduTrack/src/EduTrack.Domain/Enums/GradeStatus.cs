namespace EduTrack.Domain.Enums;

/// <summary>
/// Grade status enumeration representing the lifecycle of a grade record
/// </summary>
public enum GradeStatus
{
    Pending = 1,
    Submitted = 2,
    Approved = 3,
    Published = 4,
    Disputed = 5,
    Revised = 6
}
