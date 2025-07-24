using EduTrack.Domain.ValueObjects;
using Xunit;

namespace EduTrack.Domain.UnitTests.ValueObjects
{
    public class GPATests
    {
        [Theory]
        [InlineData(0.0)]
        [InlineData(2.5)]
        [InlineData(4.0)]
        public void Create_WithValidGPA_ShouldReturnGPAInstance(decimal validGPA)
        {
            // Act
            var gpa = GPA.Create(validGPA);

            // Assert
            Assert.NotNull(gpa);
            Assert.Equal(validGPA, gpa.Value);
        }

        [Theory]
        [InlineData(-0.1)]
        [InlineData(4.1)]
        [InlineData(5.0)]
        public void Create_WithInvalidGPA_ShouldThrowArgumentException(decimal invalidGPA)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => GPA.Create(invalidGPA));
        }

        [Fact]
        public void Create_ShouldRoundToTwoDecimalPlaces()
        {
            // Arrange
            var gpaValue = 3.567m;

            // Act
            var gpa = GPA.Create(gpaValue);

            // Assert
            Assert.Equal(3.57m, gpa.Value);
        }

        [Theory]
        [InlineData(60, 0.0)]
        [InlineData(70, 1.0)]
        [InlineData(80, 2.0)]
        [InlineData(90, 3.0)]
        [InlineData(100, 4.0)]
        public void FromPercentage_WithValidPercentage_ShouldConvertCorrectly(decimal percentage, decimal expectedGPA)
        {
            // Act
            var gpa = GPA.FromPercentage(percentage);

            // Assert
            Assert.Equal(expectedGPA, gpa.Value);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void FromPercentage_WithInvalidPercentage_ShouldThrowArgumentException(decimal invalidPercentage)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => GPA.FromPercentage(invalidPercentage));
        }

        [Theory]
        [InlineData(3.5, true)]
        [InlineData(3.7, true)]
        [InlineData(3.4, false)]
        [InlineData(2.0, false)]
        public void IsHonorsLevel_ShouldReturnCorrectResult(decimal gpaValue, bool expectedResult)
        {
            // Arrange
            var gpa = GPA.Create(gpaValue);

            // Act
            var result = gpa.IsHonorsLevel;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(2.0, true)]
        [InlineData(2.5, true)]
        [InlineData(1.9, false)]
        [InlineData(1.0, false)]
        public void IsPassingGrade_ShouldReturnCorrectResult(decimal gpaValue, bool expectedResult)
        {
            // Arrange
            var gpa = GPA.Create(gpaValue);

            // Act
            var result = gpa.IsPassingGrade;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(3.7, true)]
        [InlineData(3.8, true)]
        [InlineData(3.6, false)]
        [InlineData(3.0, false)]
        public void IsDeansListEligible_ShouldReturnCorrectResult(decimal gpaValue, bool expectedResult)
        {
            // Arrange
            var gpa = GPA.Create(gpaValue);

            // Act
            var result = gpa.IsDeansListEligible;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(1.9, true)]
        [InlineData(1.0, true)]
        [InlineData(2.0, false)]
        [InlineData(3.0, false)]
        public void IsOnProbation_ShouldReturnCorrectResult(decimal gpaValue, bool expectedResult)
        {
            // Arrange
            var gpa = GPA.Create(gpaValue);

            // Act
            var result = gpa.IsOnProbation;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(3.8, "A")]
        [InlineData(3.5, "A-")]
        [InlineData(3.1, "B+")]
        [InlineData(2.8, "B")]
        [InlineData(2.4, "B-")]
        [InlineData(2.1, "C+")]
        [InlineData(1.8, "C")]
        [InlineData(1.4, "C-")]
        [InlineData(1.2, "D")]
        [InlineData(0.5, "F")]
        public void LetterGrade_ShouldReturnCorrectGrade(decimal gpaValue, string expectedGrade)
        {
            // Arrange
            var gpa = GPA.Create(gpaValue);

            // Act
            var letterGrade = gpa.LetterGrade;

            // Assert
            Assert.Equal(expectedGrade, letterGrade);
        }

        [Theory]
        [InlineData(3.8, "Summa Cum Laude")]
        [InlineData(3.6, "Magna Cum Laude")]
        [InlineData(3.4, "Cum Laude")]
        [InlineData(3.1, "Good Standing")]
        [InlineData(2.5, "Satisfactory")]
        [InlineData(1.5, "Academic Probation")]
        public void AcademicStanding_ShouldReturnCorrectStanding(decimal gpaValue, string expectedStanding)
        {
            // Arrange
            var gpa = GPA.Create(gpaValue);

            // Act
            var standing = gpa.AcademicStanding;

            // Assert
            Assert.Equal(expectedStanding, standing);
        }

        [Fact]
        public void CalculateWeightedGPA_ShouldCalculateCorrectly()
        {
            // Arrange
            var gpa1 = GPA.Create(3.0m);
            var gpa2 = GPA.Create(4.0m);
            var credits1 = 30;
            var credits2 = 15;

            // Act
            var weightedGPA = gpa1.CalculateWeightedGPA(gpa2, credits1, credits2);

            // Assert
            var expectedValue = (3.0m * 30 + 4.0m * 15) / 45; // 3.33
            Assert.Equal(Math.Round(expectedValue, 2), weightedGPA.Value);
        }

        [Fact]
        public void ApplyBonus_ShouldAddBonusToGPA()
        {
            // Arrange
            var gpa = GPA.Create(3.5m);
            var bonus = 0.2m;

            // Act
            var bonusGPA = gpa.ApplyBonus(bonus);

            // Assert
            Assert.Equal(3.7m, bonusGPA.Value);
        }

        [Fact]
        public void ApplyBonus_ShouldNotExceedMaximum()
        {
            // Arrange
            var gpa = GPA.Create(3.9m);
            var bonus = 0.5m;

            // Act
            var bonusGPA = gpa.ApplyBonus(bonus);

            // Assert
            Assert.Equal(4.0m, bonusGPA.Value);
        }

        [Theory]
        [InlineData(3.5, 3.0, true)]
        [InlineData(3.0, 3.0, true)]
        [InlineData(2.5, 3.0, false)]
        public void MeetsRequirement_ShouldReturnCorrectResult(decimal actualGPA, decimal requiredGPA, bool expectedResult)
        {
            // Arrange
            var gpa = GPA.Create(actualGPA);
            var requirement = GPA.Create(requiredGPA);

            // Act
            var result = gpa.MeetsRequirement(requirement);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(0.0, 60.0)]
        [InlineData(2.0, 80.0)]
        [InlineData(4.0, 100.0)]
        public void ToPercentage_ShouldConvertCorrectly(decimal gpaValue, decimal expectedPercentage)
        {
            // Arrange
            var gpa = GPA.Create(gpaValue);

            // Act
            var percentage = gpa.ToPercentage();

            // Assert
            Assert.Equal(expectedPercentage, percentage);
        }

        [Fact]
        public void CompareTo_ShouldCompareCorrectly()
        {
            // Arrange
            var gpa1 = GPA.Create(3.0m);
            var gpa2 = GPA.Create(3.5m);
            var gpa3 = GPA.Create(3.0m);

            // Act & Assert
            Assert.True(gpa1.CompareTo(gpa2) < 0);
            Assert.True(gpa2.CompareTo(gpa1) > 0);
            Assert.True(gpa1.CompareTo(gpa3) == 0);
        }

        [Fact]
        public void ComparisonOperators_ShouldWorkCorrectly()
        {
            // Arrange
            var gpa1 = GPA.Create(3.0m);
            var gpa2 = GPA.Create(3.5m);

            // Act & Assert
            Assert.True(gpa2 > gpa1);
            Assert.True(gpa1 < gpa2);
            Assert.True(gpa2 >= gpa1);
            Assert.True(gpa1 <= gpa2);
        }

        [Fact]
        public void Equals_WithSameGPA_ShouldReturnTrue()
        {
            // Arrange
            var gpa1 = GPA.Create(3.5m);
            var gpa2 = GPA.Create(3.5m);

            // Act & Assert
            Assert.True(gpa1.Equals(gpa2));
            Assert.True(gpa1 == gpa2);
        }

        [Fact]
        public void Equals_WithDifferentGPAs_ShouldReturnFalse()
        {
            // Arrange
            var gpa1 = GPA.Create(3.5m);
            var gpa2 = GPA.Create(3.0m);

            // Act & Assert
            Assert.False(gpa1.Equals(gpa2));
            Assert.True(gpa1 != gpa2);
        }

        [Fact]
        public void ToString_ShouldReturnFormattedValue()
        {
            // Arrange
            var gpa = GPA.Create(3.5m);

            // Act
            var result = gpa.ToString();

            // Assert
            Assert.Equal("3.50", result);
        }

        [Fact]
        public void ToStringWithGrade_ShouldIncludeLetterGrade()
        {
            // Arrange
            var gpa = GPA.Create(3.5m);

            // Act
            var result = gpa.ToStringWithGrade();

            // Assert
            Assert.Equal("3.50 (A-)", result);
        }

        [Fact]
        public void ImplicitConversion_ShouldConvertToDecimal()
        {
            // Arrange
            var gpa = GPA.Create(3.5m);

            // Act
            decimal gpaValue = gpa;

            // Assert
            Assert.Equal(3.5m, gpaValue);
        }
    }
}
