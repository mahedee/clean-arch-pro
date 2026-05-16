using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;
using EduTrack.Domain.Events;
using EduTrack.Domain.ValueObjects;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Department aggregate root representing an academic department in the education system
    /// </summary>
    public class Department : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private string _name = null!;
        private string _code = null!;
        private string _description = string.Empty;
        private Email? _contactEmail;
        private PhoneNumber? _contactPhone;

        /// <summary>
        /// Department name (e.g., "Computer Science", "Mathematics")
        /// </summary>
        public string Name 
        { 
            get => _name;
            private set => _name = ValidateName(value);
        }

        /// <summary>
        /// Department code (e.g., "CS", "MATH", "PHYS")
        /// </summary>
        public string Code 
        { 
            get => _code;
            private set => _code = ValidateCode(value);
        }

        /// <summary>
        /// Department description
        /// </summary>
        public string Description 
        { 
            get => _description;
            private set => _description = value ?? string.Empty;
        }

        /// <summary>
        /// Date when the department was established
        /// </summary>
        public DateTime? EstablishedDate { get; private set; }

        /// <summary>
        /// Department status
        /// </summary>
        public DepartmentStatus Status { get; private set; }

        /// <summary>
        /// Building or location where the department is housed
        /// </summary>
        public string? Location { get; private set; }

        /// <summary>
        /// Department contact email
        /// </summary>
        public Email? ContactEmail 
        { 
            get => _contactEmail;
            private set => _contactEmail = value;
        }

        /// <summary>
        /// Department contact phone number
        /// </summary>
        public PhoneNumber? ContactPhone 
        { 
            get => _contactPhone;
            private set => _contactPhone = value;
        }

        /// <summary>
        /// Department head/chair (Teacher ID reference)
        /// </summary>
        public Guid? DepartmentHeadId { get; private set; }

        /// <summary>
        /// Budget allocated to the department
        /// </summary>
        public decimal? Budget { get; private set; }

        /// <summary>
        /// Number of faculty members in the department
        /// </summary>
        public int FacultyCount { get; private set; }

        /// <summary>
        /// Number of students enrolled in department programs
        /// </summary>
        public int StudentCount { get; private set; }

        // Private constructor for EF Core
        private Department() : base()
        {
        }

        /// <summary>
        /// Create a new department
        /// </summary>
        /// <param name="name">Department name</param>
        /// <param name="code">Department code</param>
        /// <param name="description">Department description</param>
        /// <returns>New department instance</returns>
        public static Department Create(string name, string code, string? description = null)
        {
            var department = new Department
            {
                Id = Guid.NewGuid(),
                _name = ValidateName(name),
                _code = ValidateCode(code),
                _description = description ?? string.Empty,
                Status = DepartmentStatus.Active,
                EstablishedDate = DateTime.UtcNow,
                FacultyCount = 0,
                StudentCount = 0
            };

            // Raise domain event
            department.AddDomainEvent(new DepartmentCreatedEvent(department.Id, department.Name, department.Code));

            return department;
        }

        /// <summary>
        /// Update department information
        /// </summary>
        /// <param name="name">New name</param>
        /// <param name="description">New description</param>
        public void UpdateInformation(string name, string? description = null)
        {
            var previousName = _name;
            _name = ValidateName(name);
            _description = description ?? string.Empty;
            
            MarkAsUpdated();

            // Raise domain event if name changed
            if (previousName != _name)
            {
                AddDomainEvent(new DepartmentNameUpdatedEvent(Id, previousName, _name));
            }
        }

        /// <summary>
        /// Update department contact information
        /// </summary>
        /// <param name="contactEmail">Contact email</param>
        /// <param name="contactPhone">Contact phone number</param>
        public void UpdateContactInformation(Email? contactEmail = null, PhoneNumber? contactPhone = null)
        {
            _contactEmail = contactEmail;
            _contactPhone = contactPhone;
            MarkAsUpdated();

            AddDomainEvent(new DepartmentContactUpdatedEvent(Id, Name, contactEmail?.Value, contactPhone?.Value));
        }

        /// <summary>
        /// Assign department head
        /// </summary>
        /// <param name="teacherId">Teacher ID to assign as department head</param>
        public void AssignDepartmentHead(Guid teacherId)
        {
            if (teacherId == Guid.Empty)
                throw new ArgumentException("Teacher ID cannot be empty", nameof(teacherId));

            var previousHeadId = DepartmentHeadId;
            DepartmentHeadId = teacherId;
            MarkAsUpdated();

            AddDomainEvent(new DepartmentHeadAssignedEvent(Id, Name, previousHeadId, teacherId));
        }

        /// <summary>
        /// Remove department head
        /// </summary>
        public void RemoveDepartmentHead()
        {
            var previousHeadId = DepartmentHeadId;
            DepartmentHeadId = null;
            MarkAsUpdated();

            if (previousHeadId.HasValue)
            {
                AddDomainEvent(new DepartmentHeadRemovedEvent(Id, Name, previousHeadId.Value));
            }
        }

        /// <summary>
        /// Update department location
        /// </summary>
        /// <param name="location">New location</param>
        public void UpdateLocation(string? location)
        {
            Location = location;
            MarkAsUpdated();
        }

        /// <summary>
        /// Update department budget
        /// </summary>
        /// <param name="budget">New budget amount</param>
        public void UpdateBudget(decimal? budget)
        {
            if (budget < 0)
                throw new ArgumentException("Budget cannot be negative", nameof(budget));

            Budget = budget;
            MarkAsUpdated();
        }

        /// <summary>
        /// Update faculty count
        /// </summary>
        /// <param name="facultyCount">Number of faculty members</param>
        public void UpdateFacultyCount(int facultyCount)
        {
            if (facultyCount < 0)
                throw new ArgumentException("Faculty count cannot be negative", nameof(facultyCount));

            FacultyCount = facultyCount;
            MarkAsUpdated();
        }

        /// <summary>
        /// Update student count
        /// </summary>
        /// <param name="studentCount">Number of students</param>
        public void UpdateStudentCount(int studentCount)
        {
            if (studentCount < 0)
                throw new ArgumentException("Student count cannot be negative", nameof(studentCount));

            StudentCount = studentCount;
            MarkAsUpdated();
        }

        /// <summary>
        /// Activate the department
        /// </summary>
        public void Activate()
        {
            if (Status == DepartmentStatus.Active)
                return;

            Status = DepartmentStatus.Active;
            MarkAsUpdated();

            AddDomainEvent(new DepartmentStatusChangedEvent(
                Id,
                Name,
                DepartmentStatus.Inactive,
                DepartmentStatus.Active,
                "Department activated"));
        }

        /// <summary>
        /// Deactivate the department
        /// </summary>
        /// <param name="reason">Reason for deactivation</param>
        public void Deactivate(string? reason = null)
        {
            if (Status == DepartmentStatus.Inactive)
                return;

            Status = DepartmentStatus.Inactive;
            MarkAsUpdated();

            AddDomainEvent(new DepartmentStatusChangedEvent(
                Id,
                Name,
                DepartmentStatus.Active,
                DepartmentStatus.Inactive,
                reason ?? "Department deactivated"));
        }

        #region Validation Methods

        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Department name is required", nameof(name));

            if (name.Length > 200)
                throw new ArgumentException("Department name cannot exceed 200 characters", nameof(name));

            return name.Trim();
        }

        private static string ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Department code is required", nameof(code));

            if (code.Length > 10)
                throw new ArgumentException("Department code cannot exceed 10 characters", nameof(code));

            // Ensure code is uppercase and contains only letters and numbers
            var normalizedCode = code.Trim().ToUpperInvariant();
            if (!System.Text.RegularExpressions.Regex.IsMatch(normalizedCode, "^[A-Z0-9]+$"))
                throw new ArgumentException("Department code can only contain letters and numbers", nameof(code));

            return normalizedCode;
        }

        #endregion
    }

}