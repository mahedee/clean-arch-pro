using AutoMapper;
using EduTrack.Application.Features.Teachers.DTOs;
using EduTrack.Domain.Entities;

namespace EduTrack.Application.Features.Teachers.Mappings;

public class TeacherProfile : Profile
{
    public TeacherProfile()
    {
        CreateMap<Teacher, TeacherDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber != null ? src.PhoneNumber.Value : null))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Teacher, TeacherListDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
