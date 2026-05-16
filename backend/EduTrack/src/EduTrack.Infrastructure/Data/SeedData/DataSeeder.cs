using EduTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduTrack.Infrastructure.Data.SeedData;

/// <summary>
/// Central coordinator for all seed data operations
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Clears all existing data from every table in dependency-safe order,
    /// then seeds all entities with fresh sample data.
    /// </summary>
    /// <param name="context">The ApplicationDbContext instance</param>
    /// <param name="logger">Logger for tracking seed operations</param>
    public static async Task SeedAllDataAsync(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            await ClearAllDataAsync(context, logger);

            logger.LogInformation("Starting database seeding...");

            // Seed independent entities first and save so Attendance can query them by PK
            await StudentSeedData.SeedStudentsAsync(context, logger);
            await CourseSeedData.SeedCoursesAsync(context, logger);
            await TeacherSeedData.SeedTeachersAsync(context, logger);
            await DepartmentSeedData.SeedDepartmentsAsync(context, logger);
            await context.SaveChangesAsync();

            // Attendance depends on persisted Students and Courses
            await AttendanceSeedData.SeedAttendanceAsync(context, logger);
            await context.SaveChangesAsync();

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    /// <summary>
    /// Removes all rows from every table in dependency-safe order to allow
    /// a clean re-seed without foreign key violations.
    /// </summary>
    private static async Task ClearAllDataAsync(ApplicationDbContext context, ILogger logger)
    {
        logger.LogInformation("Clearing all existing data from the database...");

        // Delete in reverse dependency order so FK constraints are respected:
        // Attendances references Students and Courses → delete first
        // Departments references Teachers (via DepartmentHeadId) → delete before Teachers
        await context.Attendances.ExecuteDeleteAsync();
        await context.Departments.ExecuteDeleteAsync();
        await context.Students.ExecuteDeleteAsync();
        await context.Courses.ExecuteDeleteAsync();
        await context.Teachers.ExecuteDeleteAsync();

        logger.LogInformation("All existing data cleared successfully");
    }
}