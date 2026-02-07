using EduTrack.Domain.Common;
using EduTrack.Domain.Events;
using EduTrack.Domain.ValueObjects;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Student aggregate root representing a student in the education system
    /// </summary>
    public class Student : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private FullName _fullName = null!;
        private Email _email = null!;
        private PhoneNumber? _phoneNumber;
        private Address? _address;
        private GPA? _currentGPA;

        /// <summary>
        /// Student's full name
        /// </summary>
        public FullName FullName 
        { 
            get => _fullName;
            private set => _fullName = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Student's date of birth
        /// </summary>
        public DateTime DateOfBirth { get; private set; }

        /// <summary>
        /// Student's email address
        /// </summary>
        public Email Email 
        { 
            get => _email;
            private set => _email = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Student's phone number (optional)
        /// </summary>
        public PhoneNumber? PhoneNumber 
        { 
            get => _phoneNumber;
            private set => _phoneNumber = value;
        }

        /// <summary>
        /// Student's address (optional)
        /// </summary>
        public Address? Address 
        { 
            get => _address;
            private set => _address = value;
        }

        /// <summary>
        /// Student's current GPA (optional)
        /// </summary>
        public GPA? CurrentGPA 
        { 
            get => _currentGPA;
            private set => _currentGPA = value;
        }

        /// <summary>
        /// Student's enrollment date
        /// </summary>
        public DateTime EnrollmentDate { get; private set; }

        /// <summary>
        /// Student's current status
        /// </summary>
        public StudentStatus Status { get; private set; }

        // Private constructor for EF Core
        private Student() : base()
        {
        }

        /// <summary>
        /// Create a new student
        /// </summary>
        /// <param name="fullName">Student's full name</param>
        /// <param name="dateOfBirth">Student's date of birth</param>
        /// <param name="email">Student's email address</param>
        /// <returns>New student instance</returns>
        public static Student Create(FullName fullName, DateTime dateOfBirth, Email email)
        {
            if (fullName == null)
                throw new ArgumentNullException(nameof(fullName));

            if (email == null)
                throw new ArgumentNullException(nameof(email));

            if (dateOfBirth >= DateTime.Today)
                throw new ArgumentException("Date of birth must be in the past", nameof(dateOfBirth));

            var student = new Student
            {
                Id = Guid.NewGuid(),
                _fullName = fullName,
                _email = email,
                DateOfBirth = dateOfBirth,
                EnrollmentDate = DateTime.UtcNow,
                Status = StudentStatus.Active
            };

            // Raise domain event
            student.AddDomainEvent(new StudentCreatedEvent(student.Id, student.FullName, student.Email));

            return student;
        }

        /// <summary>
        /// Create a new student with string parameters (convenience method)
        /// </summary>
        /// <param name="fullName">Student's full name as string</param>
        /// <param name="dateOfBirth">Student's date of birth</param>
        /// <param name="email">Student's email address as string</param>
        /// <returns>New student instance</returns>
        public static Student Create(string fullName, DateTime dateOfBirth, string email)
        {
            var fullNameVO = FullName.Create(fullName);
            var emailVO = Email.Create(email);
            
            return Create(fullNameVO, dateOfBirth, emailVO);
        }

        /// <summary>
        /// Update student's contact information
        /// </summary>
        /// <param name="newEmail">New email address</param>
        public void UpdateContactInformation(Email newEmail)
        {
            if (newEmail == null)
                throw new ArgumentNullException(nameof(newEmail));

            var previousEmail = _email;
            _email = newEmail;
            
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new StudentContactUpdatedEvent(Id, newEmail, previousEmail));
        }

        /// <summary>
        /// Update student's contact information with string parameter (convenience method)
        /// </summary>
        /// <param name="newEmail">New email address as string</param>
        public void UpdateContactInformation(string newEmail)
        {
            var emailVO = Email.Create(newEmail);
            UpdateContactInformation(emailVO);
        }

        /// <summary>
        /// Update student's full name
        /// </summary>
        /// <param name="newFullName">New full name</param>
        public void UpdateFullName(FullName newFullName)
        {
            if (newFullName == null)
                throw new ArgumentNullException(nameof(newFullName));

            var previousFullName = _fullName;
            _fullName = newFullName;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new StudentPersonalInfoUpdatedEvent(
                Id, 
                previousFullName?.Value, 
                newFullName.Value));
        }

        /// <summary>
        /// Update student's full name with string parameter (convenience method)
        /// </summary>
        /// <param name="newFullName">New full name as string</param>
        public void UpdateFullName(string newFullName)
        {
            var fullNameVO = FullName.Create(newFullName);
            UpdateFullName(fullNameVO);
        }

        /// <summary>
        /// Update student's phone number
        /// </summary>
        /// <param name="phoneNumber">New phone number</param>
        public void UpdatePhoneNumber(PhoneNumber? phoneNumber)
        {
            var previousPhoneNumber = _phoneNumber;
            _phoneNumber = phoneNumber;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new StudentPhoneNumberUpdatedEvent(
                Id, 
                FullName.Value, 
                Email.Value, 
                previousPhoneNumber?.Value, 
                phoneNumber?.Value));
        }

        /// <summary>
        /// Update student's phone number with string parameter (convenience method)
        /// </summary>
        /// <param name="phoneNumber">New phone number as string</param>
        public void UpdatePhoneNumber(string? phoneNumber)
        {
            _phoneNumber = string.IsNullOrWhiteSpace(phoneNumber) 
                ? null 
                : PhoneNumber.Create(phoneNumber);
            MarkAsUpdated();
        }

        /// <summary>
        /// Update student's address
        /// </summary>
        /// <param name="address">New address</param>
        public void UpdateAddress(Address? address)
        {
            var previousAddress = _address?.ToString();
            _address = address;
            MarkAsUpdated();

            // Raise domain event if address is being set
            if (address != null)
            {
                AddDomainEvent(new StudentAddressUpdatedEvent(
                    Id,
                    FullName.Value,
                    Email.Value,
                    previousAddress,
                    address.Street,
                    address.City,
                    address.State,
                    address.ZipCode,
                    address.Country));
            }
        }

        /// <summary>
        /// Update student's GPA
        /// </summary>
        /// <param name="gpa">New GPA</param>
        public void UpdateGPA(GPA? gpa)
        {
            var previousGPA = _currentGPA;
            _currentGPA = gpa;
            MarkAsUpdated();

            // Raise domain event if GPA is being set
            if (gpa != null)
            {
                AddDomainEvent(new StudentGPAUpdatedEvent(
                    Id,
                    FullName.Value,
                    Email.Value,
                    previousGPA?.Value,
                    gpa.Value));

                // Check for academic standing changes
                var previousStanding = previousGPA?.AcademicStanding ?? "No GPA Recorded";
                var newStanding = gpa.AcademicStanding;

                if (previousStanding != newStanding)
                {
                    AddDomainEvent(new StudentAcademicStandingChangedEvent(
                        Id,
                        FullName.Value,
                        Email.Value,
                        gpa.Value,
                        previousStanding,
                        newStanding,
                        "GPA Update"));
                }
            }
        }

        /// <summary>
        /// Update student's GPA with decimal parameter (convenience method)
        /// </summary>
        /// <param name="gpaValue">New GPA value</param>
        public void UpdateGPA(decimal? gpaValue)
        {
            _currentGPA = gpaValue.HasValue ? GPA.Create(gpaValue.Value) : null;
            MarkAsUpdated();
        }

        /// <summary>
        /// Check if student is eligible for honors based on GPA
        /// </summary>
        public bool IsEligibleForHonors() => CurrentGPA?.IsHonorsLevel ?? false;

        /// <summary>
        /// Check if student is on academic probation
        /// </summary>
        public bool IsOnAcademicProbation() => CurrentGPA?.IsOnProbation ?? false;

        /// <summary>
        /// Get student's academic standing
        /// </summary>
        public string GetAcademicStanding() => CurrentGPA?.AcademicStanding ?? "No GPA Recorded";

        /// <summary>
        /// Check if student has a university email
        /// </summary>
        public bool HasUniversityEmail() => Email.IsUniversityEmail();

        /// <summary>
        /// Deactivate the student
        /// </summary>
        public void Deactivate()
        {
            var previousStatus = Status;
            Status = StudentStatus.Inactive;
            MarkAsUpdated();

            // Raise domain events
            AddDomainEvent(new StudentStatusChangedEvent(
                Id,
                FullName.Value,
                Email.Value,
                previousStatus,
                StudentStatus.Inactive,
                "Student deactivated"));

            AddDomainEvent(new StudentDeactivatedEvent(
                Id,
                FullName.Value,
                Email.Value,
                "Student deactivated"));
        }

        /// <summary>
        /// Reactivate the student
        /// </summary>
        public void Reactivate()
        {
            var previousStatus = Status;
            Status = StudentStatus.Active;
            MarkAsUpdated();

            // Raise domain events
            AddDomainEvent(new StudentStatusChangedEvent(
                Id,
                FullName.Value,
                Email.Value,
                previousStatus,
                StudentStatus.Active,
                "Student reactivated"));

            AddDomainEvent(new StudentReactivatedEvent(
                Id,
                FullName.Value,
                Email.Value,
                "Student reactivated"));
        }

        /// <summary>
        /// Graduate the student
        /// </summary>
        public void Graduate()
        {
            var previousStatus = Status;
            Status = StudentStatus.Graduated;
            MarkAsUpdated();

            // Raise domain events
            AddDomainEvent(new StudentStatusChangedEvent(
                Id,
                FullName.Value,
                Email.Value,
                previousStatus,
                StudentStatus.Graduated,
                "Student graduated"));

            AddDomainEvent(new StudentGraduatedEvent(
                Id,
                FullName.Value,
                Email.Value,
                CurrentGPA?.Value,
                null, // degree - could be passed as parameter
                null, // major - could be passed as parameter
                CurrentGPA?.IsHonorsLevel == true ? "Honors" : null));
        }
    }

    /// <summary>
    /// Student status enumeration
    /// </summary>
    public enum StudentStatus
    {
        Active = 1,
        Inactive = 2,
        Graduated = 3,
        Suspended = 4,
        Expelled = 5
    }
}
