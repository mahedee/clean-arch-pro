using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a payment is successfully processed
    /// </summary>
    public class PaymentProcessedEvent : DomainEvent
    {
        public Guid PaymentId { get; }
        public Guid StudentId { get; }
        public Guid FeeId { get; }
        public decimal Amount { get; }
        public string TransactionReference { get; }

        public PaymentProcessedEvent(Guid paymentId, Guid studentId, Guid feeId, decimal amount, string transactionReference)
        {
            PaymentId = paymentId;
            StudentId = studentId;
            FeeId = feeId;
            Amount = amount;
            TransactionReference = transactionReference;
        }
    }
}
