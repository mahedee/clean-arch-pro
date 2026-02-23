# Seed Data Implementation Guide

This guide explains how the seed data system is organized and how to add seed data for new entities.

## 📁 File Structure

```
EduTrack.Infrastructure/
└── Data/
    └── SeedData/
        ├── StudentSeedData.cs      # Student entity seed data
        ├── CourseSeedData.cs       # Course entity seed data  
        ├── TeacherSeedData.cs      # Teacher entity seed data
        ├── AttendanceSeedData.cs   # Attendance entity seed data
        └── DataSeeder.cs           # Central coordinator
```

## 🎯 Current Seed Data Entities

### Students (5 records)
- **John Michael Smith** - Active student, enrolled 2020
- **Sarah Elizabeth Johnson** - Active student, enrolled 2021
- **Michael Robert Davis** - Graduated student, enrolled 2019
- **Emily Rose Wilson** - Active student, enrolled 2022
- **David Anthony Brown** - Inactive student, enrolled 2021

### Courses (6 records)
- **CS101** - Introduction to Computer Science (30 capacity)
- **MATH101** - Calculus I (35 capacity)
- **CS301** - Database Management Systems (25 capacity)
- **CS401** - Software Engineering (20 capacity)
- **PHYS101** - General Physics I (40 capacity)
- **CS605** - Advanced Machine Learning (15 capacity, Graduate level)

### Teachers (5 records)
- **Dr. James Alexander Thompson** - Professor, Computer Science
- **Dr. Maria Elena Rodriguez** - Associate Professor, Mathematics
- **Prof. William Charles Anderson** - Assistant Professor, Computer Science
- **Dr. Jennifer Marie Clarke** - Professor, Physics
- **Dr. Robert Steven Martinez** - Professor, Computer Science

### Attendance (Generated records)
- 30 days of attendance data for 5 students across 3 courses
- 85% attendance rate with realistic patterns
- Excludes weekends automatically

## 🚀 How to Add Seed Data for New Entities

### Step 1: Create Entity Seed Data File
Create a new file following the naming pattern: `{EntityName}SeedData.cs`

```csharp
using EduTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Infrastructure.Data.SeedData;

public static class YourEntitySeedData
{
    public static void SeedYourEntity(ModelBuilder modelBuilder)
    {
        var data = GetYourEntityData();
        modelBuilder.Entity<YourEntity>().HasData(data);
    }

    private static List<object> GetYourEntityData()
    {
        return new List<object>
        {
            new
            {
                Id = Guid.NewGuid(),
                Property1 = "Value1",
                Property2 = "Value2",
                // ... other properties
            },
            // ... more records
        };
    }
}
```

### Step 2: Update DataSeeder.cs
Add your new seed method to the `SeedAllData` method:

```csharp
public static void SeedAllData(ModelBuilder modelBuilder)
{
    // Existing entities
    StudentSeedData.SeedStudents(modelBuilder);
    CourseSeedData.SeedCourses(modelBuilder);
    TeacherSeedData.SeedTeachers(modelBuilder);
    AttendanceSeedData.SeedAttendance(modelBuilder);
    
    // Add your new entity
    YourEntitySeedData.SeedYourEntity(modelBuilder);
}
```

### Step 3: Update ApplicationDbContext (if needed)
If your entity isn't already configured, add the DbSet:

```csharp
public DbSet<YourEntity> YourEntities => Set<YourEntity>();
```

### Step 4: Create and Apply Migration
Run the following commands to create and apply the migration:

```bash
dotnet ef migrations add AddSeedData
dotnet ef database update
```

## 💡 Best Practices

### 1. **Use Realistic Data**
- Use believable names, emails, and addresses
- Follow proper data formats (phone numbers, postal codes)
- Maintain referential integrity between related entities

### 2. **Consider Relationships**
- Seed data in proper order (parent entities before child entities)
- Use consistent IDs when referencing between entities
- Maintain foreign key constraints

### 3. **Use Value Objects Properly**
For entities using value objects (like `FullName`, `Email`), use string values in seed data:

```csharp
new
{
    FullName = "John Doe",           // Will be converted to FullName.Create("John Doe")
    Email = "john@example.com"       // Will be converted to Email.Create("john@example.com")
}
```

### 4. **Fixed GUIDs vs Random GUIDs**
- For entities that will be referenced by others, consider using fixed GUIDs
- For standalone entities, random GUIDs are acceptable

### 5. **Realistic Quantities**
- Students: 5-10 records for development
- Courses: 6-12 records covering different departments
- Teachers: 3-8 records with varied academic titles
- Attendance: Generate programmatically for realistic patterns

## 🔄 Updating Seed Data

To modify existing seed data:

1. Update the respective seed data file
2. Create a new migration: `dotnet ef migrations add UpdateSeedData`
3. Apply the migration: `dotnet ef database update`

## 🧪 Testing with Seed Data

The seed data provides a consistent foundation for:
- Integration tests
- API testing
- UI development
- Demonstration purposes

All seed data follows the domain validation rules and business logic, ensuring data integrity and realistic testing scenarios.

## 📝 Notes

- Seed data is applied automatically when the application starts
- The data will only be inserted if the tables are empty
- All seed data respects the domain model constraints and validation rules
- Value objects are properly converted using the configured converters