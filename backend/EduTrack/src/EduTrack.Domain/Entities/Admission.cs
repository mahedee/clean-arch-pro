using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;
using EduTrack.Domain.Events;
using EduTrack.Domain.ValueObjects;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Admission aggregate root representing a student admission application
    /// </summary>
    public class Admission : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private FullName _applicantName = null!;
        private Email _applicantEmail = null!;
        private PhoneNumber? _phoneNumber;
        private Address? _address;
        private string _programAppliedFor = null!;

        /// <summary>
        /// Applicant's full name
        /// </summary>
        public FullName ApplicantName
        {
            get => _applicantName;
            private set => _applicantName = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Applicant's email address
        /// </summary>
        public Email ApplicantEmail
        {
            get => _applicantEmail;
            private set => _applicantEmail = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Applicant's phone number (optional)
        /// </summary>
        public PhoneNumber? PhoneNumber
        {
            get => _phoneNumber;
            private set => _phoneNumber = value;
        }

        /// <summary>
        /// Applicant's address (optional)
        /// </summary>
        public Address? Address
        {
            get => _address;
            private set => _address = value;
        }

        /// <summary>
        /// Applicant's date of birth
        /// </summary>
        public DateTime DateOfBirth { get; private set; }

        /// <summary>
        /// Program or course the applicant is applying for
        /// </summary>
        public string ProgramAppliedFor
        {
            get => _programAppliedFor;
            private set => _programAppliedFor = ValidateProgram(value);
        }

        /// <summary>
        /// Date the application was submitted
        /// </summary>
        public DateTime ApplicationDate { get; private set; }

        /// <summary>
        /// Current status of the admission application
        /// </summary>
        public AdmissionStatus Status { get; private set; }

        /// <summary>
        /// Notes added by the reviewer during the review process
        /// </summary>
        public string? ReviewNotes { get; private set; }

        /// <summary>
        /// Name of the person who reviewed the application
        /// </summary>
        public string? ReviewedBy { get; private set; }

        /// <summary>
        /// Date and time the decision was made
        /// </summary>
        public DateTime? DecisionDate { get; private set; }

        /// <summary>
        /// Optional reason when application is rejected
        /// </summary>
        public string? RejectionReason { get; private set; }

        /// <summary>
        /// Academic year the applicant is applying for
        /// </summary>
        public int IntendedAcademicYear { get; private set; }

        // Private constructor for EF Core
        private Admission() : base() { }

        /// <summary>
        /// Submit a new admission application
        /// </summary>
        /// <param name="applicantName">Applicant's full name</param>
        /// <param name="dateOfBirth">Applicant's date of birth</param>
        /// <param name="applicantEmail">Applicant's email address</param>
        /// <param name="programAppliedFor">Program the applicant is applying for</param>
        /// <param name="intendedAcademicYear">Intended start academic year</param>
        /// <returns>New admission instance</returns>
        public static Admission Submit(
            FullName applicantName,
            DateTime dateOfBirth,
            Email applicantEmail,
            string programAppliedFor,
            int intendedAcademicYear)
        {
            if (applicantName == null)
                throw new ArgumentNullException(nameof(applicantName));
            if (applicantEmail == null)
                throw new ArgumentNullException(nameof(applicantEmail));
            if (dateOfBirth >= DateTime.Today)
                throw new ArgumentException("Date of birth must be in the past.", nameof(dateOfBirth));
            if (intendedAcademicYear < DateTime.UtcNow.Year)
                throw new ArgumentOutOfRangeException(nameof(intendedAcademicYear), "Intended academic year cannot be in the past.");

            var admission = new Admission
            {
                Id = Guid.NewGuid(),
                ApplicantName = applicantName,
                DateOfBirth = dateOfBirth,
                ApplicantEmail = applicantEmail,
                ProgramAppliedFor = programAppliedFor,
                IntendedAcademicYear = intendedAcademicYear,
                ApplicationDate = DateTime.UtcNow,
                Status = AdmissionStatus.Submitted
            };

            admission.AddDomainEvent(new AdmissionSubmittedEvent(
                admission.Id,
                applicantName.ToString(),
                applicantEmail.Value,
                programAppliedFor));

            return admission;
        }

        /// <summary>
        /// Mark the application as under review
        /// </summary>
        /// <param name="reviewedBy">Reviewer's name</param>
        public void StartReview(string reviewedBy)
        {
            if (Status != AdmissionStatus.Submitted && Status != AdmissionStatus.DocumentsRequired)
                throw new InvalidOperationException("Only submitted or document-required applications can be reviewed.");
            if (string.IsNullOrWhiteSpace(reviewedBy))
                throw new ArgumentException("Reviewer name cannot be empty.", nameof(reviewedBy));

            Status = AdmissionStatus.UnderReview;
            ReviewedBy = reviewedBy;
            MarkAsUpdated(reviewedBy);
        }

        /// <summary>
        /// Request additional documents from the applicant
        /// </summary>
        /// <param name="notes">Details about required documents</param>
        public void RequestDocuments(string notes)
        {
            if (Status != AdmissionStatus.Submitted && Status != AdmissionStatus.UnderReview)
                throw new InvalidOperationException("Documents can only be requested during review stages.");
            if (string.IsNullOrWhiteSpace(notes))
                throw new ArgumentException("Document request notes cannot be empty.", nameof(notes));

            Status = AdmissionStatus.DocumentsRequired;
            ReviewNotes = notes;
            MarkAsUpdated();
        }

        /// <summary>
        /// Place the application on the waiting list
        /// </summary>
        public void WaitList(string? notes = null)
        {
            if (Status != AdmissionStatus.UnderReview)
                throw new InvalidOperationException("Only applications under review can be wait-listed.");

            Status = AdmissionStatus.WaitListed;
            ReviewNotes = notes;
            DecisionDate = DateTime.UtcNow;
            MarkAsUpdated();
        }

        /// <summary>
        /// Accept the admission application
        /// </summary>
        /// <param name="reviewedBy">Reviewer who accepted the application</param>
        /// <param name="notes">Optional acceptance notes</param>
        public void Accept(string reviewedBy, string? notes = null)
        {
            if (Status != AdmissionStatus.UnderReview && Status != AdmissionStatus.WaitListed)
                throw new InvalidOperationException("Only applications under review or wait-listed can be accepted.");
            if (string.IsNullOrWhiteSpace(reviewedBy))
                throw new ArgumentException("Reviewer name cannot be empty.", nameof(reviewedBy));

            Status = AdmissionStatus.Accepted;
            ReviewedBy = reviewedBy;
            ReviewNotes = notes;
            DecisionDate = DateTime.UtcNow;
            MarkAsUpdated(reviewedBy);

            AddDomainEvent(new AdmissionAcceptedEvent(
                Id,
                ApplicantName.ToString(),
                ApplicantEmail.Value,
                ProgramAppliedFor,
                reviewedBy));
        }

        /// <summary>
        /// Reject the admission application
        /// </summary>
        /// <param name="reviewedBy">Reviewer who rejected the application</param>
        /// <param name="reason">Reason for rejection</param>
        public void Reject(string reviewedBy, string? reason = null)
        {
            if (Status == AdmissionStatus.Accepted || Status == AdmissionStatus.Withdrawn)
                throw new InvalidOperationException("Accepted or withdrawn applications cannot be rejected.");
            if (string.IsNullOrWhiteSpace(reviewedBy))
                throw new ArgumentException("Reviewer name cannot be empty.", nameof(reviewedBy));

            Status = AdmissionStatus.Rejected;
            ReviewedBy = reviewedBy;
            RejectionReason = reason;
            DecisionDate = DateTime.UtcNow;
            MarkAsUpdated(reviewedBy);

            AddDomainEvent(new AdmissionRejectedEvent(Id, ApplicantName.ToString(), ApplicantEmail.Value, reason));
        }

        /// <summary>
        /// Withdraw the admission application
        /// </summary>
        public void Withdraw()
        {
            if (Status == AdmissionStatus.Accepted || Status == AdmissionStatus.Rejected)
                throw new InvalidOperationException("Decided applications cannot be withdrawn.");
            if (Status == AdmissionStatus.Withdrawn)
                throw new InvalidOperationException("Application is already withdrawn.");

            Status = AdmissionStatus.Withdrawn;
            DecisionDate = DateTime.UtcNow;
            MarkAsUpdated();
        }

        /// <summary>
        /// Update the applicant's contact information
        /// </summary>
        /// <param name="phoneNumber">New phone number (optional)</param>
        /// <param name="address">New address (optional)</param>
        public void UpdateContactInfo(PhoneNumber? phoneNumber, Address? address)
        {
            PhoneNumber = phoneNumber;
            Address = address;
            MarkAsUpdated();
        }

        // Private validation helpers
        private static string ValidateProgram(string program)
        {
            if (string.IsNullOrWhiteSpace(program))
                throw new ArgumentException("Program applied for cannot be empty.");
            if (program.Length > 200)
                throw new ArgumentException("Program name cannot exceed 200 characters.");
            return program.Trim();
        }
    }
}
