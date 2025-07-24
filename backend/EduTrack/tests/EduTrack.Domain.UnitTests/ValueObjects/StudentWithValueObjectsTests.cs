using EduTrack.Domain.Entities;
using EduTrack.Domain.ValueObjects;
using Xunit;

namespace EduTrack.Domain.UnitTests.ValueObjects
{
    public class StudentWithValueObjectsTests
    {
        [Fact]
        public void Create_WithValueObjects_ShouldCreateStudentWithCorrectProperties()
        {
            // Arrange
            var fullName = FullName.Create("John Doe");
            var email = Email.Create("john.doe@university.edu");
            var dateOfBirth = new DateTime(1995, 5, 15);

            // Act
            var student = Student.Create(fullName, dateOfBirth, email);

            // Assert
            Assert.NotNull(student);
            Assert.Equal("John Doe", student.FullName.Value);
            Assert.Equal("john.doe@university.edu", student.Email.Value);
            Assert.Equal(dateOfBirth, student.DateOfBirth);
            Assert.Equal(StudentStatus.Active, student.Status);
        }

        [Fact]
        public void Create_WithStringParameters_ShouldCreateStudentAndValidateValueObjects()
        {
            // Arrange
            var fullName = "Jane Smith";
            var email = "jane.smith@university.edu";
            var dateOfBirth = new DateTime(1994, 3, 20);

            // Act
            var student = Student.Create(fullName, dateOfBirth, email);

            // Assert
            Assert.NotNull(student);
            Assert.Equal("Jane Smith", student.FullName.Value);
            Assert.Equal("jane.smith@university.edu", student.Email.Value);
            Assert.True(student.Email.IsUniversityEmail());
            Assert.Equal("Smith", student.FullName.LastName);
            Assert.Equal("Jane", student.FullName.FirstName);
        }

        [Fact]
        public void UpdateContactInformation_WithValueObject_ShouldUpdateEmail()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@old.edu");
            var newEmail = Email.Create("john@new.edu");

            // Act
            student.UpdateContactInformation(newEmail);

            // Assert
            Assert.Equal("john@new.edu", student.Email.Value);
        }

        [Fact]
        public void UpdateFullName_WithValueObject_ShouldUpdateName()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@university.edu");
            var newName = FullName.Create("John Michael Doe");

            // Act
            student.UpdateFullName(newName);

            // Assert
            Assert.Equal("John Michael Doe", student.FullName.Value);
            Assert.Equal("Michael", student.FullName.MiddleName);
        }

        [Fact]
        public void UpdatePhoneNumber_WithValueObject_ShouldUpdatePhoneNumber()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@university.edu");
            var phoneNumber = PhoneNumber.Create("(555) 234-5678"); // Exchange code cannot start with 0 or 1

            // Act
            student.UpdatePhoneNumber(phoneNumber);

            // Assert
            Assert.NotNull(student.PhoneNumber);
            Assert.Equal("(555) 234-5678", student.PhoneNumber.Value);
            Assert.Equal("555", student.PhoneNumber.AreaCode);
        }

        [Fact]
        public void UpdateAddress_WithValueObject_ShouldUpdateAddress()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@university.edu");
            var address = Address.Create("123 Main St", "Springfield", "IL", "62701");

            // Act
            student.UpdateAddress(address);

            // Assert
            Assert.NotNull(student.Address);
            Assert.Equal("123 Main St", student.Address.Street);
            Assert.Equal("Springfield", student.Address.City);
            Assert.Equal("IL", student.Address.State);
            Assert.Equal("62701", student.Address.ZipCode);
        }

        [Fact]
        public void UpdateGPA_WithValueObject_ShouldUpdateGPA()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@university.edu");
            var gpa = GPA.Create(3.75m);

            // Act
            student.UpdateGPA(gpa);

            // Assert
            Assert.NotNull(student.CurrentGPA);
            Assert.Equal(3.75m, student.CurrentGPA.Value);
            Assert.True(student.CurrentGPA.IsHonorsLevel);
            Assert.Equal("A", student.CurrentGPA.LetterGrade); // 3.75 >= 3.7 should be "A"
        }

        [Fact]
        public void IsEligibleForHonors_WithHonorsGPA_ShouldReturnTrue()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@university.edu");
            student.UpdateGPA(3.8m);

            // Act
            var isEligible = student.IsEligibleForHonors();

            // Assert
            Assert.True(isEligible);
        }

        [Fact]
        public void IsOnAcademicProbation_WithLowGPA_ShouldReturnTrue()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@university.edu");
            student.UpdateGPA(1.5m);

            // Act
            var isOnProbation = student.IsOnAcademicProbation();

            // Assert
            Assert.True(isOnProbation);
        }

        [Fact]
        public void HasUniversityEmail_WithEduDomain_ShouldReturnTrue()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@university.edu");

            // Act
            var hasUniversityEmail = student.HasUniversityEmail();

            // Assert
            Assert.True(hasUniversityEmail);
        }

        [Fact]
        public void HasUniversityEmail_WithNonEduDomain_ShouldReturnFalse()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@gmail.com");

            // Act
            var hasUniversityEmail = student.HasUniversityEmail();

            // Assert
            Assert.False(hasUniversityEmail);
        }

        [Fact]
        public void Create_WithInvalidEmail_ShouldThrowArgumentException()
        {
            // Arrange
            var fullName = "John Doe";
            var invalidEmail = "invalid-email";
            var dateOfBirth = new DateTime(1995, 5, 15);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Student.Create(fullName, dateOfBirth, invalidEmail));
        }

        [Fact]
        public void Create_WithInvalidName_ShouldThrowArgumentException()
        {
            // Arrange
            var invalidName = "John"; // Single name should fail
            var email = "john@university.edu";
            var dateOfBirth = new DateTime(1995, 5, 15);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Student.Create(invalidName, dateOfBirth, email));
        }

        [Fact]
        public void UpdatePhoneNumber_WithInvalidNumber_ShouldThrowArgumentException()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@university.edu");
            var invalidPhoneNumber = "123"; // Too short

            // Act & Assert
            Assert.Throws<ArgumentException>(() => student.UpdatePhoneNumber(invalidPhoneNumber));
        }

        [Fact]
        public void UpdateGPA_WithInvalidGPA_ShouldThrowArgumentException()
        {
            // Arrange
            var student = Student.Create("John Doe", new DateTime(1995, 5, 15), "john@university.edu");
            var invalidGPA = 5.0m; // Out of range

            // Act & Assert
            Assert.Throws<ArgumentException>(() => student.UpdateGPA(invalidGPA));
        }
    }
}
