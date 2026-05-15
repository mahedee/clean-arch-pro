using EduTrack.Application.Features.Teachers.Commands.UpdateTeacher;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using MediatR;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Teachers.Commands;

public class UpdateTeacherCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITeacherRepository> _mockTeacherRepository;
    private readonly UpdateTeacherCommandHandler _handler;

    public UpdateTeacherCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTeacherRepository = new Mock<ITeacherRepository>();
        _mockUnitOfWork.Setup(x => x.Teachers).Returns(_mockTeacherRepository.Object);
        _handler = new UpdateTeacherCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateTeacherAndReturnUnit()
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

        var command = new UpdateTeacherCommand
        {
            Id = teacherId,
            Email = "updated.jane@university.edu",
            OfficeLocation = "Room 205",
            OfficeHours = "Mon-Wed 10am-12pm"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        _mockTeacherRepository.Verify(x => x.Update(teacher), Times.Once);
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

        var command = new UpdateTeacherCommand { Id = teacherId, Email = "new@university.edu" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateTitle_ShouldSetNewAcademicTitle()
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

        var command = new UpdateTeacherCommand { Id = teacherId, Title = "Professor" };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(AcademicTitle.Professor, teacher.Title);
    }

    [Fact]
    public async Task Handle_UpdateOfficeInfo_ShouldSetOfficeLocationAndHours()
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

        var command = new UpdateTeacherCommand
        {
            Id = teacherId,
            OfficeLocation = "Building A, Room 101",
            OfficeHours = "Tuesday 2pm-4pm"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Building A, Room 101", teacher.OfficeLocation);
        Assert.Equal("Tuesday 2pm-4pm", teacher.OfficeHours);
    }
}
