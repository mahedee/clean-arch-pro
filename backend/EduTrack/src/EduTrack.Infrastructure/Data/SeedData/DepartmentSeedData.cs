using EduTrack.Domain.Entities;
using EduTrack.Domain.ValueObjects;
using EduTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduTrack.Infrastructure.Data.SeedData;

public static class DepartmentSeedData
{
    public static async Task SeedDepartmentsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Departments.AnyAsync())
        {
            logger.LogInformation("Departments table already contains data. Skipping department seeding.");
            return;
        }

        logger.LogInformation("Seeding departments...");

        var departments = GetDepartments();
        await context.Departments.AddRangeAsync(departments);

        logger.LogInformation("Added {Count} departments to the database", departments.Count);
    }

    private static List<Department> GetDepartments()
    {
        var departments = new List<Department>();

        // Computer Science Department
        var cs = Department.Create(
            "Computer Science",
            "CS",
            "The Department of Computer Science offers programs in software engineering, artificial intelligence, data science, and systems programming."
        );
        cs.UpdateContactInformation(
            Email.Create("cs.dept@university.edu"),
            PhoneNumber.Create("+1-555-210-2001")
        );
        cs.UpdateLocation("Engineering Building, Floor 3");
        cs.UpdateBudget(1_500_000m);
        cs.UpdateFacultyCount(12);
        cs.UpdateStudentCount(320);
        departments.Add(cs);

        // Mathematics Department
        var math = Department.Create(
            "Mathematics",
            "MATH",
            "The Department of Mathematics provides rigorous instruction in pure and applied mathematics, statistics, and mathematical computing."
        );
        math.UpdateContactInformation(
            Email.Create("math.dept@university.edu"),
            PhoneNumber.Create("+1-555-220-2002")
        );
        math.UpdateLocation("Science Hall, Floor 2");
        math.UpdateBudget(900_000m);
        math.UpdateFacultyCount(8);
        math.UpdateStudentCount(210);
        departments.Add(math);

        // Physics Department
        var physics = Department.Create(
            "Physics",
            "PHYS",
            "The Department of Physics covers classical mechanics, electromagnetism, quantum physics, and experimental laboratory work."
        );
        physics.UpdateContactInformation(
            Email.Create("physics.dept@university.edu"),
            PhoneNumber.Create("+1-555-230-2003")
        );
        physics.UpdateLocation("Science Hall, Floor 4");
        physics.UpdateBudget(1_100_000m);
        physics.UpdateFacultyCount(9);
        physics.UpdateStudentCount(175);
        departments.Add(physics);

        // Electrical Engineering Department
        var ee = Department.Create(
            "Electrical Engineering",
            "EE",
            "The Department of Electrical Engineering focuses on circuits, signal processing, embedded systems, and power electronics."
        );
        ee.UpdateContactInformation(
            Email.Create("ee.dept@university.edu"),
            PhoneNumber.Create("+1-555-240-2004")
        );
        ee.UpdateLocation("Engineering Building, Floor 1");
        ee.UpdateBudget(1_200_000m);
        ee.UpdateFacultyCount(10);
        ee.UpdateStudentCount(280);
        departments.Add(ee);

        return departments;
    }
}
