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
    /// Seeds all entities with sample data at application startup
    /// </summary>
    /// <param name="context">The ApplicationDbContext instance</param>
    /// <param name="logger">Logger for tracking seed operations</param>
    public static async Task SeedAllDataAsync(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            // Check if data already exists to avoid duplicates
            if (await context.Students.AnyAsync() || await context.Courses.AnyAsync() || 
                await context.Teachers.AnyAsync())
            {
                logger.LogInformation("Database already contains seed data. Skipping seeding.");
                return;
            }

            logger.LogInformation("Starting database seeding...");

            // Seed data in proper order considering relationships
            await StudentSeedData.SeedStudentsAsync(context, logger);
            await CourseSeedData.SeedCoursesAsync(context, logger);
            await TeacherSeedData.SeedTeachersAsync(context, logger);
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
}