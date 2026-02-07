using MediatR;
using Microsoft.AspNetCore.Mvc;
using EduTrack.Application.Features.Students.Commands.CreateStudent;
using EduTrack.Application.Features.Students.Commands.UpdateStudent;
using EduTrack.Application.Features.Students.Commands.ChangeStudentStatus;
using EduTrack.Application.Features.Students.Queries.GetStudent;
using EduTrack.Application.Features.Students.Queries.GetStudentList;
using EduTrack.Application.Features.Students.Queries.GetStudentsByStatus;
using EduTrack.Application.Features.Students.Queries.GetStudentsOnProbation;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Domain.Entities;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedStudentListDto>> GetStudents([FromQuery] GetStudentListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentDto>> GetStudent(Guid id)
    {
        var result = await _mediator.Send(new GetStudentQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateStudent([FromBody] CreateStudentDto dto)
    {
        // Debug logging to check what's being received
        Console.WriteLine($"DEBUG: Received DTO FullName: '{dto.FullName}'");
        Console.WriteLine($"DEBUG: Received DTO Email: '{dto.Email}'");
        Console.WriteLine($"DEBUG: Received DTO PhoneNumber: '{dto.PhoneNumber}'");
        
        var command = new CreateStudentCommand(
            dto.FullName, dto.DateOfBirth, dto.Email, 
            dto.PhoneNumber, 
            dto.Address?.Street, dto.Address?.City, dto.Address?.State, 
            dto.Address?.ZipCode, dto.Address?.Country);
        
        Console.WriteLine($"DEBUG: Command FullName: '{command.FullName}'");
        
        var studentId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetStudent), new { id = studentId }, studentId);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentDto dto)
    {
        var command = new UpdateStudentCommand(
            id, dto.FullName, dto.Email, dto.PhoneNumber,
            dto.Address?.Street, dto.Address?.City, dto.Address?.State,
            dto.Address?.ZipCode, dto.Address?.Country, dto.GPA);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id:guid}/contact")]
    public async Task<ActionResult> UpdateStudentContact(Guid id, [FromBody] UpdateStudentContactDto dto)
    {
        var command = new UpdateStudentContactCommand(id, dto.Email, dto.PhoneNumber);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id:guid}/gpa")]
    public async Task<ActionResult> UpdateStudentGPA(Guid id, [FromBody] UpdateGPADto dto)
    {
        var command = new UpdateStudentGPACommand(id, dto.GPAValue);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult> ChangeStudentStatus(Guid id, [FromBody] ChangeStatusDto dto)
    {
        if (!Enum.TryParse<StudentStatus>(dto.NewStatus, true, out var newStatus))
        {
            return BadRequest($"Invalid status: {dto.NewStatus}");
        }
        
        var command = new ChangeStudentStatusCommand(id, newStatus);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<PaginatedStudentListDto>> GetStudentsByStatus(
        StudentStatus status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetStudentsByStatusQuery(status)
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("probation")]
    public async Task<ActionResult<PaginatedStudentListDto>> GetStudentsOnProbation(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetStudentsOnProbationQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}