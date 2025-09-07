using EduTrack.Application.Features.Courses.Commands.CreateCourse;
using EduTrack.Application.Features.Courses.Queries.GetCourse;
using EduTrack.Application.Features.Courses.Queries.GetCourseList;
using FluentValidation.TestHelper;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Courses.Validators;

public class CreateCourseCommandValidatorTests
{
    private readonly CreateCourseCommandValidator _validator;

    public CreateCourseCommandValidatorTests()
    {
        _validator = new CreateCourseCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateCourseCommand
        {
            Title = "Introduction to Computer Science",
            Description = "A comprehensive introduction to programming and computer science fundamentals",
            CourseCode = "CS101",
            Credits = 3,
            MaxCapacity = 30,
            Department = "Computer Science",
            Level = "Undergraduate",
            AcademicPeriod = "Fall 2025"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("AB")] // Too short
    [InlineData(null)]
    public void Validate_InvalidTitle_ShouldHaveValidationError(string title)
    {
        // Arrange
        var command = new CreateCourseCommand { Title = title };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Short")] // Too short
    [InlineData(null)]
    public void Validate_InvalidDescription_ShouldHaveValidationError(string description)
    {
        // Arrange
        var command = new CreateCourseCommand { Description = description };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("cs101")] // Invalid format - should be uppercase
    [InlineData("C101")] // Too short prefix
    [InlineData("CS1")] // Too short number
    [InlineData("CSABC")] // No numbers
    [InlineData("12345")] // No letters
    public void Validate_InvalidCourseCode_ShouldHaveValidationError(string courseCode)
    {
        // Arrange
        var command = new CreateCourseCommand { CourseCode = courseCode };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CourseCode);
    }

    [Theory]
    [InlineData("CS101")]
    [InlineData("MATH2010")]
    [InlineData("PHYS1234")]
    [InlineData("ENG101")]
    public void Validate_ValidCourseCode_ShouldNotHaveValidationError(string courseCode)
    {
        // Arrange
        var command = new CreateCourseCommand 
        { 
            CourseCode = courseCode,
            Title = "Valid Title",
            Description = "Valid description with enough characters",
            Credits = 3,
            MaxCapacity = 30,
            Department = "Valid Department",
            Level = "Undergraduate"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CourseCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(13)] // Exceeds maximum
    public void Validate_InvalidCredits_ShouldHaveValidationError(int credits)
    {
        // Arrange
        var command = new CreateCourseCommand { Credits = credits };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Credits);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(501)] // Exceeds maximum
    public void Validate_InvalidMaxCapacity_ShouldHaveValidationError(int maxCapacity)
    {
        // Arrange
        var command = new CreateCourseCommand { MaxCapacity = maxCapacity };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxCapacity);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_InvalidDepartment_ShouldHaveValidationError(string department)
    {
        // Arrange
        var command = new CreateCourseCommand { Department = department };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Department);
    }

    [Theory]
    [InlineData("")]
    [InlineData("InvalidLevel")]
    [InlineData("Bachelor")] // Not in allowed list
    [InlineData(null)]
    public void Validate_InvalidLevel_ShouldHaveValidationError(string level)
    {
        // Arrange
        var command = new CreateCourseCommand { Level = level };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level);
    }

    [Theory]
    [InlineData("Undergraduate")]
    [InlineData("Graduate")]
    [InlineData("Doctorate")]
    [InlineData("Certificate")]
    public void Validate_ValidLevel_ShouldNotHaveValidationError(string level)
    {
        // Arrange
        var command = new CreateCourseCommand 
        { 
            Level = level,
            Title = "Valid Title",
            Description = "Valid description with enough characters",
            CourseCode = "CS101",
            Credits = 3,
            MaxCapacity = 30,
            Department = "Valid Department"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Level);
    }

    [Fact]
    public void Validate_TooManyPrerequisites_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateCourseCommand 
        { 
            Prerequisites = Enumerable.Range(1, 11).Select(_ => Guid.NewGuid()).ToList() // 11 prerequisites
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Prerequisites);
    }
}

public class GetCourseQueryValidatorTests
{
    private readonly GetCourseQueryValidator _validator;

    public GetCourseQueryValidatorTests()
    {
        _validator = new GetCourseQueryValidator();
    }

    [Fact]
    public void Validate_ValidQuery_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetCourseQuery(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyGuid_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetCourseQuery(Guid.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CourseId);
    }
}

public class GetCourseListQueryValidatorTests
{
    private readonly GetCourseListQueryValidator _validator;

    public GetCourseListQueryValidatorTests()
    {
        _validator = new GetCourseListQueryValidator();
    }

    [Fact]
    public void Validate_ValidQuery_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetCourseListQuery
        {
            PageNumber = 1,
            PageSize = 10,
            SortBy = "Title",
            SortDirection = "asc"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidPageNumber_ShouldHaveValidationError(int pageNumber)
    {
        // Arrange
        var query = new GetCourseListQuery { PageNumber = pageNumber };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)] // Exceeds maximum
    public void Validate_InvalidPageSize_ShouldHaveValidationError(int pageSize)
    {
        // Arrange
        var query = new GetCourseListQuery { PageSize = pageSize };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData("InvalidSort")]
    [InlineData("Name")] // Not in allowed list
    public void Validate_InvalidSortBy_ShouldHaveValidationError(string sortBy)
    {
        // Arrange
        var query = new GetCourseListQuery { SortBy = sortBy };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("up")]
    [InlineData("down")]
    public void Validate_InvalidSortDirection_ShouldHaveValidationError(string sortDirection)
    {
        // Arrange
        var query = new GetCourseListQuery { SortDirection = sortDirection };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortDirection);
    }
}
