using AutoMapper;
using EduTrack.Application.Features.Departments.DTOs;
using EduTrack.Domain.Entities;

namespace EduTrack.Application.Features.Departments.Mappings;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.ContactEmail != null ? src.ContactEmail.Value : null))
            .ForMember(dest => dest.ContactPhone, opt => opt.MapFrom(src => src.ContactPhone != null ? src.ContactPhone.Value : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Department, DepartmentListDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
