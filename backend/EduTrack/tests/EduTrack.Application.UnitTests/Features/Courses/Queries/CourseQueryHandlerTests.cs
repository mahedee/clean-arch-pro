using AutoMapper;
using EduTrack.Application.Features.Courses.DTOs;
using EduTrack.Application.Features.Courses.Queries.GetCourse;
using EduTrack.Application.Features.Courses.Queries.GetCourseList;
using EduTrack.Application.Features.Courses.Mappings;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Repositories;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Courses.Queries;

public class GetCourseQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly IMapper _mapper;
    private readonly GetCourseQueryHandler _handler;

    public GetCourseQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockUnitOfWork.Setup(x => x.Courses).Returns(_mockCourseRepository.Object);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CourseMappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _handler = new GetCourseQueryHandler(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidCourseId_ShouldReturnCourseDto()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = Course.Create(
            "Introduction to Computer Science",
            "CS101",
            "A comprehensive introduction to programming",
            3,
            CourseLevel.Undergraduate,
            "Computer Science",
            30);

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(course);

        var query = new GetCourseQuery(courseId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Title, result.Title);
        Assert.Equal(course.Code, result.Code);
        Assert.Equal(course.Description, result.Description);
        Assert.Equal(course.CreditHours, result.CreditHours);
        Assert.Equal(course.Level.ToString(), result.Level);
        Assert.Equal(course.Department, result.Department);
    }

    [Fact]
    public async Task Handle_InvalidCourseId_ShouldReturnNull()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        var query = new GetCourseQuery(courseId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mockCourseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
    }
}

public class GetCourseListQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly IMapper _mapper;
    private readonly GetCourseListQueryHandler _handler;

    public GetCourseListQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockUnitOfWork.Setup(x => x.Courses).Returns(_mockCourseRepository.Object);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CourseMappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _handler = new GetCourseListQueryHandler(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldReturnPaginatedResult()
    {
        // Arrange
        var courses = new List<Course>
        {
            Course.Create("Course 1", "CS101", "Description 1", 3, CourseLevel.Undergraduate, "CS", 30),
            Course.Create("Course 2", "CS102", "Description 2", 3, CourseLevel.Undergraduate, "CS", 25)
        };

        _mockCourseRepository.Setup(x => x.GetPaginatedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((courses, 2));

        var query = new GetCourseListQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Courses.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task Handle_EmptyResult_ShouldReturnEmptyPaginatedResult()
    {
        // Arrange
        _mockCourseRepository.Setup(x => x.GetPaginatedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((new List<Course>(), 0));

        var query = new GetCourseListQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Courses);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task Handle_PaginationCalculation_ShouldBeCorrect()
    {
        // Arrange
        var courses = new List<Course>
        {
            Course.Create("Course 1", "CS101", "Description 1", 3, CourseLevel.Undergraduate, "CS", 30)
        };

        _mockCourseRepository.Setup(x => x.GetPaginatedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((courses, 25)); // Total 25 items

        var query = new GetCourseListQuery
        {
            PageNumber = 2,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.TotalPages); // Ceiling of 25/10
        Assert.True(result.HasNextPage); // Page 2 of 3
        Assert.True(result.HasPreviousPage); // Page 2 > 1
    }
}
