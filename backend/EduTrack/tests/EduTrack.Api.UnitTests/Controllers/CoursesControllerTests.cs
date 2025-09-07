using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MediatR;
using Xunit;
using FluentAssertions;
using EduTrack.Api.Controllers;
using EduTrack.Application.Features.Courses.Commands.CreateCourse;
using EduTrack.Application.Features.Courses.Commands.UpdateCourse;
using EduTrack.Application.Features.Courses.Commands.ScheduleCourse;
using EduTrack.Application.Features.Courses.Commands.ActivateCourse;
using EduTrack.Application.Features.Courses.Commands.CompleteCourse;
using EduTrack.Application.Features.Courses.Queries.GetCourse;
using EduTrack.Application.Features.Courses.Queries.GetCourseList;
using EduTrack.Application.Features.Courses.Queries.GetCoursesByDepartment;
using EduTrack.Application.Features.Courses.DTOs;
using FluentValidation;

namespace EduTrack.Api.UnitTests.Controllers
{
    public class CoursesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<CoursesController>> _loggerMock;
        private readonly CoursesController _controller;

        public CoursesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<CoursesController>>();
            _controller = new CoursesController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateCourse_WithValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            var createCourseDto = new CreateCourseDto
            {
                Title = "Test Course",
                CourseCode = "TC001",
                Credits = 3,
                Description = "Test course description",
                Department = "Computer Science"
            };

            var expectedCourse = new CourseDto
            {
                Id = Guid.NewGuid(),
                Title = createCourseDto.Title,
                Code = createCourseDto.CourseCode,
                CreditHours = createCourseDto.Credits,
                Description = createCourseDto.Description,
                Department = createCourseDto.Department,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.CreateCourse(createCourseDto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(CoursesController.GetCourse));
            createdResult.RouteValues.Should().ContainKey("id");
            createdResult.Value.Should().BeEquivalentTo(expectedCourse);

            _mediatorMock.Verify(
                m => m.Send(It.Is<CreateCourseCommand>(c =>
                    c.Name == createCourseDto.Name &&
                    c.Code == createCourseDto.Code &&
                    c.Credits == createCourseDto.Credits &&
                    c.Description == createCourseDto.Description &&
                    c.Department == createCourseDto.Department),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateCourse_WithInvalidRequest_ThrowsValidationException()
        {
            // Arrange
            var createCourseDto = new CreateCourseDto
            {
                Name = "", // Invalid empty name
                Code = "TC001",
                Credits = 3,
                Description = "Test course description",
                Department = "Computer Science"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateCourseCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("Name is required"));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _controller.CreateCourse(createCourseDto));
        }

        [Fact]
        public async Task GetCourse_WithExistingId_ReturnsOkResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedCourse = new CourseDto
            {
                Id = courseId,
                Name = "Test Course",
                Code = "TC001",
                Credits = 3,
                Description = "Test course description",
                Department = "Computer Science",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.GetCourse(courseId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedCourse);

            _mediatorMock.Verify(
                m => m.Send(It.Is<GetCourseQuery>(q => q.Id == courseId), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCourse_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseDto)null);

            // Act
            var result = await _controller.GetCourse(courseId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetCourses_WithValidParameters_ReturnsOkResult()
        {
            // Arrange
            var request = new GetCourseListQuery
            {
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = "Test",
                SortBy = "Name",
                SortDirection = "asc"
            };

            var expectedResult = new PaginatedCourseListDto
            {
                Items = new List<CourseListDto>
                {
                    new CourseListDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test Course 1",
                        Code = "TC001",
                        Credits = 3,
                        Department = "Computer Science",
                        Status = "Active"
                    }
                },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10,
                TotalPages = 1
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetCourseListQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetCourses(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.SortBy,
                request.SortDirection);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedResult);

            _mediatorMock.Verify(
                m => m.Send(It.Is<GetCourseListQuery>(q =>
                    q.PageNumber == request.PageNumber &&
                    q.PageSize == request.PageSize &&
                    q.SearchTerm == request.SearchTerm &&
                    q.SortBy == request.SortBy &&
                    q.SortDirection == request.SortDirection),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCoursesByDepartment_WithValidDepartment_ReturnsOkResult()
        {
            // Arrange
            var department = "Computer Science";
            var expectedCourses = new List<CourseListDto>
            {
                new CourseListDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Programming 101",
                    Code = "CS101",
                    Credits = 3,
                    Department = department,
                    Status = "Active"
                },
                new CourseListDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Data Structures",
                    Code = "CS201",
                    Credits = 4,
                    Department = department,
                    Status = "Active"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetCoursesByDepartmentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCourses);

            // Act
            var result = await _controller.GetCoursesByDepartment(department);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedCourses);

            _mediatorMock.Verify(
                m => m.Send(It.Is<GetCoursesByDepartmentQuery>(q => q.Department == department), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateCourse_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var updateCourseDto = new UpdateCourseDto
            {
                Name = "Updated Course Name",
                Description = "Updated description",
                Credits = 4
            };

            var expectedCourse = new CourseDto
            {
                Id = courseId,
                Name = updateCourseDto.Name,
                Code = "TC001",
                Credits = updateCourseDto.Credits,
                Description = updateCourseDto.Description,
                Department = "Computer Science",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.UpdateCourse(courseId, updateCourseDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedCourse);

            _mediatorMock.Verify(
                m => m.Send(It.Is<UpdateCourseCommand>(c =>
                    c.Id == courseId &&
                    c.Name == updateCourseDto.Name &&
                    c.Description == updateCourseDto.Description &&
                    c.Credits == updateCourseDto.Credits),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ScheduleCourse_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(30);
            var endDate = startDate.AddDays(90);
            var teacherId = Guid.NewGuid();

            var expectedCourse = new CourseDto
            {
                Id = courseId,
                Name = "Test Course",
                Code = "TC001",
                Credits = 3,
                Description = "Test course description",
                Department = "Computer Science",
                Status = "Scheduled",
                StartDate = startDate,
                EndDate = endDate,
                TeacherId = teacherId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ScheduleCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.ScheduleCourse(courseId, startDate, endDate, teacherId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedCourse);

            _mediatorMock.Verify(
                m => m.Send(It.Is<ScheduleCourseCommand>(c =>
                    c.Id == courseId &&
                    c.StartDate == startDate &&
                    c.EndDate == endDate &&
                    c.TeacherId == teacherId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ActivateCourse_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedCourse = new CourseDto
            {
                Id = courseId,
                Name = "Test Course",
                Code = "TC001",
                Credits = 3,
                Description = "Test course description",
                Department = "Computer Science",
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ActivateCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.ActivateCourse(courseId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedCourse);

            _mediatorMock.Verify(
                m => m.Send(It.Is<ActivateCourseCommand>(c => c.Id == courseId), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CompleteCourse_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedCourse = new CourseDto
            {
                Id = courseId,
                Name = "Test Course",
                Code = "TC001",
                Credits = 3,
                Description = "Test course description",
                Department = "Computer Science",
                Status = "Completed",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CompleteCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.CompleteCourse(courseId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedCourse);

            _mediatorMock.Verify(
                m => m.Send(It.Is<CompleteCourseCommand>(c => c.Id == courseId), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(0, 10)] // Invalid page number
        [InlineData(1, 0)]  // Invalid page size
        [InlineData(1, 101)] // Page size too large
        public async Task GetCourses_WithInvalidParameters_ThrowsValidationException(int pageNumber, int pageSize)
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetCourseListQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("Invalid pagination parameters"));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _controller.GetCourses(pageNumber, pageSize, null, null, null));
        }

        [Fact]
        public async Task GetCoursesByDepartment_WithEmptyDepartment_ThrowsValidationException()
        {
            // Arrange
            var department = "";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetCoursesByDepartmentQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("Department is required"));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _controller.GetCoursesByDepartment(department));
        }
    }
}
