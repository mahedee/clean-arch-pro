using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Fee aggregate root representing an academic fee structure
    /// (e.g., tuition, registration, lab fees)
    /// </summary>
    public class Fee : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private string _name = null!;
        private decimal _amount;

        /// <summary>
        /// Name of the fee (e.g., "Tuition Fee - Spring 2026")
        /// </summary>
        public string Name
        {
            get => _name;
            private set => _name = ValidateName(value);
        }

        /// <summary>
        /// Optional description of what the fee covers
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Fee amount in the institution's base currency
        /// </summary>
        public decimal Amount
        {
            get => _amount;
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Amount), "Fee amount cannot be negative.");
                _amount = value;
            }
        }

        /// <summary>
        /// Category of the fee
        /// </summary>
        public FeeType Type { get; private set; }

        /// <summary>
        /// Academic year this fee applies to
        /// </summary>
        public int AcademicYear { get; private set; }

        /// <summary>
        /// Optional academic semester this fee applies to (null = full year)
        /// </summary>
        public string? Semester { get; private set; }

        /// <summary>
        /// Whether this fee is currently active and can be assigned to students
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Optional due date by which the fee must be paid
        /// </summary>
        public DateTime? DueDate { get; private set; }

        /// <summary>
        /// Whether the fee can be paid in installments
        /// </summary>
        public bool AllowsInstallments { get; private set; }

        /// <summary>
        /// Maximum number of installments allowed (if applicable)
        /// </summary>
        public int? MaxInstallments { get; private set; }

        // Private constructor for EF Core
        private Fee() : base() { }

        /// <summary>
        /// Create a new fee structure
        /// </summary>
        /// <param name="name">Fee name</param>
        /// <param name="type">Fee category</param>
        /// <param name="amount">Fee amount</param>
        /// <param name="academicYear">Applicable academic year</param>
        /// <param name="semester">Optional applicable semester</param>
        /// <param name="description">Optional description</param>
        /// <param name="dueDate">Optional payment due date</param>
        /// <returns>New fee instance</returns>
        public static Fee Create(
            string name,
            FeeType type,
            decimal amount,
            int academicYear,
            string? semester = null,
            string? description = null,
            DateTime? dueDate = null)
        {
            if (academicYear < 2000 || academicYear > DateTime.UtcNow.Year + 2)
                throw new ArgumentOutOfRangeException(nameof(academicYear), "Academic year is not valid.");

            return new Fee
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type,
                Amount = amount,
                AcademicYear = academicYear,
                Semester = semester?.Trim(),
                Description = description?.Trim(),
                DueDate = dueDate,
                IsActive = true,
                AllowsInstallments = false
            };
        }

        /// <summary>
        /// Deactivate the fee so it can no longer be assigned
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("Fee is already inactive.");

            IsActive = false;
            MarkAsUpdated();
        }

        /// <summary>
        /// Reactivate a previously deactivated fee
        /// </summary>
        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("Fee is already active.");

            IsActive = true;
            MarkAsUpdated();
        }

        /// <summary>
        /// Update the fee amount
        /// </summary>
        /// <param name="newAmount">New fee amount</param>
        public void UpdateAmount(decimal newAmount)
        {
            Amount = newAmount;
            MarkAsUpdated();
        }

        /// <summary>
        /// Update the payment due date
        /// </summary>
        /// <param name="dueDate">New due date</param>
        public void UpdateDueDate(DateTime? dueDate)
        {
            DueDate = dueDate;
            MarkAsUpdated();
        }

        /// <summary>
        /// Configure installment payment options
        /// </summary>
        /// <param name="allow">Whether installments are allowed</param>
        /// <param name="maxInstallments">Maximum number of installments</param>
        public void ConfigureInstallments(bool allow, int? maxInstallments = null)
        {
            if (allow && (!maxInstallments.HasValue || maxInstallments.Value < 2))
                throw new ArgumentException("Maximum installments must be at least 2 when installments are allowed.");

            AllowsInstallments = allow;
            MaxInstallments = allow ? maxInstallments : null;
            MarkAsUpdated();
        }

        // Private validation helpers
        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Fee name cannot be empty.");
            if (name.Length > 200)
                throw new ArgumentException("Fee name cannot exceed 200 characters.");
            return name.Trim();
        }
    }
}
