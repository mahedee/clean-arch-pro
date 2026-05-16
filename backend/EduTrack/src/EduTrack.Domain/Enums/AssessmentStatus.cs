namespace EduTrack.Domain.Enums;

/// <summary>
/// Assessment status enumeration representing the lifecycle state of an assessment
/// </summary>
public enum AssessmentStatus
{
    Draft = 1,
    Published = 2,
    Active = 3,
    Closed = 4,
    Graded = 5
}
