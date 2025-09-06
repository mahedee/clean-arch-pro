using EduTrack.Application.Features.Courses.Commands.CreateCourse;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Repositories;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Courses.Commands;

public class CreateCourseCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly CreateCourseCommandHandler _handler;

    public CreateCourseCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockUnitOfWork.Setup(x => x.Courses).Returns(_mockCourseRepository.Object);
        _handler = new CreateCourseCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCourseAndReturnId()
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

        _mockCourseRepository.Setup(x => x.AddAsync(It.IsAny<Course>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockCourseRepository.Verify(x => x.AddAsync(It.IsAny<Course>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CommandWithPrerequisites_ShouldSetPrerequisiteRequirement()
    {
        // Arrange
        var command = new CreateCourseCommand
        {
            Title = "Advanced Programming",
            Description = "Advanced programming concepts and techniques",
            CourseCode = "CS201",
            Credits = 4,
            MaxCapacity = 25,
            Department = "Computer Science",
            Level = "Undergraduate",
            Prerequisites = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        Course capturedCourse = null;
        _mockCourseRepository.Setup(x => x.AddAsync(It.IsAny<Course>()))
            .Callback<Course>(course => capturedCourse = course)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCourse);
        Assert.Equal(6, capturedCourse.PrerequisiteCreditHours); // 2 prerequisites * 3 credits each
    }

    [Theory]
    [InlineData("InvalidLevel")]
    [InlineData("")]
    [InlineData("NotALevel")]
    public async Task Handle_InvalidLevel_ShouldThrowArgumentException(string invalidLevel)
    {
        // Arrange
        var command = new CreateCourseCommand
        {
            Title = "Test Course",
            Description = "Test Description",
            CourseCode = "TEST101",
            Credits = 3,
            MaxCapacity = 30,
            Department = "Test Department",
            Level = invalidLevel
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CommandWithAcademicPeriod_ShouldScheduleCourse()
    {
        // Arrange
        var command = new CreateCourseCommand
        {
            Title = "Data Structures",
            Description = "Introduction to data structures and algorithms",
            CourseCode = "CS102",
            Credits = 3,
            MaxCapacity = 30,
            Department = "Computer Science",
            Level = "Undergraduate",
            AcademicPeriod = "Spring 2025"
        };

        Course capturedCourse = null;
        _mockCourseRepository.Setup(x => x.AddAsync(It.IsAny<Course>()))
            .Callback<Course>(course => capturedCourse = course)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCourse);
        Assert.Equal("Spring", capturedCourse.Semester);
        Assert.Equal(2025, capturedCourse.AcademicYear);
        Assert.Equal(CourseStatus.Scheduled, capturedCourse.Status);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var command = new CreateCourseCommand
        {
            Title = "Database Systems",
            Description = "Introduction to database design and management",
            CourseCode = "CS301",
            Credits = 4,
            MaxCapacity = 25,
            Department = "Computer Science",
            Level = "Graduate"
        };

        Course capturedCourse = null;
        _mockCourseRepository.Setup(x => x.AddAsync(It.IsAny<Course>()))
            .Callback<Course>(course => capturedCourse = course)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedCourse);
        Assert.Equal(command.Title, capturedCourse.Title);
        Assert.Equal(command.Description, capturedCourse.Description);
        Assert.Equal(command.CourseCode, capturedCourse.Code);
        Assert.Equal(command.Credits, capturedCourse.CreditHours);
        Assert.Equal(command.MaxCapacity, capturedCourse.MaxEnrollment);
        Assert.Equal(command.Department, capturedCourse.Department);
        Assert.Equal(CourseLevel.Graduate, capturedCourse.Level);
    }
}
