using AutoMapper;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Domain.Entities;

namespace EduTrack.Application.Common;

public class StudentMappingProfile : Profile
{
    public StudentMappingProfile()
    {
        CreateMap<Student, StudentDto>();
    }
}
