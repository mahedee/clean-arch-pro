using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using EduTrack.Infrastructure.Data;
using EduTrack.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EduTrack.Infrastructure.UnitTests.Repositories;

public class StudentRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly StudentRepository _repository;

    public StudentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new StudentRepository(_context, NullLogger<StudentRepository>.Instance);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private static Student CreateStudent(string fullName = "John Smith", string email = "john.smith@university.edu")
    {
        return Student.Create(fullName, new DateTime(2020, 9, 1), email);
    }

    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ShouldReturnStudent()
    {
        // Arrange
        var student = CreateStudent();
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(student.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(student.Id, result!.Id);
        Assert.Equal(student.FullName.Value, result.FullName.Value);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllStudents()
    {
        // Arrange
        await _context.Students.AddRangeAsync(
            CreateStudent("John Smith", "john.smith@university.edu"),
            CreateStudent("Jane Doe", "jane.doe@university.edu"),
            CreateStudent("Bob Jones", "bob.jones@university.edu"));
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        var names = new[] { "Alice Smith", "Bob Jones", "Carol White", "David Brown", "Eva Green" };
        for (int i = 0; i < 5; i++)
        {
            await _context.Students.AddAsync(CreateStudent(names[i], $"student{i + 1}@university.edu"));
        }
        await _context.SaveChangesAsync();

        // Act
        var (students, totalCount) = await _repository.GetPagedAsync(pageNumber: 1, pageSize: 3);

        // Assert
        Assert.Equal(3, students.Count);
        Assert.Equal(5, totalCount);
    }

    [Fact]
    public async Task GetPagedAsync_SecondPage_ShouldReturnRemainingItems()
    {
        // Arrange
        var names = new[] { "Alice Smith", "Bob Jones", "Carol White", "David Brown", "Eva Green" };
        for (int i = 0; i < 5; i++)
        {
            await _context.Students.AddAsync(CreateStudent(names[i], $"student{i + 1}@university.edu"));
        }
        await _context.SaveChangesAsync();

        // Act
        var (students, totalCount) = await _repository.GetPagedAsync(pageNumber: 2, pageSize: 3);

        // Assert
        Assert.Equal(2, students.Count);
        Assert.Equal(5, totalCount);
    }

    [Fact]
    public async Task GetPagedAsync_WithStatusFilter_ShouldReturnFilteredStudents()
    {
        // Arrange
        var activeStudent = CreateStudent("Active Student", "active@university.edu");
        var inactiveStudent = CreateStudent("Inactive Student", "inactive@university.edu");
        inactiveStudent.Deactivate();

        await _context.Students.AddRangeAsync(activeStudent, inactiveStudent);
        await _context.SaveChangesAsync();

        // Act
        var (students, totalCount) = await _repository.GetPagedAsync(
            pageNumber: 1, pageSize: 10, status: StudentStatus.Active);

        // Assert - inactiveStudent is Inactive (Deactivated), so we'll have 1 Active
        Assert.Equal(1, totalCount);
        Assert.All(students, s => Assert.Equal(StudentStatus.Active, s.Status));
    }

    [Fact]
    public async Task Delete_ThenGetAllAsync_ShouldNotContainDeletedStudent()
    {
        // Arrange
        var student = CreateStudent("Delete Me", "deleteme@university.edu");
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        // Assert
        Assert.DoesNotContain(result, s => s.Id == student.Id);
    }

    [Fact]
    public async Task Update_ShouldPersistChanges()
    {
        // Arrange
        var student = CreateStudent("John Smith", "john.smith@university.edu");
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        student.Graduate();
        _context.Students.Update(student);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(student.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StudentStatus.Graduated, result!.Status);
    }
}
