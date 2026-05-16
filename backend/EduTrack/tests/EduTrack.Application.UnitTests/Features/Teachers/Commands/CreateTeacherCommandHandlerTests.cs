using EduTrack.Application.Features.Teachers.Commands.CreateTeacher;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Teachers.Commands;

public class CreateTeacherCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITeacherRepository> _mockTeacherRepository;
    private readonly CreateTeacherCommandHandler _handler;

    public CreateTeacherCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTeacherRepository = new Mock<ITeacherRepository>();
        _mockUnitOfWork.Setup(x => x.Teachers).Returns(_mockTeacherRepository.Object);
        _handler = new CreateTeacherCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateTeacherAndReturnId()
    {
        // Arrange
        var command = new CreateTeacherCommand
        {
            FullName = "Dr. Jane Smith",
            Email = "jane.smith@university.edu",
            EmployeeId = "EMP001",
            Department = "Computer Science",
            Title = "AssistantProfessor",
            DateOfBirth = new DateTime(1980, 5, 15)
        };

        _mockTeacherRepository
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);
        _mockTeacherRepository
            .Setup(x => x.ExistsByEmployeeIdAsync(command.EmployeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockTeacherRepository
            .Setup(x => x.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockTeacherRepository.Verify(x => x.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateTeacherCommand
        {
            FullName = "Dr. Jane Smith",
            Email = "jane.smith@university.edu",
            EmployeeId = "EMP001",
            Department = "Computer Science",
            Title = "AssistantProfessor",
            DateOfBirth = new DateTime(1980, 5, 15)
        };

        var existingTeacher = Teacher.Create(
            "Another Person", "jane.smith@university.edu", "EMP999",
            "Math", AcademicTitle.Lecturer, new DateTime(1975, 1, 1));

        _mockTeacherRepository
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTeacher);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        _mockTeacherRepository.Verify(x => x.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DuplicateEmployeeId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateTeacherCommand
        {
            FullName = "Dr. Jane Smith",
            Email = "jane.smith@university.edu",
            EmployeeId = "EMP001",
            Department = "Computer Science",
            Title = "AssistantProfessor",
            DateOfBirth = new DateTime(1980, 5, 15)
        };

        _mockTeacherRepository
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);
        _mockTeacherRepository
            .Setup(x => x.ExistsByEmployeeIdAsync(command.EmployeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        _mockTeacherRepository.Verify(x => x.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldSetCorrectTeacherProperties()
    {
        // Arrange
        var command = new CreateTeacherCommand
        {
            FullName = "Prof. Robert Brown",
            Email = "robert.brown@university.edu",
            EmployeeId = "EMP002",
            Department = "Mathematics",
            Title = "Professor",
            DateOfBirth = new DateTime(1970, 8, 22)
        };

        Teacher? capturedTeacher = null;
        _mockTeacherRepository
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);
        _mockTeacherRepository
            .Setup(x => x.ExistsByEmployeeIdAsync(command.EmployeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockTeacherRepository
            .Setup(x => x.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()))
            .Callback<Teacher, CancellationToken>((t, _) => capturedTeacher = t)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTeacher);
        Assert.Equal("robert.brown@university.edu", capturedTeacher!.Email.Value);
        Assert.Equal("Mathematics", capturedTeacher.Department);
        Assert.Equal(AcademicTitle.Professor, capturedTeacher.Title);
        Assert.Equal(EmploymentStatus.Active, capturedTeacher.Status);
    }

    [Fact]
    public async Task Handle_UnknownTitle_ShouldDefaultToLecturer()
    {
        // Arrange
        var command = new CreateTeacherCommand
        {
            FullName = "Dr. Jane Smith",
            Email = "jane.smith@university.edu",
            EmployeeId = "EMP001",
            Department = "Computer Science",
            Title = "InvalidTitle",
            DateOfBirth = new DateTime(1980, 5, 15)
        };

        Teacher? capturedTeacher = null;
        _mockTeacherRepository
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);
        _mockTeacherRepository
            .Setup(x => x.ExistsByEmployeeIdAsync(command.EmployeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockTeacherRepository
            .Setup(x => x.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()))
            .Callback<Teacher, CancellationToken>((t, _) => capturedTeacher = t)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTeacher);
        Assert.Equal(AcademicTitle.Lecturer, capturedTeacher!.Title);
    }
}
