using EduTrack.Domain.Entities;
using EduTrack.Domain.Events;
using EduTrack.Domain.ValueObjects;
using Xunit;

namespace EduTrack.Domain.UnitTests.Entities
{
    public class TeacherTests
    {
        [Fact]
        public void Create_ValidData_ShouldCreateTeacherWithCorrectProperties()
        {
            // Arrange
            var fullName = "Dr. Jane Smith";
            var email = "jane.smith@university.edu";
            var employeeId = "EMP001";
            var department = "Computer Science";
            var title = AcademicTitle.AssistantProfessor;
            var dateOfBirth = new DateTime(1980, 5, 15);

            // Act
            var teacher = Teacher.Create(fullName, email, employeeId, department, title, dateOfBirth);

            // Assert
            Assert.NotEqual(Guid.Empty, teacher.Id);
            Assert.Equal(fullName, teacher.FullName);
            Assert.Equal(email, teacher.Email);
            Assert.Equal(employeeId.ToUpperInvariant(), teacher.EmployeeId);
            Assert.Equal(department, teacher.Department);
            Assert.Equal(title, teacher.Title);
            Assert.Equal(dateOfBirth, teacher.DateOfBirth);
            Assert.Equal(EmploymentStatus.Active, teacher.Status);
            Assert.True(teacher.HireDate <= DateTime.UtcNow);
        }

        [Fact]
        public void Create_ValidData_ShouldRaiseTeacherCreatedEvent()
        {
            // Arrange
            var fullName = "Dr. Jane Smith";
            var email = "jane.smith@university.edu";
            var employeeId = "EMP001";
            var department = "Computer Science";
            var title = AcademicTitle.AssistantProfessor;
            var dateOfBirth = new DateTime(1980, 5, 15);

            // Act
            var teacher = Teacher.Create(fullName, email, employeeId, department, title, dateOfBirth);

            // Assert
            Assert.Single(teacher.DomainEvents);
            var domainEvent = teacher.DomainEvents.First();
            Assert.IsType<TeacherCreatedEvent>(domainEvent);
            
            var teacherCreatedEvent = (TeacherCreatedEvent)domainEvent;
            Assert.Equal(teacher.Id, teacherCreatedEvent.TeacherId);
            Assert.Equal(fullName, teacherCreatedEvent.FullName);
            Assert.Equal(email, teacherCreatedEvent.Email);
            Assert.Equal(employeeId.ToUpperInvariant(), teacherCreatedEvent.EmployeeId);
        }

        [Fact]
        public void Create_WithValueObjects_ShouldCreateTeacherSuccessfully()
        {
            // Arrange
            var fullName = FullName.Create("Dr. John Doe");
            var email = Email.Create("john.doe@university.edu");
            var employeeId = "EMP002";
            var department = "Mathematics";
            var title = AcademicTitle.Professor;
            var dateOfBirth = new DateTime(1975, 3, 20);

            // Act
            var teacher = Teacher.Create(fullName, email, employeeId, department, title, dateOfBirth);

            // Assert
            Assert.Equal(fullName, teacher.FullName);
            Assert.Equal(email, teacher.Email);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Create_InvalidEmployeeId_ShouldThrowArgumentException(string invalidEmployeeId)
        {
            // Arrange
            var fullName = "Dr. Jane Smith";
            var email = "jane.smith@university.edu";
            var department = "Computer Science";
            var title = AcademicTitle.AssistantProfessor;
            var dateOfBirth = new DateTime(1980, 5, 15);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Teacher.Create(fullName, email, invalidEmployeeId, department, title, dateOfBirth));
        }

        [Theory]
        [InlineData("AB")]
        [InlineData("A")]
        public void Create_EmployeeIdTooShort_ShouldThrowArgumentException(string shortEmployeeId)
        {
            // Arrange
            var fullName = "Dr. Jane Smith";
            var email = "jane.smith@university.edu";
            var department = "Computer Science";
            var title = AcademicTitle.AssistantProfessor;
            var dateOfBirth = new DateTime(1980, 5, 15);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Teacher.Create(fullName, email, shortEmployeeId, department, title, dateOfBirth));
        }

        [Fact]
        public void Create_EmployeeIdTooLong_ShouldThrowArgumentException()
        {
            // Arrange
            var fullName = "Dr. Jane Smith";
            var email = "jane.smith@university.edu";
            var longEmployeeId = new string('A', 21); // 21 characters
            var department = "Computer Science";
            var title = AcademicTitle.AssistantProfessor;
            var dateOfBirth = new DateTime(1980, 5, 15);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Teacher.Create(fullName, email, longEmployeeId, department, title, dateOfBirth));
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        [InlineData("A")]
        public void Create_InvalidDepartment_ShouldThrowArgumentException(string invalidDepartment)
        {
            // Arrange
            var fullName = "Dr. Jane Smith";
            var email = "jane.smith@university.edu";
            var employeeId = "EMP001";
            var title = AcademicTitle.AssistantProfessor;
            var dateOfBirth = new DateTime(1980, 5, 15);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Teacher.Create(fullName, email, employeeId, invalidDepartment, title, dateOfBirth));
        }

        [Fact]
        public void Create_DateOfBirthInFuture_ShouldThrowArgumentException()
        {
            // Arrange
            var fullName = "Dr. Jane Smith";
            var email = "jane.smith@university.edu";
            var employeeId = "EMP001";
            var department = "Computer Science";
            var title = AcademicTitle.AssistantProfessor;
            var futureDateOfBirth = DateTime.Today.AddDays(1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Teacher.Create(fullName, email, employeeId, department, title, futureDateOfBirth));
        }

        [Fact]
        public void Create_DateOfBirthTooYoung_ShouldThrowArgumentException()
        {
            // Arrange
            var fullName = "Dr. Jane Smith";
            var email = "jane.smith@university.edu";
            var employeeId = "EMP001";
            var department = "Computer Science";
            var title = AcademicTitle.AssistantProfessor;
            var youngDateOfBirth = DateTime.Today.AddYears(-17); // 17 years old

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Teacher.Create(fullName, email, employeeId, department, title, youngDateOfBirth));
        }

        [Fact]
        public void Create_DateOfBirthTooOld_ShouldThrowArgumentException()
        {
            // Arrange
            var fullName = "Dr. Jane Smith";
            var email = "jane.smith@university.edu";
            var employeeId = "EMP001";
            var department = "Computer Science";
            var title = AcademicTitle.AssistantProfessor;
            var oldDateOfBirth = DateTime.Today.AddYears(-81); // 81 years old

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Teacher.Create(fullName, email, employeeId, department, title, oldDateOfBirth));
        }

        [Theory]
        [InlineData(AcademicTitle.TeachingAssistant, 2)]
        [InlineData(AcademicTitle.Lecturer, 4)]
        [InlineData(AcademicTitle.AssistantProfessor, 3)]
        [InlineData(AcademicTitle.AssociateProfessor, 2)]
        [InlineData(AcademicTitle.Professor, 2)]
        [InlineData(AcademicTitle.DepartmentHead, 1)]
        [InlineData(AcademicTitle.Dean, 1)]
        public void Create_DifferentTitles_ShouldSetCorrectMaxCourses(AcademicTitle title, int expectedMaxCourses)
        {
            // Arrange
            var fullName = "Dr. Jane Smith";
            var email = "jane.smith@university.edu";
            var employeeId = "EMP001";
            var department = "Computer Science";
            var dateOfBirth = new DateTime(1980, 5, 15);

            // Act
            var teacher = Teacher.Create(fullName, email, employeeId, department, title, dateOfBirth);

            // Assert
            Assert.Equal(expectedMaxCourses, teacher.MaxCoursesPerSemester);
        }

        [Fact]
        public void UpdateContactInformation_ValidData_ShouldUpdateSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var newEmail = Email.Create("newemail@university.edu");
            var newPhoneNumber = PhoneNumber.Create("+1-555-234-5678");

            // Act
            teacher.UpdateContactInformation(newEmail, newPhoneNumber);

            // Assert
            Assert.Equal(newEmail, teacher.Email);
            Assert.Equal(newPhoneNumber, teacher.PhoneNumber);
            Assert.Contains(teacher.DomainEvents, e => e is TeacherContactUpdatedEvent);
        }

        [Fact]
        public void UpdateContactInformation_WithStringParams_ShouldUpdateSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var newEmail = "newemail@university.edu";
            var newPhoneNumber = "+1-555-234-5678";

            // Act
            teacher.UpdateContactInformation(newEmail, newPhoneNumber);

            // Assert
            Assert.Equal(newEmail, teacher.Email);
            Assert.Equal("(555) 234-5678", teacher.PhoneNumber); // PhoneNumber formats as (XXX) XXX-XXXX
        }

        [Fact]
        public void AssignToCourse_ValidCourse_ShouldAssignSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var courseId = Guid.NewGuid();
            var courseName = "Introduction to Programming";

            // Act
            teacher.AssignToCourse(courseId, courseName);

            // Assert
            Assert.Equal(1, teacher.CurrentCourseLoad);
            Assert.Contains(teacher.DomainEvents, e => e is TeacherAssignedToCourseEvent);
        }

        [Fact]
        public void AssignToCourse_InactiveTeacher_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            teacher.Deactivate(EmploymentStatus.Suspended);
            var courseId = Guid.NewGuid();
            var courseName = "Introduction to Programming";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => teacher.AssignToCourse(courseId, courseName));
        }

        [Fact]
        public void AssignToCourse_AtMaxCapacity_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var teacher = CreateValidTeacher(); // Assistant Professor has max 3 courses
            var courseId = Guid.NewGuid();
            
            // Assign maximum courses
            for (int i = 0; i < teacher.MaxCoursesPerSemester; i++)
            {
                teacher.AssignToCourse(Guid.NewGuid(), $"Course {i + 1}");
            }

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                teacher.AssignToCourse(courseId, "One More Course"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void AssignToCourse_InvalidCourseName_ShouldThrowArgumentException(string invalidCourseName)
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var courseId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => teacher.AssignToCourse(courseId, invalidCourseName));
        }

        [Fact]
        public void RemoveFromCourse_WithAssignedCourses_ShouldRemoveSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var courseId = Guid.NewGuid();
            var courseName = "Introduction to Programming";
            teacher.AssignToCourse(courseId, courseName);

            // Act
            teacher.RemoveFromCourse(courseId, courseName);

            // Assert
            Assert.Equal(0, teacher.CurrentCourseLoad);
            Assert.Contains(teacher.DomainEvents, e => e is TeacherRemovedFromCourseEvent);
        }

        [Fact]
        public void RemoveFromCourse_NoCourses_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var courseId = Guid.NewGuid();
            var courseName = "Introduction to Programming";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => teacher.RemoveFromCourse(courseId, courseName));
        }

        [Fact]
        public void UpdateTitle_ValidTitle_ShouldUpdateSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var newTitle = AcademicTitle.Professor;

            // Act
            teacher.UpdateTitle(newTitle);

            // Assert
            Assert.Equal(newTitle, teacher.Title);
            Assert.Equal(2, teacher.MaxCoursesPerSemester); // Professor has max 2 courses
            Assert.Contains(teacher.DomainEvents, e => e is TeacherTitleUpdatedEvent);
        }

        [Fact]
        public void AddSpecialization_ValidSpecialization_ShouldAddSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var specialization = "Machine Learning";

            // Act
            teacher.AddSpecialization(specialization);

            // Assert
            Assert.Contains(specialization, teacher.Specializations);
        }

        [Fact]
        public void AddSpecialization_DuplicateSpecialization_ShouldNotAddDuplicate()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var specialization = "Machine Learning";
            teacher.AddSpecialization(specialization);

            // Act
            teacher.AddSpecialization(specialization);

            // Assert
            Assert.Single(teacher.Specializations);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void AddSpecialization_InvalidSpecialization_ShouldThrowArgumentException(string invalidSpecialization)
        {
            // Arrange
            var teacher = CreateValidTeacher();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => teacher.AddSpecialization(invalidSpecialization));
        }

        [Fact]
        public void RemoveSpecialization_ExistingSpecialization_ShouldRemoveSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var specialization = "Machine Learning";
            teacher.AddSpecialization(specialization);

            // Act
            teacher.RemoveSpecialization(specialization);

            // Assert
            Assert.DoesNotContain(specialization, teacher.Specializations);
        }

        [Fact]
        public void AddQualification_ValidQualification_ShouldAddSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var qualification = "Ph.D. in Computer Science";

            // Act
            teacher.AddQualification(qualification);

            // Assert
            Assert.Contains(qualification, teacher.Qualifications);
        }

        [Fact]
        public void SetMaxCoursesPerSemester_ValidNumber_ShouldSetSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var maxCourses = 5;

            // Act
            teacher.SetMaxCoursesPerSemester(maxCourses);

            // Assert
            Assert.Equal(maxCourses, teacher.MaxCoursesPerSemester);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(16)]
        public void SetMaxCoursesPerSemester_InvalidNumber_ShouldThrowArgumentException(int invalidMaxCourses)
        {
            // Arrange
            var teacher = CreateValidTeacher();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => teacher.SetMaxCoursesPerSemester(invalidMaxCourses));
        }

        [Fact]
        public void SetMaxCoursesPerSemester_BelowCurrentLoad_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            teacher.AssignToCourse(Guid.NewGuid(), "Course 1");
            teacher.AssignToCourse(Guid.NewGuid(), "Course 2");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => teacher.SetMaxCoursesPerSemester(1));
        }

        [Fact]
        public void SetOfficeInfo_ValidData_ShouldSetSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var officeLocation = "Building A, Room 101";
            var officeHours = "Monday 10-12, Wednesday 2-4";

            // Act
            teacher.SetOfficeInfo(officeLocation, officeHours);

            // Assert
            Assert.Equal(officeLocation, teacher.OfficeLocation);
            Assert.Equal(officeHours, teacher.OfficeHours);
        }

        [Fact]
        public void Deactivate_ValidStatus_ShouldDeactivateSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            var newStatus = EmploymentStatus.Suspended;

            // Act
            teacher.Deactivate(newStatus);

            // Assert
            Assert.Equal(newStatus, teacher.Status);
            Assert.Contains(teacher.DomainEvents, e => e is TeacherDeactivatedEvent);
        }

        [Fact]
        public void Deactivate_ActiveStatus_ShouldThrowArgumentException()
        {
            // Arrange
            var teacher = CreateValidTeacher();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => teacher.Deactivate(EmploymentStatus.Active));
        }

        [Fact]
        public void Reactivate_ShouldReactivateSuccessfully()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            teacher.Deactivate(EmploymentStatus.Suspended);

            // Act
            teacher.Reactivate();

            // Assert
            Assert.Equal(EmploymentStatus.Active, teacher.Status);
            Assert.Contains(teacher.DomainEvents, e => e is TeacherReactivatedEvent);
        }

        [Fact]
        public void CanTakeMoreCourses_ActiveTeacherWithSpace_ShouldReturnTrue()
        {
            // Arrange
            var teacher = CreateValidTeacher();

            // Act
            var canTakeMore = teacher.CanTakeMoreCourses();

            // Assert
            Assert.True(canTakeMore);
        }

        [Fact]
        public void CanTakeMoreCourses_InactiveTeacher_ShouldReturnFalse()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            teacher.Deactivate(EmploymentStatus.Suspended);

            // Act
            var canTakeMore = teacher.CanTakeMoreCourses();

            // Assert
            Assert.False(canTakeMore);
        }

        [Fact]
        public void CanTakeMoreCourses_AtMaxCapacity_ShouldReturnFalse()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            
            // Assign maximum courses
            for (int i = 0; i < teacher.MaxCoursesPerSemester; i++)
            {
                teacher.AssignToCourse(Guid.NewGuid(), $"Course {i + 1}");
            }

            // Act
            var canTakeMore = teacher.CanTakeMoreCourses();

            // Assert
            Assert.False(canTakeMore);
        }

        [Fact]
        public void GetWorkloadPercentage_HalfLoad_ShouldReturn50Percent()
        {
            // Arrange
            var teacher = CreateValidTeacher(); // Assistant Professor, max 3 courses
            teacher.AssignToCourse(Guid.NewGuid(), "Course 1");

            // Act
            var percentage = teacher.GetWorkloadPercentage();

            // Assert
            Assert.Equal(33.33, percentage, 2); // 1/3 * 100 = 33.33%
        }

        [Fact]
        public void IsOverloaded_NormalLoad_ShouldReturnFalse()
        {
            // Arrange
            var teacher = CreateValidTeacher();
            teacher.AssignToCourse(Guid.NewGuid(), "Course 1");

            // Act
            var isOverloaded = teacher.IsOverloaded();

            // Assert
            Assert.False(isOverloaded);
        }

        [Fact]
        public void GetYearsOfService_HiredThisYear_ShouldReturnZero()
        {
            // Arrange
            var teacher = CreateValidTeacher();

            // Act
            var yearsOfService = teacher.GetYearsOfService();

            // Assert
            Assert.Equal(0, yearsOfService);
        }

        [Fact]
        public void IsSeniorTeacher_NewTeacher_ShouldReturnFalse()
        {
            // Arrange
            var teacher = CreateValidTeacher();

            // Act
            var isSenior = teacher.IsSeniorTeacher();

            // Assert
            Assert.False(isSenior);
        }

        [Fact]
        public void HasUniversityEmail_UniversityEmail_ShouldReturnTrue()
        {
            // Arrange
            var teacher = Teacher.Create(
                "Dr. Jane Smith", 
                "jane.smith@university.edu", 
                "EMP001", 
                "Computer Science", 
                AcademicTitle.AssistantProfessor, 
                new DateTime(1980, 5, 15));

            // Act
            var hasUniversityEmail = teacher.HasUniversityEmail();

            // Assert
            Assert.True(hasUniversityEmail);
        }

        private static Teacher CreateValidTeacher()
        {
            return Teacher.Create(
                "Dr. Jane Smith",
                "jane.smith@university.edu",
                "EMP001",
                "Computer Science",
                AcademicTitle.AssistantProfessor,
                new DateTime(1980, 5, 15));
        }
    }
}
