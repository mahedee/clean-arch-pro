using AutoMapper;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Application.Features.Students.Queries.GetStudent;
using EduTrack.Application.Features.Students.Queries.GetStudentList;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Students.Queries;

public class GetStudentQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetStudentQueryHandler _handler;

    public GetStudentQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork.Setup(x => x.Students).Returns(_mockStudentRepository.Object);
        _handler = new GetStudentQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingStudentId_ShouldReturnMappedStudentDto()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = Student.Create("John Smith", new DateTime(1995, 1, 15), "john.smith@example.com");
        var expectedDto = new StudentDto { Id = studentId, FullName = "John Smith", Email = "john.smith@example.com" };

        _mockStudentRepository
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _mockMapper
            .Setup(x => x.Map<StudentDto>(student))
            .Returns(expectedDto);

        var query = new GetStudentQuery(studentId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.FullName, result!.FullName);
        Assert.Equal(expectedDto.Email, result.Email);
        _mockStudentRepository.Verify(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingStudentId_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        _mockStudentRepository
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        var query = new GetStudentQuery(studentId);

        // Act & Assert
        await Assert.ThrowsAsync<EduTrack.Domain.Common.Exceptions.EntityNotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));
        _mockMapper.Verify(x => x.Map<StudentDto>(It.IsAny<Student>()), Times.Never);
    }
}

public class GetStudentListQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetStudentListQueryHandler _handler;

    public GetStudentListQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork.Setup(x => x.Students).Returns(_mockStudentRepository.Object);
        _handler = new GetStudentListQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldReturnPaginatedResult()
    {
        // Arrange
        var students = new List<Student>
        {
            Student.Create("John Smith", new DateTime(1995, 1, 15), "john.smith@example.com"),
            Student.Create("Jane Doe", new DateTime(1996, 3, 20), "jane.doe@example.com")
        };
        var studentDtos = new List<StudentListDto>
        {
            new() { FullName = "John Smith", Email = "john.smith@example.com" },
            new() { FullName = "Jane Doe", Email = "jane.doe@example.com" }
        };

        _mockStudentRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(),
                It.IsAny<StudentStatus?>(), It.IsAny<string?>(),
                It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Student>)students, 2));
        _mockMapper
            .Setup(x => x.Map<List<StudentListDto>>(students))
            .Returns(studentDtos);

        var query = new GetStudentListQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task Handle_EmptyResult_ShouldReturnEmptyPaginatedResult()
    {
        // Arrange
        _mockStudentRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(),
                It.IsAny<StudentStatus?>(), It.IsAny<string?>(),
                It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Student>)new List<Student>(), 0));
        _mockMapper
            .Setup(x => x.Map<List<StudentListDto>>(It.IsAny<List<Student>>()))
            .Returns(new List<StudentListDto>());

        var query = new GetStudentListQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Students);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task Handle_MultiplePages_ShouldCalculatePaginationCorrectly()
    {
        // Arrange
        var students = new List<Student>
        {
            Student.Create("John Smith", new DateTime(1995, 1, 15), "john.smith@example.com")
        };

        _mockStudentRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(),
                It.IsAny<StudentStatus?>(), It.IsAny<string?>(),
                It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Student>)students, 25));
        _mockMapper
            .Setup(x => x.Map<List<StudentListDto>>(It.IsAny<List<Student>>()))
            .Returns(new List<StudentListDto> { new() { FullName = "John Smith" } });

        var query = new GetStudentListQuery { PageNumber = 2, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
    }
}
