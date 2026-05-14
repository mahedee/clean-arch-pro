using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;
using EduTrack.Domain.Events;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Payment aggregate root representing a student's payment transaction for a fee
    /// </summary>
    public class Payment : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private decimal _amount;

        /// <summary>
        /// ID of the student making the payment
        /// </summary>
        public Guid StudentId { get; private set; }

        /// <summary>
        /// ID of the fee being paid
        /// </summary>
        public Guid FeeId { get; private set; }

        /// <summary>
        /// Amount paid in this transaction
        /// </summary>
        public decimal Amount
        {
            get => _amount;
            private set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Amount), "Payment amount must be greater than zero.");
                _amount = value;
            }
        }

        /// <summary>
        /// Current payment status
        /// </summary>
        public PaymentStatus Status { get; private set; }

        /// <summary>
        /// Payment method used for this transaction
        /// </summary>
        public PaymentMethod Method { get; private set; }

        /// <summary>
        /// Unique transaction reference from the payment gateway or bank
        /// </summary>
        public string? TransactionReference { get; private set; }

        /// <summary>
        /// Date and time the payment was initiated
        /// </summary>
        public DateTime PaymentDate { get; private set; }

        /// <summary>
        /// Date and time the payment was confirmed/completed
        /// </summary>
        public DateTime? CompletedAt { get; private set; }

        /// <summary>
        /// Date and time the payment was refunded (if applicable)
        /// </summary>
        public DateTime? RefundedAt { get; private set; }

        /// <summary>
        /// Optional notes about the payment (e.g., receipt number, memo)
        /// </summary>
        public string? Notes { get; private set; }

        /// <summary>
        /// Installment number (1-based) if this is a partial payment
        /// </summary>
        public int? InstallmentNumber { get; private set; }

        // Private constructor for EF Core
        private Payment() : base() { }

        /// <summary>
        /// Initiate a new payment transaction
        /// </summary>
        /// <param name="studentId">ID of the student</param>
        /// <param name="feeId">ID of the fee being paid</param>
        /// <param name="amount">Amount being paid</param>
        /// <param name="method">Payment method</param>
        /// <param name="notes">Optional notes</param>
        /// <param name="installmentNumber">Optional installment number for partial payments</param>
        /// <returns>New payment instance in Pending status</returns>
        public static Payment Initiate(
            Guid studentId,
            Guid feeId,
            decimal amount,
            PaymentMethod method,
            string? notes = null,
            int? installmentNumber = null)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));
            if (feeId == Guid.Empty)
                throw new ArgumentException("Fee ID cannot be empty.", nameof(feeId));
            if (installmentNumber.HasValue && installmentNumber.Value < 1)
                throw new ArgumentOutOfRangeException(nameof(installmentNumber), "Installment number must be at least 1.");

            return new Payment
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                FeeId = feeId,
                Amount = amount,
                Method = method,
                Status = PaymentStatus.Pending,
                PaymentDate = DateTime.UtcNow,
                Notes = notes?.Trim(),
                InstallmentNumber = installmentNumber
            };
        }

        /// <summary>
        /// Mark the payment as being processed by the payment gateway
        /// </summary>
        public void MarkAsProcessing()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be marked as processing.");

            Status = PaymentStatus.Processing;
            MarkAsUpdated();
        }

        /// <summary>
        /// Confirm the payment as successfully completed
        /// </summary>
        /// <param name="transactionReference">Reference number from payment gateway or bank</param>
        public void Complete(string? transactionReference = null)
        {
            if (Status != PaymentStatus.Processing && Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending or processing payments can be completed.");

            Status = PaymentStatus.Completed;
            TransactionReference = transactionReference;
            CompletedAt = DateTime.UtcNow;
            MarkAsUpdated();

            AddDomainEvent(new PaymentProcessedEvent(
                Id, StudentId, FeeId, Amount, transactionReference ?? Id.ToString()));
        }

        /// <summary>
        /// Mark the payment as failed
        /// </summary>
        /// <param name="notes">Reason for failure</param>
        public void Fail(string? notes = null)
        {
            if (Status != PaymentStatus.Processing && Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending or processing payments can be marked as failed.");

            Status = PaymentStatus.Failed;
            Notes = notes;
            MarkAsUpdated();
        }

        /// <summary>
        /// Refund the payment
        /// </summary>
        /// <param name="notes">Optional reason for refund</param>
        public void Refund(string? notes = null)
        {
            if (Status != PaymentStatus.Completed)
                throw new InvalidOperationException("Only completed payments can be refunded.");

            Status = PaymentStatus.Refunded;
            RefundedAt = DateTime.UtcNow;
            Notes = notes;
            MarkAsUpdated();
        }

        /// <summary>
        /// Cancel a pending payment before processing
        /// </summary>
        /// <param name="notes">Optional reason for cancellation</param>
        public void Cancel(string? notes = null)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be cancelled.");

            Status = PaymentStatus.Cancelled;
            Notes = notes;
            MarkAsUpdated();
        }
    }
}
