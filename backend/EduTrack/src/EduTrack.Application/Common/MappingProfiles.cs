using AutoMapper;
using EduTrack.Application.Features.Students.Dtos;
using EduTrack.Domain.Entities;

namespace EduTrack.Application.Common
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Student, StudentDto>();
        }
    }
}
