using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using EduTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static EduTrack.Domain.Entities.Student;
using static EduTrack.Domain.Entities.Course;

namespace EduTrack.Infrastructure.Data.SeedData;

public static class AttendanceSeedData
{
    public static async Task SeedAttendanceAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Attendances.AnyAsync())
        {
            logger.LogInformation("Attendance table already contains data. Skipping attendance seeding.");
            return;
        }

        logger.LogInformation("Seeding attendance records...");

        var attendanceRecords = await GetAttendanceRecordsAsync(context, logger);
        if (attendanceRecords.Count > 0)
        {
            await context.Attendances.AddRangeAsync(attendanceRecords);
            logger.LogInformation("Added {Count} attendance records to the database", attendanceRecords.Count);
        }
        else
        {
            logger.LogWarning("No attendance records generated. Make sure Students and Courses are seeded first.");
        }
    }

    private static async Task<List<Attendance>> GetAttendanceRecordsAsync(ApplicationDbContext context, ILogger logger)
    {
        var attendanceRecords = new List<Attendance>();
        
        // Get existing students and courses
        var students = await context.Students.Where(s => s.Status == StudentStatus.Active).ToListAsync();
        var courses = await context.Courses.Where(c => c.Status == CourseStatus.Active).ToListAsync();
        
        if (!students.Any())
        {
            logger.LogWarning("No active students found. Cannot create attendance records.");
            return attendanceRecords;
        }
        
        if (!courses.Any())
        {
            logger.LogWarning("No active courses found. Cannot create attendance records.");
            return attendanceRecords;
        }
        
        logger.LogInformation("Creating attendance records for {StudentCount} students and {CourseCount} courses", 
            students.Count, courses.Count);
        
        var random = new Random(42); // Fixed seed for consistent data
        var startDate = new DateTime(2024, 1, 15);
        
        // Generate attendance records for the past 30 days
        for (int day = 0; day < 30; day++)
        {
            var currentDate = startDate.AddDays(day);
            
            // Skip weekends
            if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                continue;
            
            // Generate attendance for each student in each course
            foreach (var student in students.Take(5)) // Limit to first 5 students
            {
                foreach (var course in courses.Take(3)) // Limit to first 3 courses
                {
                    // 85% chance of being present
                    var isPresent = random.NextDouble() > 0.15;
                    
                    var attendance = Attendance.RecordAttendance(
                        studentId: student.Id,
                        courseId: course.Id,
                        attendanceDate: currentDate,
                        isPresent: isPresent,
                        recordedBy: "System",
                        notes: isPresent ? null : "Generated absence"
                    );
                    
                    attendanceRecords.Add(attendance);
                }
            }
        }
        
        return attendanceRecords;
    }
}