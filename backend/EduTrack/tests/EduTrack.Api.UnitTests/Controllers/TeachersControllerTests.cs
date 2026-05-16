using EduTrack.Application.Features.Teachers.DTOs;
using EduTrack.Application.Features.Teachers.Commands.CreateTeacher;
using EduTrack.Application.Features.Teachers.Commands.DeleteTeacher;
using EduTrack.Application.Features.Teachers.Commands.UpdateTeacher;
using EduTrack.Application.Features.Teachers.Queries.GetTeacher;
using EduTrack.Application.Features.Teachers.Queries.GetTeacherList;
using EduTrack.Api.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EduTrack.Api.UnitTests.Controllers;

public class TeachersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TeachersController _controller;

    public TeachersControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new TeachersController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateTeacher_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateTeacherDto
        {
            FullName = "Dr. Jane Smith",
            Email = "jane.smith@university.edu",
            EmployeeId = "EMP001",
            Department = "Computer Science",
            Title = "AssistantProfessor"
        };
        var expectedId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTeacherCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _controller.CreateTeacher(dto);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(_controller.GetTeacher));
        created.Value.Should().Be(expectedId);
    }

    [Fact]
    public async Task CreateTeacher_DuplicateTeacher_ReturnsConflict()
    {
        // Arrange
        var dto = new CreateTeacherDto
        {
            FullName = "Dr. Jane Smith",
            Email = "jane.smith@university.edu",
            EmployeeId = "EMP001",
            Department = "Computer Science"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTeacherCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Teacher with this email already exists."));

        // Act
        var result = await _controller.CreateTeacher(dto);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task GetTeacher_ExistingId_ReturnsOkWithDto()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var teacherDto = new TeacherDto
        {
            Id = teacherId,
            FullName = "Dr. Jane Smith",
            Email = "jane.smith@university.edu"
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTeacherQuery>(q => q.TeacherId == teacherId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacherDto);

        // Act
        var result = await _controller.GetTeacher(teacherId);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(teacherDto);
    }

    [Fact]
    public async Task GetTeacher_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var teacherId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTeacherQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeacherDto?)null);

        // Act
        var result = await _controller.GetTeacher(teacherId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetTeachers_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var paginatedResult = new PaginatedTeacherListDto
        {
            Teachers = new List<TeacherListDto>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTeacherListQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetTeachers(new GetTeacherListQuery());

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact]
    public async Task UpdateTeacher_ExistingTeacher_ReturnsNoContent()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var dto = new UpdateTeacherDto { Email = "updated@university.edu" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateTeacherCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateTeacher(teacherId, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateTeacher_TeacherNotFound_ReturnsNotFound()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var dto = new UpdateTeacherDto { Email = "updated@university.edu" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateTeacherCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Teacher not found."));

        // Act
        var result = await _controller.UpdateTeacher(teacherId, dto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteTeacher_ExistingTeacher_ReturnsNoContent()
    {
        // Arrange
        var teacherId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteTeacherCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTeacher(teacherId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteTeacher_TeacherNotFound_ReturnsNotFound()
    {
        // Arrange
        var teacherId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteTeacherCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Teacher not found."));

        // Act
        var result = await _controller.DeleteTeacher(teacherId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
