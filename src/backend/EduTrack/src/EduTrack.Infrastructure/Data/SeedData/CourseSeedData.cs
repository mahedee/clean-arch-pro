using EduTrack.Domain.Entities;
using EduTrack.Domain.Enums;
using EduTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static EduTrack.Domain.Entities.Course;

namespace EduTrack.Infrastructure.Data.SeedData;

public static class CourseSeedData
{
    public static async Task SeedCoursesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Courses.AnyAsync())
        {
            logger.LogInformation("Courses table already contains data. Skipping course seeding.");
            return;
        }

        logger.LogInformation("Seeding courses...");

        var courses = GetCourses();
        await context.Courses.AddRangeAsync(courses);
        
        logger.LogInformation("Added {Count} courses to the database", courses.Count);
    }

    private static List<Course> GetCourses()
    {
        var courses = new List<Course>();

        // Introduction to Computer Science
        var course1 = Course.Create(
            "Introduction to Computer Science",
            "CS101",
            "Fundamental concepts of computer science including programming, algorithms, and data structures. This course introduces students to computational thinking and problem-solving techniques.",
            3,
            CourseLevel.Undergraduate,
            "Computer Science",
            30
        );
        course1.Schedule("Fall 2026", 2026, new DateTime(2026, 8, 24), new DateTime(2026, 12, 13));
        courses.Add(course1);

        // Calculus I
        var course2 = Course.Create(
            "Calculus I",
            "MATH101",
            "Introduction to differential and integral calculus. Covers limits, derivatives, and integrals with applications to real-world problems.",
            4,
            CourseLevel.Undergraduate,
            "Mathematics",
            35
        );
        course2.Schedule("Fall 2026", 2026, new DateTime(2026, 8, 24), new DateTime(2026, 12, 13));
        courses.Add(course2);

        // Database Management Systems
        var course3 = Course.Create(
            "Database Management Systems",
            "CS301",
            "Comprehensive study of database design, implementation, and management. Covers relational databases, SQL, normalization, and database administration.",
            3,
            CourseLevel.Undergraduate,
            "Computer Science",
            25
        );
        course3.Schedule("Spring 2027", 2027, new DateTime(2027, 1, 18), new DateTime(2027, 5, 8));
        courses.Add(course3);

        // Software Engineering
        var course4 = Course.Create(
            "Software Engineering",
            "CS401",
            "Advanced software development methodologies, project management, and software lifecycle management. Includes team-based software development projects.",
            4,
            CourseLevel.Undergraduate,
            "Computer Science",
            20
        );
        course4.Schedule("Fall 2026", 2026, new DateTime(2026, 8, 24), new DateTime(2026, 12, 13));
        courses.Add(course4);

        // General Physics I
        var course5 = Course.Create(
            "General Physics I",
            "PHYS101",
            "Introduction to mechanics, waves, and thermodynamics. Laboratory work included to reinforce theoretical concepts.",
            4,
            CourseLevel.Undergraduate,
            "Physics",
            40
        );
        course5.Schedule("Fall 2026", 2026, new DateTime(2026, 8, 24), new DateTime(2026, 12, 13));
        courses.Add(course5);

        // Advanced Machine Learning
        var course6 = Course.Create(
            "Advanced Machine Learning",
            "CS605",
            "Graduate-level course covering advanced machine learning algorithms, deep learning, and artificial intelligence applications in various domains.",
            3,
            CourseLevel.Graduate,
            "Computer Science",
            15
        );
        course6.Schedule("Spring 2027", 2027, new DateTime(2027, 1, 18), new DateTime(2027, 5, 8));
        courses.Add(course6);

        return courses;
    }
}