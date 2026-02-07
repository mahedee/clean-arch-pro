using EduTrack.Domain.Entities;
using EduTrack.Domain.Events;
using Xunit;

namespace EduTrack.Domain.UnitTests.Entities
{
    public class StudentTests
    {
        [Fact]
        public void Create_ValidData_ShouldCreateStudentWithCorrectProperties()
        {
            // Arrange
            var fullName = "John Doe";
            var dateOfBirth = new DateTime(2000, 1, 1);
            var email = "john.doe@example.com";

            // Act
            var student = Student.Create(fullName, dateOfBirth, email);

            // Assert
            Assert.NotEqual(Guid.Empty, student.Id);
            Assert.Equal(fullName, student.FullName);
            Assert.Equal(dateOfBirth, student.DateOfBirth);
            Assert.Equal(email, student.Email);
            Assert.Equal(StudentStatus.Active, student.Status);
            Assert.True(student.EnrollmentDate <= DateTime.UtcNow);
        }

        [Fact]
        public void Create_ValidData_ShouldRaiseStudentCreatedEvent()
        {
            // Arrange
            var fullName = "John Doe";
            var dateOfBirth = new DateTime(2000, 1, 1);
            var email = "john.doe@example.com";

            // Act
            var student = Student.Create(fullName, dateOfBirth, email);

            // Assert
            Assert.Single(student.DomainEvents);
            var domainEvent = student.DomainEvents.First();
            Assert.IsType<StudentCreatedEvent>(domainEvent);
            
            var studentCreatedEvent = (StudentCreatedEvent)domainEvent;
            Assert.Equal(student.Id, studentCreatedEvent.StudentId);
            Assert.Equal(fullName, studentCreatedEvent.FullName);
            Assert.Equal(email, studentCreatedEvent.Email);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidFullName_ShouldThrowArgumentException(string invalidFullName)
        {
            // Arrange
            var dateOfBirth = new DateTime(2000, 1, 1);
            var email = "john.doe@example.com";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Student.Create(invalidFullName, dateOfBirth, email));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Arrange
            var fullName = "John Doe";
            var dateOfBirth = new DateTime(2000, 1, 1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Student.Create(fullName, dateOfBirth, invalidEmail));
        }

        [Fact]
        public void Create_FutureDateOfBirth_ShouldThrowArgumentException()
        {
            // Arrange
            var fullName = "John Doe";
            var futureDate = DateTime.Today.AddDays(1);
            var email = "john.doe@example.com";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Student.Create(fullName, futureDate, email));
        }

        [Fact]
        public void UpdateContactInformation_ValidEmail_ShouldUpdateAndRaiseDomainEvent()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(2000, 1, 1), "john@example.com");
            var newEmail = "john.doe@example.com";
            var originalEventCount = student.DomainEvents.Count;

            // Act
            student.UpdateContactInformation(newEmail);

            // Assert
            Assert.Equal(newEmail, student.Email);
            Assert.NotNull(student.UpdatedAt);
            Assert.Equal(originalEventCount + 1, student.DomainEvents.Count);
            
            var contactUpdatedEvent = student.DomainEvents.OfType<StudentContactUpdatedEvent>().FirstOrDefault();
            Assert.NotNull(contactUpdatedEvent);
            Assert.Equal(student.Id, contactUpdatedEvent.StudentId);
            Assert.Equal(newEmail, contactUpdatedEvent.NewEmail);
            Assert.Equal("john@example.com", contactUpdatedEvent.PreviousEmail);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateContactInformation_InvalidEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(2000, 1, 1), "john@example.com");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => student.UpdateContactInformation(invalidEmail));
        }

        [Fact]
        public void UpdateFullName_ValidName_ShouldUpdateName()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(2000, 1, 1), "john@example.com");
            var newName = "John Smith";

            // Act
            student.UpdateFullName(newName);

            // Assert
            Assert.Equal(newName, student.FullName);
            Assert.NotNull(student.UpdatedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateFullName_InvalidName_ShouldThrowArgumentException(string invalidName)
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(2000, 1, 1), "john@example.com");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => student.UpdateFullName(invalidName));
        }

        [Fact]
        public void Deactivate_ShouldSetStatusToInactive()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(2000, 1, 1), "john@example.com");

            // Act
            student.Deactivate();

            // Assert
            Assert.Equal(StudentStatus.Inactive, student.Status);
            Assert.NotNull(student.UpdatedAt);
        }

        [Fact]
        public void Reactivate_ShouldSetStatusToActive()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(2000, 1, 1), "john@example.com");
            student.Deactivate();

            // Act
            student.Reactivate();

            // Assert
            Assert.Equal(StudentStatus.Active, student.Status);
            Assert.NotNull(student.UpdatedAt);
        }

        [Fact]
        public void Graduate_ShouldSetStatusToGraduated()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(2000, 1, 1), "john@example.com");

            // Act
            student.Graduate();

            // Assert
            Assert.Equal(StudentStatus.Graduated, student.Status);
            Assert.NotNull(student.UpdatedAt);
        }
    }
}
