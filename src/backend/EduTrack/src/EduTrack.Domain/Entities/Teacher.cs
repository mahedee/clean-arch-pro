using EduTrack.Domain.Common;
using EduTrack.Domain.Events;
using EduTrack.Domain.ValueObjects;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Teacher aggregate root representing a teacher/faculty member in the education system
    /// </summary>
    public class Teacher : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private FullName _fullName = null!;
        private Email _email = null!;
        private PhoneNumber? _phoneNumber;
        private Address? _address;

        /// <summary>
        /// Teacher's full name
        /// </summary>
        public FullName FullName 
        { 
            get => _fullName;
            private set => _fullName = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Teacher's email address
        /// </summary>
        public Email Email 
        { 
            get => _email;
            private set => _email = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Teacher's phone number (optional)
        /// </summary>
        public PhoneNumber? PhoneNumber 
        { 
            get => _phoneNumber;
            private set => _phoneNumber = value;
        }

        /// <summary>
        /// Teacher's address (optional)
        /// </summary>
        public Address? Address 
        { 
            get => _address;
            private set => _address = value;
        }

        /// <summary>
        /// Teacher's employee ID
        /// </summary>
        public string EmployeeId { get; private set; } = null!;

        /// <summary>
        /// Teacher's department
        /// </summary>
        public string Department { get; private set; } = null!;

        /// <summary>
        /// Teacher's academic title/position
        /// </summary>
        public AcademicTitle Title { get; private set; }

        /// <summary>
        /// Teacher's employment status
        /// </summary>
        public EmploymentStatus Status { get; private set; }

        /// <summary>
        /// Date when teacher was hired
        /// </summary>
        public DateTime HireDate { get; private set; }

        /// <summary>
        /// Teacher's date of birth
        /// </summary>
        public DateTime DateOfBirth { get; private set; }

        /// <summary>
        /// Teacher's specialization areas
        /// </summary>
        public List<string> Specializations { get; private set; } = new();

        /// <summary>
        /// Teacher's qualifications/degrees
        /// </summary>
        public List<string> Qualifications { get; private set; } = new();

        /// <summary>
        /// Maximum number of courses teacher can handle per semester
        /// </summary>
        public int MaxCoursesPerSemester { get; private set; }

        /// <summary>
        /// Current number of assigned courses
        /// </summary>
        public int CurrentCourseLoad { get; private set; }

        /// <summary>
        /// Teacher's office location
        /// </summary>
        public string? OfficeLocation { get; private set; }

        /// <summary>
        /// Teacher's office hours
        /// </summary>
        public string? OfficeHours { get; private set; }

        // Private constructor for EF Core
        private Teacher() : base()
        {
        }

        /// <summary>
        /// Create a new teacher
        /// </summary>
        /// <param name="fullName">Teacher's full name</param>
        /// <param name="email">Teacher's email address</param>
        /// <param name="employeeId">Teacher's employee ID</param>
        /// <param name="department">Teacher's department</param>
        /// <param name="title">Teacher's academic title</param>
        /// <param name="dateOfBirth">Teacher's date of birth</param>
        /// <returns>New teacher instance</returns>
        public static Teacher Create(
            FullName fullName, 
            Email email, 
            string employeeId, 
            string department, 
            AcademicTitle title, 
            DateTime dateOfBirth)
        {
            if (fullName == null)
                throw new ArgumentNullException(nameof(fullName));

            if (email == null)
                throw new ArgumentNullException(nameof(email));

            var teacher = new Teacher
            {
                Id = Guid.NewGuid(),
                _fullName = fullName,
                _email = email,
                EmployeeId = ValidateEmployeeId(employeeId),
                Department = ValidateDepartment(department),
                Title = title,
                DateOfBirth = ValidateDateOfBirth(dateOfBirth),
                HireDate = DateTime.UtcNow,
                Status = EmploymentStatus.Active,
                MaxCoursesPerSemester = GetDefaultMaxCourses(title),
                CurrentCourseLoad = 0,
                Specializations = new List<string>(),
                Qualifications = new List<string>()
            };

            // Raise domain event
            teacher.AddDomainEvent(new TeacherCreatedEvent(teacher.Id, teacher.FullName, teacher.Email, teacher.EmployeeId));

            return teacher;
        }

        /// <summary>
        /// Create a new teacher with string parameters (convenience method)
        /// </summary>
        /// <param name="fullName">Teacher's full name as string</param>
        /// <param name="email">Teacher's email address as string</param>
        /// <param name="employeeId">Teacher's employee ID</param>
        /// <param name="department">Teacher's department</param>
        /// <param name="title">Teacher's academic title</param>
        /// <param name="dateOfBirth">Teacher's date of birth</param>
        /// <returns>New teacher instance</returns>
        public static Teacher Create(
            string fullName, 
            string email, 
            string employeeId, 
            string department, 
            AcademicTitle title, 
            DateTime dateOfBirth)
        {
            var fullNameVO = FullName.Create(fullName);
            var emailVO = Email.Create(email);
            
            return Create(fullNameVO, emailVO, employeeId, department, title, dateOfBirth);
        }

        /// <summary>
        /// Update teacher's contact information
        /// </summary>
        /// <param name="newEmail">New email address</param>
        /// <param name="newPhoneNumber">New phone number (optional)</param>
        /// <param name="newAddress">New address (optional)</param>
        public void UpdateContactInformation(Email newEmail, PhoneNumber? newPhoneNumber = null, Address? newAddress = null)
        {
            if (newEmail == null)
                throw new ArgumentNullException(nameof(newEmail));

            var previousEmail = _email;
            _email = newEmail;
            _phoneNumber = newPhoneNumber;
            _address = newAddress;
            
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new TeacherContactUpdatedEvent(Id, newEmail, previousEmail));
        }

        /// <summary>
        /// Update teacher's contact information with string parameters (convenience method)
        /// </summary>
        /// <param name="newEmail">New email address as string</param>
        /// <param name="newPhoneNumber">New phone number as string (optional)</param>
        public void UpdateContactInformation(string newEmail, string? newPhoneNumber = null)
        {
            var emailVO = Email.Create(newEmail);
            var phoneVO = string.IsNullOrWhiteSpace(newPhoneNumber) ? null : PhoneNumber.Create(newPhoneNumber);
            UpdateContactInformation(emailVO, phoneVO);
        }

        /// <summary>
        /// Assign teacher to a course (increase course load)
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <param name="courseName">Course name</param>
        public void AssignToCourse(Guid courseId, string courseName)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty", nameof(courseId));

            if (string.IsNullOrWhiteSpace(courseName))
                throw new ArgumentException("Course name cannot be empty", nameof(courseName));

            if (Status != EmploymentStatus.Active)
                throw new InvalidOperationException("Cannot assign courses to inactive teachers");

            if (CurrentCourseLoad >= MaxCoursesPerSemester)
                throw new InvalidOperationException($"Teacher has reached maximum course load of {MaxCoursesPerSemester}");

            CurrentCourseLoad++;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new TeacherAssignedToCourseEvent(Id, FullName, courseId, courseName, CurrentCourseLoad));
        }

        /// <summary>
        /// Remove teacher from a course (decrease course load)
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <param name="courseName">Course name</param>
        public void RemoveFromCourse(Guid courseId, string courseName)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty", nameof(courseId));

            if (CurrentCourseLoad <= 0)
                throw new InvalidOperationException("Teacher has no courses assigned to remove");

            CurrentCourseLoad--;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new TeacherRemovedFromCourseEvent(Id, FullName, courseId, courseName, CurrentCourseLoad));
        }

        /// <summary>
        /// Update teacher's academic title
        /// </summary>
        /// <param name="newTitle">New academic title</param>
        public void UpdateTitle(AcademicTitle newTitle)
        {
            var previousTitle = Title;
            Title = newTitle;
            MaxCoursesPerSemester = GetDefaultMaxCourses(newTitle);
            
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new TeacherTitleUpdatedEvent(Id, FullName, newTitle, previousTitle));
        }

        /// <summary>
        /// Set teacher's office information
        /// </summary>
        /// <param name="officeLocation">Office location</param>
        /// <param name="officeHours">Office hours</param>
        public void SetOfficeInfo(string? officeLocation, string? officeHours)
        {
            OfficeLocation = officeLocation;
            OfficeHours = officeHours;
            MarkAsUpdated();
        }

        /// <summary>
        /// Add a specialization to the teacher
        /// </summary>
        /// <param name="specialization">Specialization to add</param>
        public void AddSpecialization(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Specialization cannot be empty", nameof(specialization));

            if (Specializations.Contains(specialization, StringComparer.OrdinalIgnoreCase))
                return; // Already exists

            Specializations.Add(specialization.Trim());
            MarkAsUpdated();
        }

        /// <summary>
        /// Remove a specialization from the teacher
        /// </summary>
        /// <param name="specialization">Specialization to remove</param>
        public void RemoveSpecialization(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                return;

            Specializations.RemoveAll(s => s.Equals(specialization, StringComparison.OrdinalIgnoreCase));
            MarkAsUpdated();
        }

        /// <summary>
        /// Add a qualification to the teacher
        /// </summary>
        /// <param name="qualification">Qualification to add</param>
        public void AddQualification(string qualification)
        {
            if (string.IsNullOrWhiteSpace(qualification))
                throw new ArgumentException("Qualification cannot be empty", nameof(qualification));

            if (Qualifications.Contains(qualification, StringComparer.OrdinalIgnoreCase))
                return; // Already exists

            Qualifications.Add(qualification.Trim());
            MarkAsUpdated();
        }

        /// <summary>
        /// Remove a qualification from the teacher
        /// </summary>
        /// <param name="qualification">Qualification to remove</param>
        public void RemoveQualification(string qualification)
        {
            if (string.IsNullOrWhiteSpace(qualification))
                return;

            Qualifications.RemoveAll(q => q.Equals(qualification, StringComparison.OrdinalIgnoreCase));
            MarkAsUpdated();
        }

        /// <summary>
        /// Set custom maximum courses per semester
        /// </summary>
        /// <param name="maxCourses">Maximum courses per semester</param>
        public void SetMaxCoursesPerSemester(int maxCourses)
        {
            if (maxCourses < 1)
                throw new ArgumentException("Maximum courses must be at least 1", nameof(maxCourses));

            if (maxCourses > 15)
                throw new ArgumentException("Maximum courses cannot exceed 15", nameof(maxCourses));

            if (maxCourses < CurrentCourseLoad)
                throw new InvalidOperationException($"Cannot set max courses to {maxCourses} when current load is {CurrentCourseLoad}");

            MaxCoursesPerSemester = maxCourses;
            MarkAsUpdated();
        }

        /// <summary>
        /// Deactivate the teacher (suspend or terminate employment)
        /// </summary>
        /// <param name="status">New employment status</param>
        public void Deactivate(EmploymentStatus status)
        {
            if (status == EmploymentStatus.Active)
                throw new ArgumentException("Cannot deactivate to active status", nameof(status));

            Status = status;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new TeacherDeactivatedEvent(Id, FullName, status));
        }

        /// <summary>
        /// Reactivate the teacher
        /// </summary>
        public void Reactivate()
        {
            Status = EmploymentStatus.Active;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new TeacherReactivatedEvent(Id, FullName));
        }

        /// <summary>
        /// Check if teacher can take more courses
        /// </summary>
        public bool CanTakeMoreCourses() => Status == EmploymentStatus.Active && CurrentCourseLoad < MaxCoursesPerSemester;

        /// <summary>
        /// Get teacher's workload percentage
        /// </summary>
        public double GetWorkloadPercentage() => MaxCoursesPerSemester > 0 ? (double)CurrentCourseLoad / MaxCoursesPerSemester * 100 : 0;

        /// <summary>
        /// Check if teacher is overloaded
        /// </summary>
        public bool IsOverloaded() => CurrentCourseLoad > MaxCoursesPerSemester;

        /// <summary>
        /// Check if teacher has university email
        /// </summary>
        public bool HasUniversityEmail() => Email.IsUniversityEmail();

        /// <summary>
        /// Get years of service
        /// </summary>
        public int GetYearsOfService() => DateTime.Today.Year - HireDate.Year;

        /// <summary>
        /// Check if teacher is senior (5+ years of service)
        /// </summary>
        public bool IsSeniorTeacher() => GetYearsOfService() >= 5;

        #region Validation Methods

        private static string ValidateEmployeeId(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                throw new ArgumentException("Employee ID cannot be empty", nameof(employeeId));

            if (employeeId.Length < 3)
                throw new ArgumentException("Employee ID must be at least 3 characters long", nameof(employeeId));

            if (employeeId.Length > 20)
                throw new ArgumentException("Employee ID cannot exceed 20 characters", nameof(employeeId));

            return employeeId.Trim().ToUpperInvariant();
        }

        private static string ValidateDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Department cannot be empty", nameof(department));

            if (department.Length < 2)
                throw new ArgumentException("Department must be at least 2 characters long", nameof(department));

            if (department.Length > 50)
                throw new ArgumentException("Department cannot exceed 50 characters", nameof(department));

            return department.Trim();
        }

        private static DateTime ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth >= DateTime.Today)
                throw new ArgumentException("Date of birth must be in the past", nameof(dateOfBirth));

            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < 18)
                throw new ArgumentException("Teacher must be at least 18 years old", nameof(dateOfBirth));

            if (age > 80)
                throw new ArgumentException("Teacher cannot be more than 80 years old", nameof(dateOfBirth));

            return dateOfBirth;
        }

        private static int GetDefaultMaxCourses(AcademicTitle title)
        {
            return title switch
            {
                AcademicTitle.TeachingAssistant => 2,
                AcademicTitle.Lecturer => 4,
                AcademicTitle.AssistantProfessor => 3,
                AcademicTitle.AssociateProfessor => 2,
                AcademicTitle.Professor => 2,
                AcademicTitle.DepartmentHead => 1,
                AcademicTitle.Dean => 1,
                _ => 3
            };
        }

        #endregion
    }

    /// <summary>
    /// Academic title enumeration
    /// </summary>
    public enum AcademicTitle
    {
        TeachingAssistant = 1,
        Lecturer = 2,
        AssistantProfessor = 3,
        AssociateProfessor = 4,
        Professor = 5,
        DepartmentHead = 6,
        Dean = 7,
        Adjunct = 8,
        Visiting = 9
    }

    /// <summary>
    /// Employment status enumeration
    /// </summary>
    public enum EmploymentStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        Terminated = 4,
        Retired = 5,
        OnLeave = 6
    }
}
