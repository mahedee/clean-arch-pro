using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Application.Features.Students.Commands.CreateStudent;
using EduTrack.Application.Features.Students.Commands.DeleteStudent;
using EduTrack.Application.Features.Students.Commands.UpdateStudent;
using EduTrack.Application.Features.Students.Queries.GetStudent;
using EduTrack.Application.Features.Students.Queries.GetStudentList;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EduTrack.Api.UnitTests.Controllers;

public class StudentsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly StudentsController _controller;

    public StudentsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new StudentsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateStudent_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateStudentDto
        {
            FullName = "John Smith",
            Email = "john.smith@university.edu",
            DateOfBirth = new DateTime(1998, 1, 15)
        };
        var expectedId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateStudentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _controller.CreateStudent(dto);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(_controller.GetStudent));
        created.Value.Should().Be(expectedId);
    }

    [Fact]
    public async Task GetStudent_ExistingId_ReturnsOkWithDto()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var studentDto = new StudentDto
        {
            Id = studentId,
            FullName = "John Smith",
            Email = "john.smith@university.edu"
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetStudentQuery>(q => q.StudentId == studentId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentDto);

        // Act
        var result = await _controller.GetStudent(studentId);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(studentDto);
    }

    [Fact]
    public async Task GetStudent_NonExistingId_ReturnsOkWithNull()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStudentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentDto?)null);

        // Act
        var result = await _controller.GetStudent(studentId);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetStudents_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var paginatedResult = new PaginatedStudentListDto
        {
            Students = new List<StudentListDto>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStudentListQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetStudents(new GetStudentListQuery());

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact]
    public async Task UpdateStudent_ReturnsNoContent()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var dto = new UpdateStudentDto { FullName = "Jane Smith" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateStudentCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateStudent(studentId, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteStudent_ReturnsNoContent()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteStudentCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteStudent(studentId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
