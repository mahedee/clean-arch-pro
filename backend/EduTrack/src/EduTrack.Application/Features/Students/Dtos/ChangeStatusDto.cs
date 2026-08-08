using EduTrack.Domain.Enums;

namespace EduTrack.Application.Features.Students.DTOs;

public class ChangeStatusDto
{
    public StudentStatus NewStatus { get; set; }
}
