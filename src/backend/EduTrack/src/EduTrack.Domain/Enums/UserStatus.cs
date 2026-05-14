namespace EduTrack.Domain.Enums;

/// <summary>
/// User status enumeration representing the state of a system user account
/// </summary>
public enum UserStatus
{
    PendingVerification = 1,
    Active = 2,
    Inactive = 3,
    Suspended = 4,
    Locked = 5
}
