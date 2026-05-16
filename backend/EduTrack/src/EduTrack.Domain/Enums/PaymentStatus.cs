namespace EduTrack.Domain.Enums;

/// <summary>
/// Payment status enumeration representing the state of a fee payment
/// </summary>
public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    PartiallyPaid = 5,
    Refunded = 6,
    Cancelled = 7
}
