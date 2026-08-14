using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduTrack.Application.Features.Students.Commands.CreateStudent;
using EduTrack.Application.Features.Students.Commands.UpdateStudent;
using EduTrack.Application.Features.Students.Commands.DeleteStudent;
using EduTrack.Application.Features.Students.Commands.ChangeStudentStatus;
using EduTrack.Application.Features.Students.Queries.GetStudent;
using EduTrack.Application.Features.Students.Queries.GetStudentList;
using EduTrack.Application.Features.Students.Queries.GetStudentsByStatus;
using EduTrack.Application.Features.Students.Queries.GetStudentsOnProbation;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Domain.Enums;

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

    [Authorize]
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> CreateStudent([FromBody] CreateStudentDto dto)
    {
        var command = new CreateStudentCommand(
            dto.FullName, dto.DateOfBirth, dto.Email,
            dto.PhoneNumber,
            dto.Address?.Street, dto.Address?.City, dto.Address?.State,
            dto.Address?.ZipCode, dto.Address?.Country);

        var studentId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetStudent), new { id = studentId }, studentId);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentDto dto)
    {
        var command = new UpdateStudentCommand(
            id, dto.FullName, dto.Email, dto.PhoneNumber,
            dto.Address?.Street, dto.Address?.City, dto.Address?.State,
            dto.Address?.ZipCode, dto.Address?.Country, dto.GPA);
        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize]
    [HttpPut("{id:guid}/contact")]
    [Authorize]
    public async Task<ActionResult> UpdateStudentContact(Guid id, [FromBody] UpdateStudentContactDto dto)
    {
        var command = new UpdateStudentContactCommand(id, dto.Email, dto.PhoneNumber);
        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize]
    [HttpPut("{id:guid}/gpa")]
    [Authorize]
    public async Task<ActionResult> UpdateStudentGPA(Guid id, [FromBody] UpdateGPADto dto)
    {
        var command = new UpdateStudentGPACommand(id, dto.GPAValue);
        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize]
    [HttpPut("{id:guid}/status")]
    [Authorize]
    public async Task<ActionResult> ChangeStudentStatus(Guid id, [FromBody] ChangeStatusDto dto)
    {
        var command = new ChangeStudentStatusCommand(id, dto.NewStatus);
        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> DeleteStudent(Guid id)
    {
        var command = new DeleteStudentCommand(id);
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