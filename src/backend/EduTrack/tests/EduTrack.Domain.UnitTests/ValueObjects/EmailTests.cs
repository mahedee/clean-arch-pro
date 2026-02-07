using EduTrack.Domain.ValueObjects;
using Xunit;

namespace EduTrack.Domain.UnitTests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Create_WithValidEmail_ShouldReturnEmailInstance()
        {
            // Arrange
            var emailString = "test@example.com";

            // Act
            var email = Email.Create(emailString);

            // Assert
            Assert.NotNull(email);
            Assert.Equal("test@example.com", email.Value);
        }

        [Fact]
        public void Create_WithValidEmail_ShouldNormalizeToLowerCase()
        {
            // Arrange
            var emailString = "TEST@EXAMPLE.COM";

            // Act
            var email = Email.Create(emailString);

            // Assert
            Assert.Equal("test@example.com", email.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        [InlineData("test.example.com")]
        [InlineData("test @example.com")]
        public void Create_WithInvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
        }

        [Fact]
        public void Create_WithEmailTooLong_ShouldThrowArgumentException()
        {
            // Arrange
            var longEmail = new string('a', 250) + "@example.com";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Email.Create(longEmail));
        }

        [Fact]
        public void Domain_ShouldReturnCorrectDomain()
        {
            // Arrange
            var email = Email.Create("user@example.com");

            // Act
            var domain = email.Domain;

            // Assert
            Assert.Equal("example.com", domain);
        }

        [Fact]
        public void LocalPart_ShouldReturnCorrectLocalPart()
        {
            // Arrange
            var email = Email.Create("user@example.com");

            // Act
            var localPart = email.LocalPart;

            // Assert
            Assert.Equal("user", localPart);
        }

        [Fact]
        public void IsUniversityEmail_WithEduDomain_ShouldReturnTrue()
        {
            // Arrange
            var email = Email.Create("student@university.edu");

            // Act
            var isUniversity = email.IsUniversityEmail();

            // Assert
            Assert.True(isUniversity);
        }

        [Fact]
        public void IsUniversityEmail_WithNonEduDomain_ShouldReturnFalse()
        {
            // Arrange
            var email = Email.Create("user@gmail.com");

            // Act
            var isUniversity = email.IsUniversityEmail();

            // Assert
            Assert.False(isUniversity);
        }

        [Fact]
        public void IsCorporateEmail_WithCorporateDomain_ShouldReturnTrue()
        {
            // Arrange
            var email = Email.Create("employee@company.com");

            // Act
            var isCorporate = email.IsCorporateEmail();

            // Assert
            Assert.True(isCorporate);
        }

        [Theory]
        [InlineData("user@gmail.com")]
        [InlineData("user@yahoo.com")]
        [InlineData("user@hotmail.com")]
        public void IsCorporateEmail_WithFreeEmailProvider_ShouldReturnFalse(string freeEmail)
        {
            // Arrange
            var email = Email.Create(freeEmail);

            // Act
            var isCorporate = email.IsCorporateEmail();

            // Assert
            Assert.False(isCorporate);
        }

        [Fact]
        public void WithDomain_ShouldCreateNewEmailWithNewDomain()
        {
            // Arrange
            var originalEmail = Email.Create("user@old.com");

            // Act
            var newEmail = originalEmail.WithDomain("new.com");

            // Assert
            Assert.Equal("user@new.com", newEmail.Value);
            Assert.Equal("user@old.com", originalEmail.Value); // Original unchanged
        }

        [Fact]
        public void Equals_WithSameEmail_ShouldReturnTrue()
        {
            // Arrange
            var email1 = Email.Create("test@example.com");
            var email2 = Email.Create("TEST@EXAMPLE.COM");

            // Act & Assert
            Assert.True(email1.Equals(email2));
            Assert.True(email1 == email2);
        }

        [Fact]
        public void Equals_WithDifferentEmails_ShouldReturnFalse()
        {
            // Arrange
            var email1 = Email.Create("test1@example.com");
            var email2 = Email.Create("test2@example.com");

            // Act & Assert
            Assert.False(email1.Equals(email2));
            Assert.True(email1 != email2);
        }

        [Fact]
        public void GetHashCode_WithSameEmail_ShouldReturnSameHashCode()
        {
            // Arrange
            var email1 = Email.Create("test@example.com");
            var email2 = Email.Create("TEST@EXAMPLE.COM");

            // Act & Assert
            Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnEmailValue()
        {
            // Arrange
            var email = Email.Create("test@example.com");

            // Act
            var result = email.ToString();

            // Assert
            Assert.Equal("test@example.com", result);
        }

        [Fact]
        public void ImplicitConversion_ShouldConvertToString()
        {
            // Arrange
            var email = Email.Create("test@example.com");

            // Act
            string emailString = email;

            // Assert
            Assert.Equal("test@example.com", emailString);
        }
    }
}
