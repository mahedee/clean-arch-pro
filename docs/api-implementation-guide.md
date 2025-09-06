# EduTrack API Implementation Guide

## 📋 Overview
This guide provides detailed implementation instructions for the EduTrack API layer, covering tasks T011A-T011C for Course, Teacher, and Attendance controllers.

**Prerequisites**: GitHub Issue #45 Complete ✅  
**Domain Entities**: Course, Teacher, Student (with 325 passing unit tests)  
**Target**: Complete CRUD APIs following Clean Architecture and CQRS patterns

## 🏆 Implementation Progress Summary

| Task | Component | Status | Progress | Priority |
|------|-----------|---------|----------|----------|
| **T011A** | 🎓 **Course Management API** | ✅ **COMPLETE** | 95% | ✅ **Done** |
| **T011B** | 👨‍🏫 **Teacher Management API** | ❌ **NOT STARTED** | 0% | 🔥 **HIGH** |
| **T011C** | 📊 **Attendance Management API** | ⚠️ **FOUNDATION** | 10% | 🔶 **MEDIUM** |

### 🎯 Current Status
- **✅ Course API**: Fully implemented with CQRS, validation, DTOs, unit tests (325 tests passing)
- **✅ Integration Tests**: Complete infrastructure with 13 comprehensive API tests running successfully
- **🚀 Next Priority**: Teacher Management API implementation
- **📊 Coverage**: Excellent test coverage across Domain and Application layers

---

## 🎯 Task T011A - Course Management API ✅ **COMPLETE**

### **Implementation Scope**
- **Duration**: 5-7 days
- **Dependencies**: Student CRUD (T011), Course Entity (GitHub Issue #45)
- **Deliverables**: Complete Course API with 10 endpoints

### **Required Course Endpoints**
```csharp
// Course CRUD Operations
[HttpGet("api/courses")]              // Get paginated course list
[HttpGet("api/courses/{id}")]         // Get course details
[HttpPost("api/courses")]             // Create new course
[HttpPut("api/courses/{id}")]         // Update course information

// Course Business Operations  
[HttpPost("api/courses/{id}/schedule")]     // Schedule course
[HttpPost("api/courses/{id}/activate")]     // Activate for enrollment
[HttpPost("api/courses/{id}/complete")]     // Mark as completed

// Student Enrollment Management
[HttpGet("api/courses/{id}/students")]               // Get enrolled students
[HttpPost("api/courses/{id}/students/{studentId}")]  // Enroll student
[HttpDelete("api/courses/{id}/students/{studentId}")] // Remove student
```

### **Implementation Checklist**
- [x] **Commands**: CreateCourseCommand, UpdateCourseCommand, ScheduleCourseCommand, ActivateCourseCommand, CompleteCourseCommand
- [x] **Queries**: GetCourseQuery, GetCourseListQuery, GetCoursesByDepartmentQuery ✅ **All implemented with handlers**
- [x] **Handlers**: Implement CQRS handlers with Course domain logic integration ✅ **Complete with domain integration**
- [x] **DTOs**: CourseDto, CreateCourseDto, UpdateCourseDto, CourseListDto, EnrollmentDto ✅ **Full DTO suite with pagination**
- [x] **Validation**: FluentValidation for all commands with business rules ✅ **Comprehensive validation pipeline**
- [x] **Mapping**: AutoMapper profiles for Course ↔ DTO transformations ✅ **Complete mapping with Value Objects**
- [x] **Controller**: CourseController with comprehensive error handling ✅ **Full CRUD + business operations**
- [x] **Unit Tests**: Handler and controller tests (target >95% coverage) ✅ **325 tests passing, excellent coverage**
- [x] **Integration Tests**: End-to-end API testing with test database ✅ **Complete - 13 tests running successfully**

### **Course Domain Integration**
```csharp
// Leverage existing Course entity methods:
course.ScheduleCourse(startDate, endDate, schedule);
course.ActivateCourse();
course.EnrollStudent(studentId);
course.CompleteCourse();

// Integrate domain events:
- CourseCreatedEvent
- CourseScheduledEvent  
- CourseActivatedEvent
- CourseCompletedEvent
```

---

## 🎓 Task T011B - Teacher Management API ❌ **NOT STARTED**

### **Implementation Scope**
- **Duration**: 5-7 days
- **Dependencies**: Course API (T011A), Teacher Entity (GitHub Issue #45)
- **Deliverables**: Teacher lifecycle management API with academic workflows

### **Required Teacher Endpoints**
```csharp
// Teacher CRUD Operations
[HttpGet("api/teachers")]             // Get paginated teacher list
[HttpGet("api/teachers/{id}")]        // Get teacher profile
[HttpPost("api/teachers")]            // Create teacher profile
[HttpPut("api/teachers/{id}")]        // Update teacher information

// Academic Workflow Operations
[HttpPost("api/teachers/{id}/hire")]           // Process hiring workflow
[HttpPut("api/teachers/{id}/contact")]         // Update contact info
[HttpPut("api/teachers/{id}/academic-title")]  // Update academic title

// Course Assignment Management
[HttpGet("api/teachers/{id}/courses")]                    // Get course assignments
[HttpPost("api/teachers/{id}/courses/{courseId}")]        // Assign to course
[HttpDelete("api/teachers/{id}/courses/{courseId}")]      // Remove assignment
```

### **Implementation Checklist**
- [❌] **Commands**: CreateTeacherCommand, UpdateTeacherCommand, HireTeacherCommand, AssignCourseCommand ❌ **Not started**
- [❌] **Queries**: GetTeacherQuery, GetTeacherListQuery, GetTeachersByCourseQuery ❌ **Not started**
- [❌] **Handlers**: CQRS handlers with Teacher domain logic and validation ❌ **Not started**
- [❌] **DTOs**: TeacherDto, CreateTeacherDto, UpdateTeacherDto, TeacherProfileDto ❌ **Not started**
- [❌] **Validation**: Academic credential validation and business rule enforcement ❌ **Not started**
- [❌] **Mapping**: AutoMapper with Value Object integration (FullName, Email, PhoneNumber) ❌ **Not started**
- [❌] **Controller**: TeacherController with academic workflow support ❌ **Not started**
- [❌] **Unit Tests**: Comprehensive testing of teacher operations ❌ **Not started**
- [❌] **Integration Tests**: Academic workflow and course assignment testing ❌ **Not started**

**📅 Status**: Ready to begin - Teacher Entity exists with domain logic ✅

### **Teacher Domain Integration**
```csharp
// Leverage existing Teacher entity methods:
teacher.HireTeacher(hireDate, department, initialTitle);
teacher.AssignToCourse(courseId, role);
teacher.UpdateAcademicTitle(newTitle, effectiveDate);
teacher.UpdateContactInformation(email, phoneNumber);

// Integrate domain events:
- TeacherCreatedEvent
- TeacherHiredEvent
- TeacherAssignedToCourseEvent
- TeacherContactUpdatedEvent
```

---

## 📊 Task T011C - Attendance Management API

### **Implementation Scope**
- **Duration**: 4-6 days
- **Dependencies**: Teacher API (T011B), Student/Course/Teacher entities
- **Deliverables**: Real-time attendance tracking with analytics

### **Required Attendance Endpoints**
```csharp
// Attendance Tracking
[HttpPost("api/attendance/mark")]           // Mark individual attendance
[HttpPost("api/attendance/bulk")]           // Bulk attendance for class
[HttpPut("api/attendance/{id}")]            // Update attendance record

// Attendance Queries
[HttpGet("api/attendance/session/{sessionId}")]     // Session attendance
[HttpGet("api/attendance/student/{studentId}")]     // Student history
[HttpGet("api/attendance/course/{courseId}")]       // Course summary

// Reporting and Analytics
[HttpGet("api/attendance/reports/daily")]                    // Daily reports
[HttpGet("api/attendance/reports/student/{studentId}")]      // Individual reports
[HttpGet("api/attendance/analytics/trends")]                 // Trend analysis
[HttpGet("api/attendance/alerts/low-attendance")]            // Alert system
```

### **New Domain Entity Required**
```csharp
// Create Attendance entity in Domain layer:
public class Attendance : AggregateRoot<Guid>
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid TeacherId { get; private set; }
    public DateTime SessionDate { get; private set; }
    public AttendanceStatus Status { get; private set; } // Present, Absent, Late, Excused
    public string? Notes { get; private set; }
    
    // Business methods:
    public void MarkAttendance(AttendanceStatus status, string? notes = null);
    public void UpdateAttendance(AttendanceStatus newStatus, string? notes = null);
    public bool IsWithinAllowedTimeWindow();
}
```

### **Implementation Checklist**
- [⚠️] **Domain Entity**: Create Attendance entity with business logic ⚠️ **Basic entity exists, needs enhancement**
- [❌] **Commands**: MarkAttendanceCommand, BulkAttendanceCommand, UpdateAttendanceCommand ❌ **Not started**
- [❌] **Queries**: GetAttendanceQuery, GetAttendanceReportQuery, GetStudentAttendanceQuery ❌ **Not started**
- [❌] **Handlers**: Multi-entity integration with Student, Course, Teacher ❌ **Not started**
- [❌] **DTOs**: AttendanceDto, MarkAttendanceDto, AttendanceReportDto, AttendanceAnalyticsDto ❌ **Not started**
- [❌] **Validation**: Time window validation, duplicate attendance prevention ❌ **Not started**
- [❌] **Controller**: AttendanceController with real-time capabilities ❌ **Not started**
- [❌] **Analytics**: Attendance trend analysis and automated alerts ❌ **Not started**
- [❌] **Unit Tests**: Attendance business logic and integration scenarios ❌ **Not started**
- [❌] **Integration Tests**: Multi-entity workflow testing ❌ **Not started**

**📅 Status**: Domain entity needs enhancement - Current entity is too basic for business requirements

### **Multi-Entity Integration**
```csharp
// Integration with existing entities:
var student = await _studentRepository.GetByIdAsync(studentId);
var course = await _courseRepository.GetByIdAsync(courseId);  
var teacher = await _teacherRepository.GetByIdAsync(teacherId);

// Validation rules:
- Verify student enrolled in course
- Confirm teacher assigned to course
- Check session within course schedule
- Prevent duplicate attendance records
```

---

## 🏗️ Common Implementation Patterns

### **CQRS Structure**
```
src/EduTrack.Application/
├── Features/
│   ├── Courses/
│   │   ├── Commands/
│   │   │   ├── CreateCourse/
│   │   │   ├── UpdateCourse/
│   │   │   └── ScheduleCourse/
│   │   └── Queries/
│   │       ├── GetCourse/
│   │       └── GetCourseList/
│   ├── Teachers/
│   └── Attendance/
```

### **AutoMapper Configuration**
```csharp
// Domain to DTO mapping profiles:
public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        CreateMap<Course, CourseDto>();
        CreateMap<CreateCourseDto, Course>();
        // Handle Value Object mapping
        CreateMap<Teacher, TeacherDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.ToString()));
    }
}
```

### **FluentValidation Example**
```csharp
public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Course title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");
            
        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Max capacity must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("Max capacity cannot exceed 500");
    }
}
```

### **Domain Event Integration**
```csharp
// In command handlers:
public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
{
    var course = Course.Create(request.Title, request.Description, request.MaxCapacity);
    
    await _courseRepository.AddAsync(course);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    
    // Domain events are automatically published
    return course.Id;
}
```

---

## 📊 Success Criteria

### **API Quality Standards**
- ✅ **Test Coverage**: >95% for all handlers and controllers
- ✅ **Performance**: <200ms response time for CRUD operations
- ✅ **Validation**: Comprehensive business rule enforcement
- ✅ **Documentation**: Complete Swagger/OpenAPI documentation
- ✅ **Error Handling**: Consistent error responses with proper HTTP codes

### **Architecture Compliance**
- ✅ **Clean Architecture**: Proper dependency direction
- ✅ **CQRS**: Clear separation of commands and queries
- ✅ **Domain Events**: Audit trail and process automation
- ✅ **Value Objects**: Primitive obsession elimination
- ✅ **Repository Pattern**: Consistent data access

### **Integration Testing**
- ✅ **End-to-End Workflows**: Complete business process testing
- ✅ **Multi-Entity Operations**: Student-Course-Teacher integration
- ✅ **Domain Event Verification**: Event publishing and handling
- ✅ **Performance Testing**: Load testing with realistic data volumes

---

## 📚 Resources

- **Testing Guide**: [Comprehensive Testing Tutorial](testing-guide.md) - How to run unit and integration tests
- **Domain Entities**: [Domain Implementation Summary](domain-implementation-summary.md)
- **Architecture Guide**: [Clean Architecture Documentation](architecture/)
- **Testing Standards**: [Test Coverage Guidelines](../tests/)
- **Code Examples**: Reference existing Student API implementation

---

*This guide will be updated as API implementation progresses through tasks T011A-T011C.*
