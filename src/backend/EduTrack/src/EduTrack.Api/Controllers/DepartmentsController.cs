using EduTrack.Application.Features.Departments.Commands.CreateDepartment;
using EduTrack.Application.Features.Departments.Commands.UpdateDepartment;
using EduTrack.Application.Features.Departments.Commands.DeleteDepartment;
using EduTrack.Application.Features.Departments.Queries.GetDepartment;
using EduTrack.Application.Features.Departments.Queries.GetDepartmentList;
using EduTrack.Application.Features.Departments.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace EduTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepartmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedDepartmentListDto>> GetDepartments([FromQuery] GetDepartmentListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(Guid id)
    {
        var result = await _mediator.Send(new GetDepartmentQuery(id));
        if (result is null)
            return NotFound(new { Message = $"Department with ID '{id}' not found." });
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateDepartment([FromBody] CreateDepartmentDto dto)
    {
        try
        {
            var command = new CreateDepartmentCommand
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                Location = dto.Location,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone
            };
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetDepartment), new { id }, id);
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
    public async Task<ActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentDto dto)
    {
        try
        {
            var command = new UpdateDepartmentCommand
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description,
                Location = dto.Location,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone
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
    public async Task<ActionResult> DeleteDepartment(Guid id)
    {
        try
        {
            await _mediator.Send(new DeleteDepartmentCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}
