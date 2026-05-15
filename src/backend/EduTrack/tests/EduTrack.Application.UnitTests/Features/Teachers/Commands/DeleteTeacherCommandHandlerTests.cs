using EduTrack.Application.Features.Teachers.Commands.DeleteTeacher;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Teachers.Commands;

public class DeleteTeacherCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITeacherRepository> _mockTeacherRepository;
    private readonly DeleteTeacherCommandHandler _handler;

    public DeleteTeacherCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTeacherRepository = new Mock<ITeacherRepository>();
        _mockUnitOfWork.Setup(x => x.Teachers).Returns(_mockTeacherRepository.Object);
        _handler = new DeleteTeacherCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingTeacher_ShouldDeleteAndSaveChanges()
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
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteTeacherCommand(teacherId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTeacherRepository.Verify(x => x.Delete(teacher), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TeacherNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var teacherId = Guid.NewGuid();

        _mockTeacherRepository
            .Setup(x => x.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        var command = new DeleteTeacherCommand(teacherId);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _mockTeacherRepository.Verify(x => x.Delete(It.IsAny<Teacher>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ExistingTeacher_ShouldPassCorrectTeacherToDelete()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var teacher = Teacher.Create(
            "Prof. Robert Brown", "robert.brown@university.edu",
            "EMP002", "Mathematics", AcademicTitle.Professor,
            new DateTime(1970, 8, 22));
        Teacher? deletedTeacher = null;

        _mockTeacherRepository
            .Setup(x => x.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);
        _mockTeacherRepository
            .Setup(x => x.Delete(It.IsAny<Teacher>()))
            .Callback<Teacher>(t => deletedTeacher = t);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteTeacherCommand(teacherId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(deletedTeacher);
        Assert.Equal(teacher.Email.Value, deletedTeacher!.Email.Value);
        Assert.Equal(teacher.EmployeeId, deletedTeacher.EmployeeId);
    }
}
