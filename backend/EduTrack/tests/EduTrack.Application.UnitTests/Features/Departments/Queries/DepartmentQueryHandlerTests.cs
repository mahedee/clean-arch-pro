using AutoMapper;
using EduTrack.Application.Features.Departments.DTOs;
using EduTrack.Application.Features.Departments.Mappings;
using EduTrack.Application.Features.Departments.Queries.GetDepartment;
using EduTrack.Application.Features.Departments.Queries.GetDepartmentList;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Departments.Queries;

public class GetDepartmentQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
    private readonly IMapper _mapper;
    private readonly GetDepartmentQueryHandler _handler;

    public GetDepartmentQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockUnitOfWork.Setup(x => x.Departments).Returns(_mockDepartmentRepository.Object);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<DepartmentProfile>(), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        _mapper = mapperConfig.CreateMapper();

        _handler = new GetDepartmentQueryHandler(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ExistingDepartmentId_ShouldReturnDepartmentDto()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var department = Department.Create("Computer Science", "CS", "Dept of CS");

        _mockDepartmentRepository
            .Setup(x => x.GetByIdAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(department);

        var query = new GetDepartmentQuery(departmentId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Computer Science", result!.Name);
        Assert.Equal("CS", result.Code);
        Assert.Equal("Dept of CS", result.Description);
        Assert.Equal(DepartmentStatus.Active.ToString(), result.Status);
        _mockDepartmentRepository.Verify(x => x.GetByIdAsync(departmentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingDepartmentId_ShouldReturnNull()
    {
        // Arrange
        var departmentId = Guid.NewGuid();

        _mockDepartmentRepository
            .Setup(x => x.GetByIdAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Department?)null);

        var query = new GetDepartmentQuery(departmentId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}

public class GetDepartmentListQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
    private readonly IMapper _mapper;
    private readonly GetDepartmentListQueryHandler _handler;

    public GetDepartmentListQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockUnitOfWork.Setup(x => x.Departments).Returns(_mockDepartmentRepository.Object);

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<DepartmentProfile>(), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        _mapper = mapperConfig.CreateMapper();

        _handler = new GetDepartmentListQueryHandler(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldReturnPaginatedResult()
    {
        // Arrange
        var departments = new List<Department>
        {
            Department.Create("Computer Science", "CS"),
            Department.Create("Mathematics", "MATH")
        };

        _mockDepartmentRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(),
                It.IsAny<DepartmentStatus?>(), It.IsAny<string?>(),
                It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Department>)departments, 2));

        var query = new GetDepartmentListQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Departments.Count);
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
        _mockDepartmentRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(),
                It.IsAny<DepartmentStatus?>(), It.IsAny<string?>(),
                It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Department>)new List<Department>(), 0));

        var query = new GetDepartmentListQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Departments);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task Handle_StatusFilter_ShouldPassParsedStatusToRepository()
    {
        // Arrange
        _mockDepartmentRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(),
                It.Is<DepartmentStatus?>(s => s == DepartmentStatus.Active),
                It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Department>)new List<Department>(), 0));

        var query = new GetDepartmentListQuery { PageNumber = 1, PageSize = 10, Status = "Active" };

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockDepartmentRepository.Verify(
            x => x.GetPagedAsync(
                1, 10, null,
                It.Is<DepartmentStatus?>(s => s == DepartmentStatus.Active),
                It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_MappedDepartments_ShouldHaveCorrectValues()
    {
        // Arrange
        var departments = new List<Department>
        {
            Department.Create("Physics", "PHYS", "Department of Physics")
        };

        _mockDepartmentRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(),
                It.IsAny<DepartmentStatus?>(), It.IsAny<string?>(),
                It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Department>)departments, 1));

        var query = new GetDepartmentListQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.Departments);
        Assert.Equal("Physics", result.Departments[0].Name);
        Assert.Equal("PHYS", result.Departments[0].Code);
        Assert.Equal(DepartmentStatus.Active.ToString(), result.Departments[0].Status);
    }
}
