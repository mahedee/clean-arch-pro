using EduTrack.Domain.ValueObjects;
using Xunit;

namespace EduTrack.Domain.UnitTests.ValueObjects
{
    public class FullNameTests
    {
        [Fact]
        public void Create_WithValidFullName_ShouldReturnFullNameInstance()
        {
            // Arrange
            var nameString = "John Doe";

            // Act
            var fullName = FullName.Create(nameString);

            // Assert
            Assert.NotNull(fullName);
            Assert.Equal("John Doe", fullName.Value);
        }

        [Fact]
        public void Create_WithValidFullName_ShouldNormalizeToProperCase()
        {
            // Arrange
            var nameString = "john doe";

            // Act
            var fullName = FullName.Create(nameString);

            // Assert
            Assert.Equal("John Doe", fullName.Value);
        }

        [Fact]
        public void Create_WithExtraSpaces_ShouldNormalizeSpaces()
        {
            // Arrange
            var nameString = "  John   Doe  ";

            // Act
            var fullName = FullName.Create(nameString);

            // Assert
            Assert.Equal("John Doe", fullName.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullName_ShouldThrowArgumentException(string invalidName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => FullName.Create(invalidName));
        }

        [Fact]
        public void Create_WithSingleName_ShouldThrowArgumentException()
        {
            // Arrange
            var singleName = "John";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FullName.Create(singleName));
        }

        [Fact]
        public void Create_WithTooShortName_ShouldThrowArgumentException()
        {
            // Arrange
            var shortName = "A";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FullName.Create(shortName));
        }

        [Fact]
        public void Create_WithTooLongName_ShouldThrowArgumentException()
        {
            // Arrange
            var longName = new string('A', 101);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => FullName.Create(longName));
        }

        [Theory]
        [InlineData("John123 Doe")]
        [InlineData("John@ Doe")]
        [InlineData("John# Doe")]
        public void Create_WithInvalidCharacters_ShouldThrowArgumentException(string invalidName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => FullName.Create(invalidName));
        }

        [Fact]
        public void CreateFromParts_WithValidParts_ShouldReturnFullNameInstance()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";

            // Act
            var fullName = FullName.Create(firstName, lastName);

            // Assert
            Assert.Equal("John Doe", fullName.Value);
        }

        [Fact]
        public void CreateFromParts_WithMiddleName_ShouldIncludeMiddleName()
        {
            // Arrange
            var firstName = "John";
            var middleName = "Michael";
            var lastName = "Doe";

            // Act
            var fullName = FullName.Create(firstName, lastName, middleName);

            // Assert
            Assert.Equal("John Michael Doe", fullName.Value);
        }

        [Fact]
        public void FirstName_ShouldReturnFirstPart()
        {
            // Arrange
            var fullName = FullName.Create("John Michael Doe");

            // Act
            var firstName = fullName.FirstName;

            // Assert
            Assert.Equal("John", firstName);
        }

        [Fact]
        public void LastName_ShouldReturnLastPart()
        {
            // Arrange
            var fullName = FullName.Create("John Michael Doe");

            // Act
            var lastName = fullName.LastName;

            // Assert
            Assert.Equal("Doe", lastName);
        }

        [Fact]
        public void MiddleName_WithMiddleName_ShouldReturnMiddlePart()
        {
            // Arrange
            var fullName = FullName.Create("John Michael Doe");

            // Act
            var middleName = fullName.MiddleName;

            // Assert
            Assert.Equal("Michael", middleName);
        }

        [Fact]
        public void MiddleName_WithoutMiddleName_ShouldReturnNull()
        {
            // Arrange
            var fullName = FullName.Create("John Doe");

            // Act
            var middleName = fullName.MiddleName;

            // Assert
            Assert.Null(middleName);
        }

        [Fact]
        public void MiddleName_WithMultipleMiddleNames_ShouldReturnAllMiddleParts()
        {
            // Arrange
            var fullName = FullName.Create("John Michael James Doe");

            // Act
            var middleName = fullName.MiddleName;

            // Assert
            Assert.Equal("Michael James", middleName);
        }

        [Fact]
        public void Initials_ShouldReturnCorrectInitials()
        {
            // Arrange
            var fullName = FullName.Create("John Michael Doe");

            // Act
            var initials = fullName.Initials;

            // Assert
            Assert.Equal("J.M.D.", initials);
        }

        [Fact]
        public void GetDisplayName_ShouldReturnLastFirstFormat()
        {
            // Arrange
            var fullName = FullName.Create("John Doe");

            // Act
            var displayName = fullName.GetDisplayName();

            // Assert
            Assert.Equal("Doe, John", displayName);
        }

        [Fact]
        public void GetFormalName_WithTitle_ShouldIncludeTitle()
        {
            // Arrange
            var fullName = FullName.Create("John Doe");

            // Act
            var formalName = fullName.GetFormalName("Dr.");

            // Assert
            Assert.Equal("Dr. John Doe", formalName);
        }

        [Fact]
        public void GetFormalName_WithoutTitle_ShouldReturnOriginalName()
        {
            // Arrange
            var fullName = FullName.Create("John Doe");

            // Act
            var formalName = fullName.GetFormalName();

            // Assert
            Assert.Equal("John Doe", formalName);
        }

        [Fact]
        public void Contains_WithExistingPart_ShouldReturnTrue()
        {
            // Arrange
            var fullName = FullName.Create("John Michael Doe");

            // Act & Assert
            Assert.True(fullName.Contains("John"));
            Assert.True(fullName.Contains("Michael"));
            Assert.True(fullName.Contains("Doe"));
        }

        [Fact]
        public void Contains_WithNonExistingPart_ShouldReturnFalse()
        {
            // Arrange
            var fullName = FullName.Create("John Doe");

            // Act & Assert
            Assert.False(fullName.Contains("Smith"));
        }

        [Fact]
        public void Equals_WithSameName_ShouldReturnTrue()
        {
            // Arrange
            var name1 = FullName.Create("John Doe");
            var name2 = FullName.Create("john doe");

            // Act & Assert
            Assert.True(name1.Equals(name2));
            Assert.True(name1 == name2);
        }

        [Fact]
        public void Equals_WithDifferentNames_ShouldReturnFalse()
        {
            // Arrange
            var name1 = FullName.Create("John Doe");
            var name2 = FullName.Create("Jane Smith");

            // Act & Assert
            Assert.False(name1.Equals(name2));
            Assert.True(name1 != name2);
        }

        [Fact]
        public void GetHashCode_WithSameName_ShouldReturnSameHashCode()
        {
            // Arrange
            var name1 = FullName.Create("John Doe");
            var name2 = FullName.Create("john doe");

            // Act & Assert
            Assert.Equal(name1.GetHashCode(), name2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnNameValue()
        {
            // Arrange
            var fullName = FullName.Create("John Doe");

            // Act
            var result = fullName.ToString();

            // Assert
            Assert.Equal("John Doe", result);
        }

        [Fact]
        public void ImplicitConversion_ShouldConvertToString()
        {
            // Arrange
            var fullName = FullName.Create("John Doe");

            // Act
            string nameString = fullName;

            // Assert
            Assert.Equal("John Doe", nameString);
        }
    }
}
