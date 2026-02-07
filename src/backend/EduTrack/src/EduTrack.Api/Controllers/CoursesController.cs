using EduTrack.Application.Features.Courses.Commands.CreateCourse;
using EduTrack.Application.Features.Courses.Commands.UpdateCourse;
using EduTrack.Application.Features.Courses.Commands.ScheduleCourse;
using EduTrack.Application.Features.Courses.Commands.ActivateCourse;
using EduTrack.Application.Features.Courses.Commands.CompleteCourse;
using EduTrack.Application.Features.Courses.Queries.GetCourse;
using EduTrack.Application.Features.Courses.Queries.GetCourseList;
using EduTrack.Application.Features.Courses.Queries.GetCoursesByDepartment;
using EduTrack.Application.Features.Courses.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace EduTrack.Api.Controllers;

/// <summary>
/// Course management API controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    /// <param name="command">Course creation details</param>
    /// <returns>Created course ID</returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCourse([FromBody] CreateCourseCommand command)
    {
        try
        {
            // MediatR automatically triggers ValidationBehavior before CreateCourseCommandHandler
            // 1. ValidationBehavior finds CreateCourseCommandValidator
            // 2. Runs validation rules
            // 3. If validation fails -> throws ValidationException
            // 4. If validation passes -> continues to CreateCourseCommandHandler
            var courseId = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetCourse), new { id = courseId }, courseId);
        }
        catch (ValidationException ex)
        {
            // Convert validation errors to BadRequest response
            var errors = ex.Errors.Select(error => new
            {
                Property = error.PropertyName,
                Message = error.ErrorMessage,
                AttemptedValue = error.AttemptedValue
            });

            return BadRequest(new
            {
                Message = "Validation failed",
                Errors = errors
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing course
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="command">Course update details</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseCommand command)
    {
        try
        {
            command.CourseId = id;
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound(new { Message = "Course not found" });
                
            return NoContent();
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(error => new
            {
                Property = error.PropertyName,
                Message = error.ErrorMessage
            });

            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }
    }

    /// <summary>
    /// Schedule a course
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="command">Schedule details</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/schedule")]
    public async Task<ActionResult> ScheduleCourse(Guid id, [FromBody] ScheduleCourseCommand command)
    {
        try
        {
            command.CourseId = id;
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound(new { Message = "Course not found" });
                
            return Ok(new { Message = "Course scheduled successfully" });
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(error => new
            {
                Property = error.PropertyName,
                Message = error.ErrorMessage
            });

            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Activate a course for enrollment
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/activate")]
    public async Task<ActionResult> ActivateCourse(Guid id)
    {
        try
        {
            var command = new ActivateCourseCommand { CourseId = id };
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound(new { Message = "Course not found" });
                
            return Ok(new { Message = "Course activated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Mark a course as completed
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="command">Completion details</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult> CompleteCourse(Guid id, [FromBody] CompleteCourseCommand command)
    {
        try
        {
            command.CourseId = id;
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound(new { Message = "Course not found" });
                
            return Ok(new { Message = "Course completed successfully" });
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(error => new
            {
                Property = error.PropertyName,
                Message = error.ErrorMessage
            });

            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }
    }

    /// <summary>
    /// Get all courses with pagination and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search term for title or code</param>
    /// <param name="department">Department filter</param>
    /// <param name="level">Level filter</param>
    /// <param name="status">Status filter</param>
    /// <param name="sortBy">Sort field (default: Title)</param>
    /// <param name="sortDirection">Sort direction (default: asc)</param>
    /// <returns>Paginated course list</returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedCourseListDto>> GetCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? department = null,
        [FromQuery] string? level = null,
        [FromQuery] string? status = null,
        [FromQuery] string sortBy = "Title",
        [FromQuery] string sortDirection = "asc")
    {
        try
        {
            var query = new GetCourseListQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Department = department,
                Level = level,
                Status = status,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(error => new
            {
                Property = error.PropertyName,
                Message = error.ErrorMessage
            });

            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }
    }

    /// <summary>
    /// Get course by ID
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Course details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
    {
        try
        {
            var query = new GetCourseQuery(id);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound(new { Message = "Course not found" });
                
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(error => new
            {
                Property = error.PropertyName,
                Message = error.ErrorMessage
            });

            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }
    }

    /// <summary>
    /// Get courses by department
    /// </summary>
    /// <param name="department">Department name</param>
    /// <param name="level">Optional level filter</param>
    /// <param name="status">Optional status filter</param>
    /// <returns>List of courses in the department</returns>
    [HttpGet("department/{department}")]
    public async Task<ActionResult<List<CourseListDto>>> GetCoursesByDepartment(
        string department,
        [FromQuery] string? level = null,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = new GetCoursesByDepartmentQuery(department)
            {
                Level = level,
                Status = status
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(error => new
            {
                Property = error.PropertyName,
                Message = error.ErrorMessage
            });

            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }
    }
}
