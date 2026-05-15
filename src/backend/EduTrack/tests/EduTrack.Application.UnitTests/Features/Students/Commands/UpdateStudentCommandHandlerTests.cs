using AutoMapper;
using EduTrack.Application.Features.Students.Commands.UpdateStudent;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using MediatR;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Students.Commands;

public class UpdateStudentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UpdateStudentCommandHandler _handler;

    public UpdateStudentCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork.Setup(x => x.Students).Returns(_mockStudentRepository.Object);
        _handler = new UpdateStudentCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateStudentAndReturnUnit()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = Student.Create("John Smith", new DateTime(1995, 1, 15), "old.email@example.com");

        _mockStudentRepository
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _mockStudentRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateStudentCommand(studentId, FullName: "Jane Smith", Email: "jane.smith@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_StudentNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        _mockStudentRepository
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        var command = new UpdateStudentCommand(studentId, FullName: "Jane Smith");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmailAlreadyInUseByAnotherStudent_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = Student.Create("John Smith", new DateTime(1995, 1, 15), "john.smith@example.com");
        var anotherStudent = Student.Create("Bob Brown", new DateTime(1993, 3, 20), "taken@example.com");

        _mockStudentRepository
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _mockStudentRepository
            .Setup(x => x.GetByEmailAsync("taken@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(anotherStudent);

        var command = new UpdateStudentCommand(studentId, Email: "taken@example.com");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateGPA_ShouldSetGPAOnStudent()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = Student.Create("John Smith", new DateTime(1995, 1, 15), "john.smith@example.com");

        _mockStudentRepository
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateStudentCommand(studentId, GPA: 3.75m);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(student.CurrentGPA);
        Assert.Equal(3.75m, student.CurrentGPA!.Value);
    }

    [Fact]
    public async Task Handle_NoFieldsToUpdate_ShouldStillSaveChanges()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = Student.Create("John Smith", new DateTime(1995, 1, 15), "john.smith@example.com");

        _mockStudentRepository
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateStudentCommand(studentId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
