using EduTrack.Application.Features.Departments.Commands.CreateDepartment;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Departments.Commands;

public class CreateDepartmentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
    private readonly CreateDepartmentCommandHandler _handler;

    public CreateDepartmentCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockUnitOfWork.Setup(x => x.Departments).Returns(_mockDepartmentRepository.Object);
        _handler = new CreateDepartmentCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateDepartmentAndReturnId()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            Name = "Computer Science",
            Code = "CS",
            Description = "Department of Computer Science"
        };

        _mockDepartmentRepository
            .Setup(x => x.ExistsByCodeAsync(command.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockDepartmentRepository
            .Setup(x => x.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _mockDepartmentRepository.Verify(x => x.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            Name = "Computer Science",
            Code = "CS"
        };

        _mockDepartmentRepository
            .Setup(x => x.ExistsByCodeAsync(command.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        _mockDepartmentRepository.Verify(x => x.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldSetCorrectDepartmentProperties()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            Name = "Mathematics",
            Code = "MATH",
            Description = "Department of Mathematics"
        };

        Department? capturedDepartment = null;
        _mockDepartmentRepository
            .Setup(x => x.ExistsByCodeAsync(command.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockDepartmentRepository
            .Setup(x => x.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
            .Callback<Department, CancellationToken>((d, _) => capturedDepartment = d)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedDepartment);
        Assert.Equal("Mathematics", capturedDepartment!.Name);
        Assert.Equal("MATH", capturedDepartment.Code);
        Assert.Equal("Department of Mathematics", capturedDepartment.Description);
    }

    [Fact]
    public async Task Handle_CommandWithLocation_ShouldSetLocation()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            Name = "Physics",
            Code = "PHYS",
            Location = "Science Building, Floor 3"
        };

        Department? capturedDepartment = null;
        _mockDepartmentRepository
            .Setup(x => x.ExistsByCodeAsync(command.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockDepartmentRepository
            .Setup(x => x.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
            .Callback<Department, CancellationToken>((d, _) => capturedDepartment = d)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedDepartment);
        Assert.Equal("Science Building, Floor 3", capturedDepartment!.Location);
    }

    [Fact]
    public async Task Handle_CommandWithContactInfo_ShouldSetContactEmailAndPhone()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            Name = "Engineering",
            Code = "ENG",
            ContactEmail = "eng@university.edu",
            ContactPhone = "+1-555-234-5678"
        };

        Department? capturedDepartment = null;
        _mockDepartmentRepository
            .Setup(x => x.ExistsByCodeAsync(command.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockDepartmentRepository
            .Setup(x => x.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()))
            .Callback<Department, CancellationToken>((d, _) => capturedDepartment = d)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedDepartment);
        Assert.NotNull(capturedDepartment!.ContactEmail);
        Assert.Equal("eng@university.edu", capturedDepartment.ContactEmail!.Value);
    }
}
