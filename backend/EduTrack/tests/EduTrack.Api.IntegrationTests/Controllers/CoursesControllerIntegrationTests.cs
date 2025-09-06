using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using EduTrack.Infrastructure.Data;
using EduTrack.Application.Features.Courses.DTOs;
using EduTrack.Domain.Entities;
using Newtonsoft.Json;
using static EduTrack.Domain.Entities.Course;
using static EduTrack.Domain.Entities.Teacher;

namespace EduTrack.Api.IntegrationTests.Controllers
{
    public class CoursesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public CoursesControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateCourse_WithValidData_ReturnsCreatedCourse()
        {
            // Arrange
            var createCourseDto = new CreateCourseDto
            {
                Title = "Integration Test Course",
                CourseCode = "ITC001",
                Credits = 3,
                MaxCapacity = 30,
                Level = "Undergraduate",
                Description = "A course for integration testing",
                Department = "Computer Science"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/courses", createCourseDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdCourse = JsonConvert.DeserializeObject<CourseDto>(responseContent);
            
            createdCourse.Should().NotBeNull();
            createdCourse!.Title.Should().Be(createCourseDto.Title);
            createdCourse.Code.Should().Be(createCourseDto.CourseCode);
            createdCourse.CreditHours.Should().Be(createCourseDto.Credits);
            createdCourse.Description.Should().Be(createCourseDto.Description);
            createdCourse.Department.Should().Be(createCourseDto.Department);
            createdCourse.Id.Should().NotBeEmpty();
            createdCourse.Status.Should().Be("Draft");
            
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateCourse_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var createCourseDto = new CreateCourseDto
            {
                Title = "", // Invalid empty title
                CourseCode = "ITC002",
                Credits = 3,
                MaxCapacity = 30,
                Level = "Undergraduate",
                Description = "A course for integration testing",
                Department = "Computer Science"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/courses", createCourseDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetCourse_WithExistingId_ReturnsCourse()
        {
            // Arrange
            var course = await CreateTestCourse();

            // Act
            var response = await _client.GetAsync($"/api/courses/{course.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var retrievedCourse = JsonConvert.DeserializeObject<CourseDto>(responseContent);
            
            retrievedCourse.Should().NotBeNull();
            retrievedCourse!.Id.Should().Be(course.Id);
            retrievedCourse.Title.Should().Be(course.Title);
            retrievedCourse.Code.Should().Be(course.Code);
        }

        [Fact]
        public async Task GetCourse_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/courses/{nonExistentId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCourses_WithDefaultParameters_ReturnsPagedCourses()
        {
            // Arrange
            await CreateTestCourse("Course 1", "C001");
            await CreateTestCourse("Course 2", "C002");
            await CreateTestCourse("Course 3", "C003");

            // Act
            var response = await _client.GetAsync("/api/courses");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var pagedResult = JsonConvert.DeserializeObject<PaginatedCourseListDto>(responseContent);
            
            pagedResult.Should().NotBeNull();
            pagedResult!.Courses.Should().HaveCountGreaterOrEqualTo(3);
            pagedResult.TotalCount.Should().BeGreaterOrEqualTo(3);
            pagedResult.PageNumber.Should().Be(1);
            pagedResult.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task GetCourses_WithSearchTerm_ReturnsFilteredCourses()
        {
            // Arrange
            await CreateTestCourse("Programming Basics", "PB001");
            await CreateTestCourse("Advanced Mathematics", "AM001");
            await CreateTestCourse("Programming Advanced", "PA001");

            // Act
            var response = await _client.GetAsync("/api/courses?searchTerm=Programming");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var pagedResult = JsonConvert.DeserializeObject<PaginatedCourseListDto>(responseContent);
            
            pagedResult.Should().NotBeNull();
            pagedResult!.Courses.Should().HaveCount(2);
            pagedResult.Courses.Should().OnlyContain(c => c.Title.Contains("Programming"));
        }

        [Fact]
        public async Task GetCourses_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
                await CreateTestCourse($"Course {i:D2}", $"C{i:D3}");
            }

            // Act
            var response = await _client.GetAsync("/api/courses?pageNumber=2&pageSize=5");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var pagedResult = JsonConvert.DeserializeObject<PaginatedCourseListDto>(responseContent);
            
            pagedResult.Should().NotBeNull();
            pagedResult!.Courses.Should().HaveCount(5);
            pagedResult.PageNumber.Should().Be(2);
            pagedResult.PageSize.Should().Be(5);
            pagedResult.TotalCount.Should().BeGreaterOrEqualTo(15);
        }

        [Fact]
        public async Task GetCoursesByDepartment_WithExistingDepartment_ReturnsCourses()
        {
            // Arrange
            await CreateTestCourse("CS Course 1", "CS001", "Computer Science");
            await CreateTestCourse("CS Course 2", "CS002", "Computer Science");
            await CreateTestCourse("Math Course", "MATH001", "Mathematics");

            // Act
            var response = await _client.GetAsync("/api/courses/department/Computer Science");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var courses = JsonConvert.DeserializeObject<List<CourseListDto>>(responseContent);
            
            courses.Should().NotBeNull();
            courses.Should().HaveCount(2);
            courses.Should().OnlyContain(c => c.Department == "Computer Science");
        }

        [Fact]
        public async Task UpdateCourse_WithValidData_ReturnsUpdatedCourse()
        {
            // Arrange
            var course = await CreateTestCourse();
            var updateDto = new UpdateCourseDto
            {
                Title = "Updated Course Title",
                Description = "Updated description",
                Credits = 4
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/courses/{course.Id}", updateDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedCourse = JsonConvert.DeserializeObject<CourseDto>(responseContent);
            
            updatedCourse.Should().NotBeNull();
            updatedCourse!.Id.Should().Be(course.Id);
            updatedCourse.Title.Should().Be(updateDto.Title);
            updatedCourse.Description.Should().Be(updateDto.Description);
            updatedCourse.CreditHours.Should().Be(updateDto.Credits);
        }

        [Fact]
        public async Task ScheduleCourse_WithValidData_ReturnsScheduledCourse()
        {
            // Arrange
            var course = await CreateTestCourse();
            var startDate = DateTime.UtcNow.AddDays(30);
            var endDate = startDate.AddDays(90);

            // Act
            var response = await _client.PostAsync(
                $"/api/courses/{course.Id}/schedule?startDate={startDate:yyyy-MM-ddTHH:mm:ss.fffZ}&endDate={endDate:yyyy-MM-ddTHH:mm:ss.fffZ}",
                null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var scheduledCourse = JsonConvert.DeserializeObject<CourseDto>(responseContent);
            
            scheduledCourse.Should().NotBeNull();
            scheduledCourse!.Id.Should().Be(course.Id);
            scheduledCourse.Status.Should().Be("Scheduled");
            scheduledCourse.StartDate.Should().BeCloseTo(startDate, TimeSpan.FromSeconds(1));
            scheduledCourse.EndDate.Should().BeCloseTo(endDate, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ActivateCourse_WithValidCourse_ReturnsActivatedCourse()
        {
            // Arrange
            var course = await CreateTestCourse();

            // Act
            var response = await _client.PostAsync($"/api/courses/{course.Id}/activate", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var activatedCourse = JsonConvert.DeserializeObject<CourseDto>(responseContent);
            
            activatedCourse.Should().NotBeNull();
            activatedCourse!.Id.Should().Be(course.Id);
            activatedCourse.Status.Should().Be("Active");
        }

        [Fact]
        public async Task CompleteCourse_WithValidCourse_ReturnsCompletedCourse()
        {
            // Arrange
            var course = await CreateTestCourse();

            // Act
            var response = await _client.PostAsync($"/api/courses/{course.Id}/complete", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var completedCourse = JsonConvert.DeserializeObject<CourseDto>(responseContent);
            
            completedCourse.Should().NotBeNull();
            completedCourse!.Id.Should().Be(course.Id);
            completedCourse.Status.Should().Be("Completed");
        }

        [Fact]
        public async Task CourseWorkflow_CreateScheduleActivateComplete_WorksCorrectly()
        {
            // Arrange
            var createDto = new CreateCourseDto
            {
                Title = "Workflow Test Course",
                CourseCode = "WTC001",
                Credits = 3,
                MaxCapacity = 30,
                Level = "Undergraduate",
                Description = "Testing complete workflow",
                Department = "Computer Science"
            };

            var startDate = DateTime.UtcNow.AddDays(30);
            var endDate = startDate.AddDays(90);

            // Act & Assert - Create
            var createResponse = await _client.PostAsJsonAsync("/api/courses", createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdCourse = JsonConvert.DeserializeObject<CourseDto>(
                await createResponse.Content.ReadAsStringAsync());

            // Act & Assert - Schedule
            var scheduleResponse = await _client.PostAsync(
                $"/api/courses/{createdCourse!.Id}/schedule?startDate={startDate:yyyy-MM-ddTHH:mm:ss.fffZ}&endDate={endDate:yyyy-MM-ddTHH:mm:ss.fffZ}",
                null);
            scheduleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var scheduledCourse = JsonConvert.DeserializeObject<CourseDto>(
                await scheduleResponse.Content.ReadAsStringAsync());
            scheduledCourse!.Status.Should().Be("Scheduled");

            // Act & Assert - Activate
            var activateResponse = await _client.PostAsync($"/api/courses/{createdCourse.Id}/activate", null);
            activateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var activatedCourse = JsonConvert.DeserializeObject<CourseDto>(
                await activateResponse.Content.ReadAsStringAsync());
            activatedCourse!.Status.Should().Be("Active");

            // Act & Assert - Complete
            var completeResponse = await _client.PostAsync($"/api/courses/{createdCourse.Id}/complete", null);
            completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var completedCourse = JsonConvert.DeserializeObject<CourseDto>(
                await completeResponse.Content.ReadAsStringAsync());
            completedCourse!.Status.Should().Be("Completed");
        }

        private async Task<Course> CreateTestCourse(string title = "Test Course", string code = "TC001", string department = "Computer Science")
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = Course.Create(title, code, "Test description", 3, CourseLevel.Undergraduate, department);
            dbContext.Courses.Add(course);
            await dbContext.SaveChangesAsync();

            return course;
        }
    }
}
