using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;
using EduTrack.Domain.Events;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Scholarship aggregate root representing a scholarship program offered by the institution
    /// </summary>
    public class Scholarship : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private string _name = null!;
        private string _description = null!;
        private decimal _amount;
        private decimal _minimumGPA;

        /// <summary>
        /// Name of the scholarship program
        /// </summary>
        public string Name
        {
            get => _name;
            private set => _name = ValidateName(value);
        }

        /// <summary>
        /// Detailed description of the scholarship criteria and benefits
        /// </summary>
        public string Description
        {
            get => _description;
            private set => _description = value?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Monetary value of the scholarship (per academic year or per semester)
        /// </summary>
        public decimal Amount
        {
            get => _amount;
            private set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Amount), "Scholarship amount must be greater than zero.");
                _amount = value;
            }
        }

        /// <summary>
        /// Scholarship category
        /// </summary>
        public ScholarshipType Type { get; private set; }

        /// <summary>
        /// Current status of the scholarship program
        /// </summary>
        public ScholarshipStatus Status { get; private set; }

        /// <summary>
        /// Minimum GPA required to qualify (0.0 means no GPA requirement)
        /// </summary>
        public decimal MinimumGPA
        {
            get => _minimumGPA;
            private set
            {
                if (value < 0 || value > 4.0m)
                    throw new ArgumentOutOfRangeException(nameof(MinimumGPA), "Minimum GPA must be between 0.0 and 4.0.");
                _minimumGPA = value;
            }
        }

        /// <summary>
        /// Date from which this scholarship is available
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Date after which no new recipients can be added
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Maximum number of students that can receive this scholarship simultaneously
        /// </summary>
        public int MaxRecipients { get; private set; }

        /// <summary>
        /// Current number of active recipients
        /// </summary>
        public int CurrentRecipients { get; private set; }

        /// <summary>
        /// Whether there are still slots available for new recipients
        /// </summary>
        public bool HasAvailableSlots => CurrentRecipients < MaxRecipients && Status == ScholarshipStatus.Active;

        /// <summary>
        /// Optional additional eligibility criteria description
        /// </summary>
        public string? EligibilityCriteria { get; private set; }

        // Private constructor for EF Core
        private Scholarship() : base() { }

        /// <summary>
        /// Create a new scholarship program
        /// </summary>
        /// <param name="name">Scholarship name</param>
        /// <param name="description">Scholarship description</param>
        /// <param name="type">Scholarship category</param>
        /// <param name="amount">Monetary value</param>
        /// <param name="minimumGPA">Minimum GPA required (0.0 for no requirement)</param>
        /// <param name="startDate">Scholarship availability start date</param>
        /// <param name="endDate">Scholarship availability end date</param>
        /// <param name="maxRecipients">Maximum number of recipients</param>
        /// <param name="eligibilityCriteria">Optional additional eligibility criteria</param>
        /// <returns>New scholarship instance in Active status</returns>
        public static Scholarship Create(
            string name,
            string description,
            ScholarshipType type,
            decimal amount,
            decimal minimumGPA,
            DateTime startDate,
            DateTime endDate,
            int maxRecipients,
            string? eligibilityCriteria = null)
        {
            if (endDate <= startDate)
                throw new ArgumentException("End date must be after start date.", nameof(endDate));
            if (maxRecipients < 1)
                throw new ArgumentOutOfRangeException(nameof(maxRecipients), "Maximum recipients must be at least 1.");

            var scholarship = new Scholarship
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Type = type,
                Amount = amount,
                MinimumGPA = minimumGPA,
                StartDate = startDate,
                EndDate = endDate,
                MaxRecipients = maxRecipients,
                CurrentRecipients = 0,
                EligibilityCriteria = eligibilityCriteria?.Trim(),
                Status = ScholarshipStatus.Active
            };

            scholarship.AddDomainEvent(new ScholarshipCreatedEvent(scholarship.Id, name, amount, maxRecipients));
            return scholarship;
        }

        /// <summary>
        /// Add a new recipient to the scholarship
        /// </summary>
        public void AddRecipient()
        {
            if (Status != ScholarshipStatus.Active)
                throw new InvalidOperationException("Only active scholarships can accept new recipients.");
            if (CurrentRecipients >= MaxRecipients)
                throw new InvalidOperationException("Scholarship has reached its maximum number of recipients.");
            if (DateTime.UtcNow > EndDate)
                throw new InvalidOperationException("Scholarship has expired.");

            CurrentRecipients++;
            MarkAsUpdated();
        }

        /// <summary>
        /// Remove a recipient from the scholarship
        /// </summary>
        public void RemoveRecipient()
        {
            if (CurrentRecipients <= 0)
                throw new InvalidOperationException("No recipients to remove.");

            CurrentRecipients--;
            MarkAsUpdated();
        }

        /// <summary>
        /// Deactivate the scholarship (new applications no longer accepted)
        /// </summary>
        public void Deactivate()
        {
            if (Status != ScholarshipStatus.Active)
                throw new InvalidOperationException("Only active scholarships can be deactivated.");

            Status = ScholarshipStatus.Inactive;
            MarkAsUpdated();
        }

        /// <summary>
        /// Reactivate a deactivated scholarship
        /// </summary>
        public void Reactivate()
        {
            if (Status != ScholarshipStatus.Inactive)
                throw new InvalidOperationException("Only inactive scholarships can be reactivated.");
            if (DateTime.UtcNow > EndDate)
                throw new InvalidOperationException("Cannot reactivate an expired scholarship.");

            Status = ScholarshipStatus.Active;
            MarkAsUpdated();
        }

        /// <summary>
        /// Suspend the scholarship temporarily
        /// </summary>
        public void Suspend()
        {
            if (Status == ScholarshipStatus.Expired || Status == ScholarshipStatus.Suspended)
                throw new InvalidOperationException("Scholarship is already suspended or expired.");

            Status = ScholarshipStatus.Suspended;
            MarkAsUpdated();
        }

        /// <summary>
        /// Mark the scholarship as expired
        /// </summary>
        public void Expire()
        {
            if (Status == ScholarshipStatus.Expired)
                throw new InvalidOperationException("Scholarship is already expired.");

            Status = ScholarshipStatus.Expired;
            MarkAsUpdated();
        }

        /// <summary>
        /// Update the scholarship amount
        /// </summary>
        /// <param name="newAmount">New scholarship amount</param>
        public void UpdateAmount(decimal newAmount)
        {
            Amount = newAmount;
            MarkAsUpdated();
        }

        /// <summary>
        /// Extend the scholarship end date
        /// </summary>
        /// <param name="newEndDate">New end date (must be after current end date)</param>
        public void ExtendEndDate(DateTime newEndDate)
        {
            if (newEndDate <= EndDate)
                throw new ArgumentException("New end date must be after the current end date.", nameof(newEndDate));

            EndDate = newEndDate;

            if (Status == ScholarshipStatus.Expired)
                Status = ScholarshipStatus.Active;

            MarkAsUpdated();
        }

        // Private validation helpers
        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Scholarship name cannot be empty.");
            if (name.Length > 200)
                throw new ArgumentException("Scholarship name cannot exceed 200 characters.");
            return name.Trim();
        }
    }
}
