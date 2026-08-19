using AutoMapper;
using EduTrack.Application.Features.Teachers.DTOs;
using EduTrack.Application.Features.Teachers.Mappings;
using EduTrack.Application.Features.Teachers.Queries.GetTeacher;
using EduTrack.Application.Features.Teachers.Queries.GetTeacherList;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Teachers.Queries;

public class GetTeacherQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITeacherRepository> _mockTeacherRepository;
    private readonly IMapper _mapper;
    private readonly GetTeacherQueryHandler _handler;

    public GetTeacherQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTeacherRepository = new Mock<ITeacherRepository>();
        _mockUnitOfWork.Setup(x => x.Teachers).Returns(_mockTeacherRepository.Object);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<TeacherProfile>(), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        _mapper = mapperConfig.CreateMapper();

        _handler = new GetTeacherQueryHandler(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ExistingTeacherId_ShouldReturnTeacherDto()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var teacher = Teacher.Create(
            "Dr. Jane Smith", "jane.smith@university.edu",
            "EMP001", "Computer Science", AcademicTitle.AssistantProfessor,
            new DateTime(1980, 5, 15));

        _mockTeacherRepository
            .Setup(x => x.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        var query = new GetTeacherQuery(teacherId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(teacher.FullName.Value, result!.FullName);
        Assert.Equal(teacher.Email.Value, result.Email);
        Assert.Equal(teacher.Department, result.Department);
        Assert.Equal(teacher.Title.ToString(), result.Title);
        Assert.Equal(teacher.Status.ToString(), result.Status);
        _mockTeacherRepository.Verify(x => x.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingTeacherId_ShouldReturnNull()
    {
        // Arrange
        var teacherId = Guid.NewGuid();

        _mockTeacherRepository
            .Setup(x => x.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        var query = new GetTeacherQuery(teacherId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}

public class GetTeacherListQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITeacherRepository> _mockTeacherRepository;
    private readonly IMapper _mapper;
    private readonly GetTeacherListQueryHandler _handler;

    public GetTeacherListQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTeacherRepository = new Mock<ITeacherRepository>();
        _mockUnitOfWork.Setup(x => x.Teachers).Returns(_mockTeacherRepository.Object);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<TeacherProfile>(), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        _mapper = mapperConfig.CreateMapper();

        _handler = new GetTeacherListQueryHandler(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldReturnPaginatedResult()
    {
        // Arrange
        var teachers = new List<Teacher>
        {
            Teacher.Create("Dr. Jane Smith", "jane.smith@university.edu", "EMP001", "Computer Science", AcademicTitle.AssistantProfessor, new DateTime(1980, 5, 15)),
            Teacher.Create("Prof. Bob Brown", "bob.brown@university.edu", "EMP002", "Mathematics", AcademicTitle.Professor, new DateTime(1970, 3, 10))
        };

        _mockTeacherRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<EmploymentStatus?>(),
                It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Teacher>)teachers, 2));

        var query = new GetTeacherListQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Teachers.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task Handle_EmptyResult_ShouldReturnEmptyPaginatedResult()
    {
        // Arrange
        _mockTeacherRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<EmploymentStatus?>(),
                It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Teacher>)new List<Teacher>(), 0));

        var query = new GetTeacherListQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Teachers);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task Handle_StatusFilter_ShouldPassStatusToRepository()
    {
        // Arrange
        _mockTeacherRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.Is<EmploymentStatus?>(s => s == EmploymentStatus.Active),
                It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Teacher>)new List<Teacher>(), 0));

        var query = new GetTeacherListQuery { PageNumber = 1, PageSize = 10, Status = "Active" };

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockTeacherRepository.Verify(
            x => x.GetPagedAsync(
                1, 10, null, null,
                It.Is<EmploymentStatus?>(s => s == EmploymentStatus.Active),
                It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
