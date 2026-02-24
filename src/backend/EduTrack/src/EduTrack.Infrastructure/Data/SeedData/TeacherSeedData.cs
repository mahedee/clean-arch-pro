using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using EduTrack.Domain.ValueObjects;
using EduTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static EduTrack.Domain.Entities.Teacher;

namespace EduTrack.Infrastructure.Data.SeedData;

public static class TeacherSeedData
{
    public static async Task SeedTeachersAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Teachers.AnyAsync())
        {
            logger.LogInformation("Teachers table already contains data. Skipping teacher seeding.");
            return;
        }

        logger.LogInformation("Seeding teachers...");

        var teachers = GetTeachers();
        await context.Teachers.AddRangeAsync(teachers);
        
        logger.LogInformation("Added {Count} teachers to the database", teachers.Count);
    }

    private static List<Teacher> GetTeachers()
    {
        var teachers = new List<Teacher>();

        // Dr. James Alexander Thompson - Professor
        var teacher1 = Teacher.Create(
            "Dr. James Alexander Thompson",
            "james.thompson@university.edu",
            "EMP001",
            "Computer Science",
            AcademicTitle.Professor,
            new DateTime(1975, 3, 22)
        );
        teacher1.UpdateContactInformation(
            Email.Create("james.thompson@university.edu"), 
            PhoneNumber.Create("+1-555-211-2222")
        );
        teachers.Add(teacher1);

        // Dr. Maria Elena Rodriguez - Associate Professor
        var teacher2 = Teacher.Create(
            "Dr. Maria Elena Rodriguez",
            "maria.rodriguez@university.edu",
            "EMP002",
            "Mathematics",
            AcademicTitle.AssociateProfessor,
            new DateTime(1980, 7, 14)
        );
        teacher2.UpdateContactInformation(
            Email.Create("maria.rodriguez@university.edu"), 
            PhoneNumber.Create("+1-555-222-3333")
        );
        teachers.Add(teacher2);

        // Prof. William Charles Anderson - Assistant Professor
        var teacher3 = Teacher.Create(
            "Prof. William Charles Anderson",
            "william.anderson@university.edu",
            "EMP003",
            "Computer Science",
            AcademicTitle.AssistantProfessor,
            new DateTime(1985, 11, 8)
        );
        teacher3.UpdateContactInformation(
            Email.Create("william.anderson@university.edu"), 
            PhoneNumber.Create("+1-555-333-4444")
        );
        teachers.Add(teacher3);

        // Dr. Jennifer Marie Clarke - Professor
        var teacher4 = Teacher.Create(
            "Dr. Jennifer Marie Clarke",
            "jennifer.clarke@university.edu",
            "EMP004",
            "Physics",
            AcademicTitle.Professor,
            new DateTime(1972, 12, 3)
        );
        teacher4.UpdateContactInformation(
            Email.Create("jennifer.clarke@university.edu"), 
            PhoneNumber.Create("+1-555-444-5555")
        );
        teachers.Add(teacher4);

        // Dr. Robert Steven Martinez - Professor  
        var teacher5 = Teacher.Create(
            "Dr. Robert Steven Martinez",
            "robert.martinez@university.edu",
            "EMP005",
            "Computer Science",
            AcademicTitle.Professor,
            new DateTime(1970, 5, 15)
        );
        teacher5.UpdateContactInformation(
            Email.Create("robert.martinez@university.edu"), 
            PhoneNumber.Create("+1-555-555-6666")
        );
        teachers.Add(teacher5);

        return teachers;
    }
}