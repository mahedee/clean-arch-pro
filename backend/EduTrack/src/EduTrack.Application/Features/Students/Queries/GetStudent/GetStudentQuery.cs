using MediatR;
using EduTrack.Application.Features.Students.DTOs;

namespace EduTrack.Application.Features.Students.Queries.GetStudent;

/// <summary>
/// Query to get a single student by ID
/// </summary>
public class GetStudentQuery : IRequest<StudentDto?>
{
    /// <summary>
    /// Student ID to retrieve
    /// </summary>
    public Guid StudentId { get; set; }

    public GetStudentQuery(Guid studentId)
    {
        StudentId = studentId;
    }
}