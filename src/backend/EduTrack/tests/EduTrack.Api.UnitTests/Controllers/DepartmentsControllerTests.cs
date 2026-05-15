using EduTrack.Application.Features.Departments.DTOs;
using EduTrack.Application.Features.Departments.Commands.CreateDepartment;
using EduTrack.Application.Features.Departments.Commands.DeleteDepartment;
using EduTrack.Application.Features.Departments.Commands.UpdateDepartment;
using EduTrack.Application.Features.Departments.Queries.GetDepartment;
using EduTrack.Application.Features.Departments.Queries.GetDepartmentList;
using EduTrack.Api.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EduTrack.Api.UnitTests.Controllers;

public class DepartmentsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DepartmentsController _controller;

    public DepartmentsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new DepartmentsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateDepartment_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateDepartmentDto
        {
            Name = "Computer Science",
            Code = "CS",
            Description = "Department of Computer Science"
        };
        var expectedId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateDepartmentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _controller.CreateDepartment(dto);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(_controller.GetDepartment));
        created.Value.Should().Be(expectedId);
    }

    [Fact]
    public async Task CreateDepartment_DuplicateCode_ReturnsConflict()
    {
        // Arrange
        var dto = new CreateDepartmentDto { Name = "Computer Science", Code = "CS" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateDepartmentCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Department with code CS already exists."));

        // Act
        var result = await _controller.CreateDepartment(dto);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task GetDepartment_ExistingId_ReturnsOkWithDto()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var departmentDto = new DepartmentDto
        {
            Id = departmentId,
            Name = "Computer Science",
            Code = "CS"
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetDepartmentQuery>(q => q.DepartmentId == departmentId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(departmentDto);

        // Act
        var result = await _controller.GetDepartment(departmentId);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(departmentDto);
    }

    [Fact]
    public async Task GetDepartment_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var departmentId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetDepartmentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DepartmentDto?)null);

        // Act
        var result = await _controller.GetDepartment(departmentId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetDepartments_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var paginatedResult = new PaginatedDepartmentListDto
        {
            Departments = new List<DepartmentListDto>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetDepartmentListQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetDepartments(new GetDepartmentListQuery());

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact]
    public async Task UpdateDepartment_ExistingDepartment_ReturnsNoContent()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var dto = new UpdateDepartmentDto { Name = "Updated Name" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateDepartmentCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateDepartment(departmentId, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateDepartment_DepartmentNotFound_ReturnsNotFound()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var dto = new UpdateDepartmentDto { Name = "Updated Name" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateDepartmentCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Department not found."));

        // Act
        var result = await _controller.UpdateDepartment(departmentId, dto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteDepartment_ExistingDepartment_ReturnsNoContent()
    {
        // Arrange
        var departmentId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteDepartmentCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteDepartment(departmentId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteDepartment_DepartmentNotFound_ReturnsNotFound()
    {
        // Arrange
        var departmentId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteDepartmentCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Department not found."));

        // Act
        var result = await _controller.DeleteDepartment(departmentId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
