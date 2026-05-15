using EduTrack.Application.Features.Departments.Commands.UpdateDepartment;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using MediatR;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Departments.Commands;

public class UpdateDepartmentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
    private readonly UpdateDepartmentCommandHandler _handler;

    public UpdateDepartmentCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockUnitOfWork.Setup(x => x.Departments).Returns(_mockDepartmentRepository.Object);
        _handler = new UpdateDepartmentCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateDepartmentAndReturnUnit()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var department = Department.Create("Computer Science", "CS", "Original description");

        _mockDepartmentRepository
            .Setup(x => x.GetByIdAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(department);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateDepartmentCommand
        {
            Id = departmentId,
            Name = "Computer Science & Engineering",
            Description = "Updated description",
            Location = "Tech Building"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        _mockDepartmentRepository.Verify(x => x.Update(department), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DepartmentNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var departmentId = Guid.NewGuid();

        _mockDepartmentRepository
            .Setup(x => x.GetByIdAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Department?)null);

        var command = new UpdateDepartmentCommand { Id = departmentId, Name = "New Name" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateName_ShouldChangeNameOnDepartment()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var department = Department.Create("Computer Science", "CS");

        _mockDepartmentRepository
            .Setup(x => x.GetByIdAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(department);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateDepartmentCommand { Id = departmentId, Name = "Computer Science & AI" };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Computer Science & AI", department.Name);
    }

    [Fact]
    public async Task Handle_UpdateContactEmail_ShouldSetContactEmail()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var department = Department.Create("Mathematics", "MATH");

        _mockDepartmentRepository
            .Setup(x => x.GetByIdAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(department);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateDepartmentCommand { Id = departmentId, ContactEmail = "math@university.edu" };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(department.ContactEmail);
        Assert.Equal("math@university.edu", department.ContactEmail!.Value);
    }
}
