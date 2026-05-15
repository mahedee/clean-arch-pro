using AutoMapper;
using EduTrack.Application.Features.Students.Commands.CreateStudent;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Students.Commands;

public class CreateStudentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreateStudentCommandHandler _handler;

    public CreateStudentCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork.Setup(x => x.Students).Returns(_mockStudentRepository.Object);
        _handler = new CreateStudentCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateStudentAndReturnId()
    {
        // Arrange
        var command = new CreateStudentCommand("John Smith", new DateTime(1995, 1, 15), "john.smith@example.com");

        _mockStudentRepository
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);
        _mockStudentRepository
            .Setup(x => x.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockStudentRepository.Verify(x => x.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateStudentCommand("John Smith", new DateTime(1995, 1, 15), "john.smith@example.com");
        var existingStudent = Student.Create("Jane Doe", new DateTime(1994, 1, 1), "john.smith@example.com");

        _mockStudentRepository
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingStudent);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        _mockStudentRepository.Verify(x => x.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldSetCorrectStudentProperties()
    {
        // Arrange
        var dob = new DateTime(1998, 6, 15);
        var command = new CreateStudentCommand("Alice Johnson", dob, "alice.johnson@example.com");

        Student? capturedStudent = null;
        _mockStudentRepository
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);
        _mockStudentRepository
            .Setup(x => x.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()))
            .Callback<Student, CancellationToken>((s, _) => capturedStudent = s)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedStudent);
        Assert.Equal("Alice Johnson", capturedStudent!.FullName.Value);
        Assert.Equal("alice.johnson@example.com", capturedStudent.Email.Value);
        Assert.Equal(dob, capturedStudent.DateOfBirth);
    }

    [Fact]
    public async Task Handle_CommandWithPhoneNumber_ShouldSetPhoneNumber()
    {
        // Arrange
        var command = new CreateStudentCommand(
            "John Smith",
            new DateTime(1995, 1, 15),
            "john.smith@example.com",
            PhoneNumber: "+1-555-234-5678");

        Student? capturedStudent = null;
        _mockStudentRepository
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);
        _mockStudentRepository
            .Setup(x => x.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()))
            .Callback<Student, CancellationToken>((s, _) => capturedStudent = s)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedStudent);
        Assert.NotNull(capturedStudent!.PhoneNumber);
    }

    [Fact]
    public async Task Handle_CommandWithAddress_ShouldSetAddress()
    {
        // Arrange
        var command = new CreateStudentCommand(
            "John Smith",
            new DateTime(1995, 1, 15),
            "john.smith@example.com",
            Street: "123 Main St",
            City: "Springfield",
            State: "IL",
            ZipCode: "62701",
            Country: "USA");

        Student? capturedStudent = null;
        _mockStudentRepository
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);
        _mockStudentRepository
            .Setup(x => x.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()))
            .Callback<Student, CancellationToken>((s, _) => capturedStudent = s)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedStudent);
        Assert.NotNull(capturedStudent!.Address);
        Assert.Equal("123 Main St", capturedStudent.Address!.Street);
        Assert.Equal("Springfield", capturedStudent.Address.City);
    }
}
