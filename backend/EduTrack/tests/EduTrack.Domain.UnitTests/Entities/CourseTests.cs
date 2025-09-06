using EduTrack.Domain.Entities;
using EduTrack.Domain.Events;
using Xunit;

namespace EduTrack.Domain.UnitTests.Entities
{
    public class CourseTests
    {
        [Fact]
        public void Create_ValidData_ShouldCreateCourseWithCorrectProperties()
        {
            // Arrange
            var title = "Introduction to Computer Science";
            var code = "CS101";
            var description = "A foundational course in computer science covering programming fundamentals.";
            var creditHours = 3;
            var level = CourseLevel.Undergraduate;
            var department = "Computer Science";
            var maxEnrollment = 30;

            // Act
            var course = Course.Create(title, code, description, creditHours, level, department, maxEnrollment);

            // Assert
            Assert.NotEqual(Guid.Empty, course.Id);
            Assert.Equal(title, course.Title);
            Assert.Equal(code.ToUpperInvariant(), course.Code);
            Assert.Equal(description, course.Description);
            Assert.Equal(creditHours, course.CreditHours);
            Assert.Equal(level, course.Level);
            Assert.Equal(department, course.Department);
            Assert.Equal(maxEnrollment, course.MaxEnrollment);
            Assert.Equal(0, course.CurrentEnrollment);
            Assert.Equal(CourseStatus.Draft, course.Status);
        }

        [Fact]
        public void Create_ValidData_ShouldRaiseCourseCreatedEvent()
        {
            // Arrange
            var title = "Introduction to Computer Science";
            var code = "CS101";
            var description = "A foundational course in computer science covering programming fundamentals.";
            var creditHours = 3;
            var level = CourseLevel.Undergraduate;
            var department = "Computer Science";

            // Act
            var course = Course.Create(title, code, description, creditHours, level, department);

            // Assert
            Assert.Single(course.DomainEvents);
            var domainEvent = course.DomainEvents.First();
            Assert.IsType<CourseCreatedEvent>(domainEvent);
            
            var courseCreatedEvent = (CourseCreatedEvent)domainEvent;
            Assert.Equal(course.Id, courseCreatedEvent.CourseId);
            Assert.Equal(title, courseCreatedEvent.Title);
            Assert.Equal(code.ToUpperInvariant(), courseCreatedEvent.Code);
            Assert.Equal(department, courseCreatedEvent.Department);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Create_InvalidTitle_ShouldThrowArgumentException(string invalidTitle)
        {
            // Arrange
            var code = "CS101";
            var description = "A foundational course in computer science covering programming fundamentals.";
            var creditHours = 3;
            var level = CourseLevel.Undergraduate;
            var department = "Computer Science";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Course.Create(invalidTitle, code, description, creditHours, level, department));
        }

        [Theory]
        [InlineData("AB")]
        [InlineData("A")]
        public void Create_TitleTooShort_ShouldThrowArgumentException(string shortTitle)
        {
            // Arrange
            var code = "CS101";
            var description = "A foundational course in computer science covering programming fundamentals.";
            var creditHours = 3;
            var level = CourseLevel.Undergraduate;
            var department = "Computer Science";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Course.Create(shortTitle, code, description, creditHours, level, department));
        }

        [Fact]
        public void Create_TitleTooLong_ShouldThrowArgumentException()
        {
            // Arrange
            var longTitle = new string('A', 101); // 101 characters
            var code = "CS101";
            var description = "A foundational course in computer science covering programming fundamentals.";
            var creditHours = 3;
            var level = CourseLevel.Undergraduate;
            var department = "Computer Science";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Course.Create(longTitle, code, description, creditHours, level, department));
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        [InlineData("AB")]
        [InlineData("A")]
        public void Create_InvalidCode_ShouldThrowArgumentException(string invalidCode)
        {
            // Arrange
            var title = "Introduction to Computer Science";
            var description = "A foundational course in computer science covering programming fundamentals.";
            var creditHours = 3;
            var level = CourseLevel.Undergraduate;
            var department = "Computer Science";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Course.Create(title, invalidCode, description, creditHours, level, department));
        }

        [Fact]
        public void Create_CodeWithSpecialCharacters_ShouldThrowArgumentException()
        {
            // Arrange
            var title = "Introduction to Computer Science";
            var codeWithSpecialChars = "CS-101";
            var description = "A foundational course in computer science covering programming fundamentals.";
            var creditHours = 3;
            var level = CourseLevel.Undergraduate;
            var department = "Computer Science";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Course.Create(title, codeWithSpecialChars, description, creditHours, level, department));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(13)]
        public void Create_InvalidCreditHours_ShouldThrowArgumentException(int invalidCreditHours)
        {
            // Arrange
            var title = "Introduction to Computer Science";
            var code = "CS101";
            var description = "A foundational course in computer science covering programming fundamentals.";
            var level = CourseLevel.Undergraduate;
            var department = "Computer Science";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Course.Create(title, code, description, invalidCreditHours, level, department));
        }

        [Fact]
        public void Schedule_ValidData_ShouldScheduleCourseSuccessfully()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            var semester = "Fall";
            var academicYear = 2024;
            var startDate = DateTime.Today.AddMonths(1);
            var endDate = startDate.AddMonths(4);

            // Act
            course.Schedule(semester, academicYear, startDate, endDate);

            // Assert
            Assert.Equal(semester, course.Semester);
            Assert.Equal(academicYear, course.AcademicYear);
            Assert.Equal(startDate, course.StartDate);
            Assert.Equal(endDate, course.EndDate);
            Assert.Equal(CourseStatus.Scheduled, course.Status);
            Assert.Contains(course.DomainEvents, e => e is CourseScheduledEvent);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Schedule_InvalidSemester_ShouldThrowArgumentException(string invalidSemester)
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            var academicYear = 2024;
            var startDate = DateTime.Today.AddMonths(1);
            var endDate = startDate.AddMonths(4);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                course.Schedule(invalidSemester, academicYear, startDate, endDate));
        }

        [Fact]
        public void Schedule_StartDateInPast_ShouldThrowArgumentException()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            var semester = "Fall";
            var academicYear = 2024;
            var startDate = DateTime.Today.AddDays(-1);
            var endDate = DateTime.Today.AddMonths(4);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                course.Schedule(semester, academicYear, startDate, endDate));
        }

        [Fact]
        public void Schedule_StartDateAfterEndDate_ShouldThrowArgumentException()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            var semester = "Fall";
            var academicYear = 2024;
            var startDate = DateTime.Today.AddMonths(2);
            var endDate = DateTime.Today.AddMonths(1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                course.Schedule(semester, academicYear, startDate, endDate));
        }

        [Fact]
        public void Activate_ScheduledCourse_ShouldActivateSuccessfully()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            course.Schedule("Fall", 2024, DateTime.Today.AddMonths(1), DateTime.Today.AddMonths(5));

            // Act
            course.Activate();

            // Assert
            Assert.Equal(CourseStatus.Active, course.Status);
            Assert.Contains(course.DomainEvents, e => e is CourseActivatedEvent);
        }

        [Fact]
        public void Activate_DraftCourse_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => course.Activate());
        }

        [Fact]
        public void Cancel_WithReason_ShouldCancelCourseSuccessfully()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            var reason = "Low enrollment";

            // Act
            course.Cancel(reason);

            // Assert
            Assert.Equal(CourseStatus.Cancelled, course.Status);
            Assert.Contains(course.DomainEvents, e => e is CourseCancelledEvent cancelledEvent && cancelledEvent.Reason == reason);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Cancel_InvalidReason_ShouldThrowArgumentException(string invalidReason)
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => course.Cancel(invalidReason));
        }

        [Fact]
        public void EnrollStudent_ActiveCourseWithSpace_ShouldEnrollSuccessfully()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS", 30);
            course.Schedule("Fall", 2024, DateTime.Today.AddDays(10), DateTime.Today.AddMonths(4));
            course.Activate();

            // Act
            course.EnrollStudent();

            // Assert
            Assert.Equal(1, course.CurrentEnrollment);
            Assert.Contains(course.DomainEvents, e => e is StudentEnrolledInCourseEvent);
        }

        [Fact]
        public void EnrollStudent_InactiveCourse_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => course.EnrollStudent());
        }

        [Fact]
        public void EnrollStudent_FullCourse_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS", 1);
            course.Schedule("Fall", 2024, DateTime.Today.AddDays(10), DateTime.Today.AddMonths(4));
            course.Activate();
            course.EnrollStudent(); // Fill the course

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => course.EnrollStudent());
        }

        [Fact]
        public void WithdrawStudent_WithEnrolledStudents_ShouldWithdrawSuccessfully()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            course.Schedule("Fall", 2024, DateTime.Today.AddDays(10), DateTime.Today.AddMonths(4));
            course.Activate();
            course.EnrollStudent();

            // Act
            course.WithdrawStudent();

            // Assert
            Assert.Equal(0, course.CurrentEnrollment);
            Assert.Contains(course.DomainEvents, e => e is StudentWithdrewFromCourseEvent);
        }

        [Fact]
        public void WithdrawStudent_NoEnrolledStudents_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => course.WithdrawStudent());
        }

        [Fact]
        public void HasAvailableSpots_WithSpace_ShouldReturnTrue()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS", 30);

            // Act
            var hasSpace = course.HasAvailableSpots();

            // Assert
            Assert.True(hasSpace);
        }

        [Fact]
        public void HasAvailableSpots_FullCourse_ShouldReturnFalse()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS", 1);
            course.Schedule("Fall", 2024, DateTime.Today.AddDays(10), DateTime.Today.AddMonths(4));
            course.Activate();
            course.EnrollStudent();

            // Act
            var hasSpace = course.HasAvailableSpots();

            // Assert
            Assert.False(hasSpace);
        }

        [Fact]
        public void GetEnrollmentPercentage_HalfFull_ShouldReturn50Percent()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS", 10);
            course.Schedule("Fall", 2024, DateTime.Today.AddDays(10), DateTime.Today.AddMonths(4));
            course.Activate();
            
            // Enroll 5 students
            for (int i = 0; i < 5; i++)
            {
                course.EnrollStudent();
            }

            // Act
            var percentage = course.GetEnrollmentPercentage();

            // Assert
            Assert.Equal(50.0, percentage);
        }

        [Fact]
        public void IsFull_FullCourse_ShouldReturnTrue()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS", 2);
            course.Schedule("Fall", 2024, DateTime.Today.AddDays(10), DateTime.Today.AddMonths(4));
            course.Activate();
            course.EnrollStudent();
            course.EnrollStudent();

            // Act
            var isFull = course.IsFull();

            // Assert
            Assert.True(isFull);
        }

        [Fact]
        public void UpdateCourseInfo_ValidData_ShouldUpdateSuccessfully()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            var newTitle = "Advanced Computer Science";
            var newDescription = "An advanced course in computer science covering advanced topics.";
            var newMaxEnrollment = 25;

            // Act
            course.UpdateCourseInfo(newTitle, newDescription, newMaxEnrollment);

            // Assert
            Assert.Equal(newTitle, course.Title);
            Assert.Equal(newDescription, course.Description);
            Assert.Equal(newMaxEnrollment, course.MaxEnrollment);
            Assert.Contains(course.DomainEvents, e => e is CourseUpdatedEvent);
        }

        [Fact]
        public void UpdateCourseInfo_ActiveCourseWithStudents_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            course.Schedule("Fall", 2024, DateTime.Today.AddDays(10), DateTime.Today.AddMonths(4));
            course.Activate();
            course.EnrollStudent();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                course.UpdateCourseInfo("New Title", "New Description", 25));
        }

        [Fact]
        public void SetPrerequisiteRequirement_ValidCreditHours_ShouldSetSuccessfully()
        {
            // Arrange
            var course = Course.Create("CS201", "CS201", "Data Structures", 3, CourseLevel.Undergraduate, "CS");
            var prerequisiteCreditHours = 15;

            // Act
            course.SetPrerequisiteRequirement(prerequisiteCreditHours);

            // Assert
            Assert.Equal(prerequisiteCreditHours, course.PrerequisiteCreditHours);
        }

        [Fact]
        public void SetPrerequisiteRequirement_NegativeCreditHours_ShouldThrowArgumentException()
        {
            // Arrange
            var course = Course.Create("CS201", "CS201", "Data Structures", 3, CourseLevel.Undergraduate, "CS");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => course.SetPrerequisiteRequirement(-1));
        }

        [Fact]
        public void Complete_ActiveCourseAfterEndDate_ShouldCompleteSuccessfully()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            var startDate = DateTime.Today.AddDays(1); // Start tomorrow
            var endDate = DateTime.Today.AddDays(2); // End day after tomorrow
            course.Schedule("Fall", 2024, startDate, endDate);
            course.Activate();

            // Act
            course.Complete();

            // Assert
            Assert.Equal(CourseStatus.Completed, course.Status);
            Assert.Contains(course.DomainEvents, e => e is CourseCompletedEvent);
        }

        [Fact]
        public void Complete_InactiveCourse_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => course.Complete());
        }

        [Fact]
        public void GetCourseDurationDays_ValidDates_ShouldReturnCorrectDuration()
        {
            // Arrange
            var course = Course.Create("CS101", "CS101", "Test Course", 3, CourseLevel.Undergraduate, "CS");
            var startDate = DateTime.Today.AddDays(1); // Tomorrow
            var endDate = DateTime.Today.AddDays(106); // 105 days later
            course.Schedule("Fall", 2024, startDate, endDate);

            // Act
            var duration = course.GetCourseDurationDays();

            // Assert
            Assert.Equal(105, duration); // 105 days difference
        }
    }
}
