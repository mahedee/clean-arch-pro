using EduTrack.Application.Features.Departments.Commands.DeleteDepartment;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using Moq;
using Xunit;

namespace EduTrack.Application.UnitTests.Features.Departments.Commands;

public class DeleteDepartmentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
    private readonly DeleteDepartmentCommandHandler _handler;

    public DeleteDepartmentCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockUnitOfWork.Setup(x => x.Departments).Returns(_mockDepartmentRepository.Object);
        _handler = new DeleteDepartmentCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ExistingDepartment_ShouldDeleteAndSaveChanges()
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

        var command = new DeleteDepartmentCommand(departmentId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockDepartmentRepository.Verify(x => x.Delete(department), Times.Once);
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

        var command = new DeleteDepartmentCommand(departmentId);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _mockDepartmentRepository.Verify(x => x.Delete(It.IsAny<Department>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ExistingDepartment_ShouldPassCorrectDepartmentToDelete()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var department = Department.Create("Mathematics", "MATH", "Department of Mathematics");
        Department? deletedDepartment = null;

        _mockDepartmentRepository
            .Setup(x => x.GetByIdAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(department);
        _mockDepartmentRepository
            .Setup(x => x.Delete(It.IsAny<Department>()))
            .Callback<Department>(d => deletedDepartment = d);
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteDepartmentCommand(departmentId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(deletedDepartment);
        Assert.Equal("Mathematics", deletedDepartment!.Name);
        Assert.Equal("MATH", deletedDepartment.Code);
    }
}
