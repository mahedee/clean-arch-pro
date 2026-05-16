using AutoMapper;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Domain.Entities;
using EduTrack.Domain.ValueObjects;

namespace EduTrack.Application.Common;

public class StudentMappingProfile : Profile
{
    public StudentMappingProfile()
    {
        // Student to DTOs mappings
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber != null ? src.PhoneNumber.Value : null))
            .ForMember(dest => dest.GPA, opt => opt.MapFrom(src => src.CurrentGPA != null ? src.CurrentGPA.Value : (decimal?)null))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<Student, StudentListDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber != null ? src.PhoneNumber.Value : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // Address mapping
        CreateMap<Address, AddressDto>()
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));
    }
}
