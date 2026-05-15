using EduTrack.Application.Features.Students.Commands.DeleteStudent;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Students.Commands;

public class DeleteStudentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly DeleteStudentCommandHandler _handler;

    public DeleteStudentCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockUnitOfWork.Setup(x => x.Students).Returns(_mockStudentRepository.Object);
        _handler = new DeleteStudentCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingStudent_ShouldDeleteAndSaveChanges()
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

        var command = new DeleteStudentCommand(studentId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockStudentRepository.Verify(x => x.Delete(student), Times.Once);
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

        var command = new DeleteStudentCommand(studentId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        _mockStudentRepository.Verify(x => x.Delete(It.IsAny<Student>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ExistingStudent_ShouldPassCorrectStudentToDelete()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = Student.Create("Alice Johnson", new DateTime(1998, 6, 15), "alice.johnson@example.com");
        Student? deletedStudent = null;

        _mockStudentRepository
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _mockStudentRepository
            .Setup(x => x.Delete(It.IsAny<Student>()))
            .Callback<Student>(s => deletedStudent = s);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteStudentCommand(studentId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(deletedStudent);
        Assert.Equal(student.Email.Value, deletedStudent!.Email.Value);
    }
}
