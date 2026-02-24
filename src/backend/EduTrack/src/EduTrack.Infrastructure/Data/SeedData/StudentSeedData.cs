using EduTrack.Domain.Entities;
using EduTrack.Domain.ValueObjects;
using EduTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static EduTrack.Domain.Entities.Student;

namespace EduTrack.Infrastructure.Data.SeedData;

public static class StudentSeedData
{
    public static async Task SeedStudentsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Students.AnyAsync())
        {
            logger.LogInformation("Students table already contains data. Skipping student seeding.");
            return;
        }

        logger.LogInformation("Seeding students...");

        var students = GetStudents();
        await context.Students.AddRangeAsync(students);
        
        logger.LogInformation("Added {Count} students to the database", students.Count);
    }

    private static List<Student> GetStudents()
    {
        var students = new List<Student>();

        // John Michael Smith - Active student
        var student1 = Student.Create(
            "John Michael Smith",
            new DateTime(1998, 5, 15),
            "john.smith@student.edu"
        );
        student1.UpdatePhoneNumber("+1-555-223-4567");
        students.Add(student1);

        // Sarah Elizabeth Johnson - Active student  
        var student2 = Student.Create(
            "Sarah Elizabeth Johnson",
            new DateTime(1999, 3, 22),
            "sarah.johnson@student.edu"
        );
        student2.UpdatePhoneNumber("+1-555-234-5678");
        students.Add(student2);

        // Michael Robert Davis - Graduated student
        var student3 = Student.Create(
            "Michael Robert Davis",
            new DateTime(1997, 11, 8),
            "michael.davis@student.edu"
        );
        student3.UpdatePhoneNumber("+1-555-345-6789");
        student3.Graduate();
        students.Add(student3);

        // Emily Rose Wilson - Active student
        var student4 = Student.Create(
            "Emily Rose Wilson", 
            new DateTime(2000, 7, 14),
            "emily.wilson@student.edu"
        );
        student4.UpdatePhoneNumber("+1-555-456-7890");
        students.Add(student4);

        // David Anthony Brown - Inactive student
        var student5 = Student.Create(
            "David Anthony Brown",
            new DateTime(1999, 12, 3),
            "david.brown@student.edu"
        );
        student5.UpdatePhoneNumber("+1-555-567-8901");
        student5.Deactivate();
        students.Add(student5);

        return students;
    }
}