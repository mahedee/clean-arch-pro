using EduTrack.Application.Features.Teachers.Commands.CreateTeacher;
using EduTrack.Application.Features.Teachers.Commands.UpdateTeacher;
using EduTrack.Application.Features.Teachers.Commands.DeleteTeacher;
using EduTrack.Application.Features.Teachers.Queries.GetTeacher;
using EduTrack.Application.Features.Teachers.Queries.GetTeacherList;
using EduTrack.Application.Features.Teachers.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace EduTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeachersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedTeacherListDto>> GetTeachers([FromQuery] GetTeacherListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TeacherDto>> GetTeacher(Guid id)
    {
        var result = await _mediator.Send(new GetTeacherQuery(id));
        if (result is null)
            return NotFound(new { Message = $"Teacher with ID '{id}' not found." });
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTeacher([FromBody] CreateTeacherDto dto)
    {
        try
        {
            var command = new CreateTeacherCommand
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                EmployeeId = dto.EmployeeId,
                Department = dto.Department,
                Title = dto.Title,
                DateOfBirth = dto.DateOfBirth
            };
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTeacher), new { id }, id);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { Property = e.PropertyName, Message = e.ErrorMessage });
            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateTeacher(Guid id, [FromBody] UpdateTeacherDto dto)
    {
        try
        {
            var command = new UpdateTeacherCommand
            {
                Id = id,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Department = dto.Department,
                Title = dto.Title,
                OfficeLocation = dto.OfficeLocation,
                OfficeHours = dto.OfficeHours
            };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { Property = e.PropertyName, Message = e.ErrorMessage });
            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTeacher(Guid id)
    {
        try
        {
            await _mediator.Send(new DeleteTeacherCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}
