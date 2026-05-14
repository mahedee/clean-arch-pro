namespace EduTrack.Domain.Enums;

/// <summary>
/// Admission status enumeration representing the state of a student admission application
/// </summary>
public enum AdmissionStatus
{
    Submitted = 1,
    UnderReview = 2,
    DocumentsRequired = 3,
    WaitListed = 4,
    Accepted = 5,
    Rejected = 6,
    Withdrawn = 7
}
