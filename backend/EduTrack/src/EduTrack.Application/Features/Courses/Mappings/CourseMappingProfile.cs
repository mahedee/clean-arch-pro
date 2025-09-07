using AutoMapper;
using EduTrack.Domain.Entities;
using EduTrack.Application.Features.Courses.DTOs;
using EduTrack.Application.Features.Courses.Commands.CreateCourse;
using EduTrack.Application.Features.Courses.Commands.UpdateCourse;

namespace EduTrack.Application.Features.Courses.Mappings;

/// <summary>
/// AutoMapper profile for Course entity mappings
/// </summary>
public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        // Entity to DTO mappings
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Course, CourseListDto>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Enrollment, opt => opt.MapFrom(src => $"{src.CurrentEnrollment}/{src.MaxEnrollment}"))
            .ForMember(dest => dest.AcademicPeriod, opt => opt.MapFrom(src => $"{src.Semester} {src.AcademicYear}"));

        // DTO to Command mappings
        CreateMap<CreateCourseDto, CreateCourseCommand>();
        CreateMap<UpdateCourseDto, UpdateCourseCommand>();

        // Command to Entity mappings (if needed for direct mapping)
        CreateMap<CreateCourseCommand, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => Enum.Parse<CourseLevel>(src.Level, true)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CourseStatus.Draft))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.CourseCode))
            .ForMember(dest => dest.CreditHours, opt => opt.MapFrom(src => src.Credits))
            .ForMember(dest => dest.MaxEnrollment, opt => opt.MapFrom(src => src.MaxCapacity))
            .ForMember(dest => dest.CurrentEnrollment, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.PrerequisiteCreditHours, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(src => DateTime.Now.Year))
            .ForMember(dest => dest.StartDate, opt => opt.Ignore())
            .ForMember(dest => dest.EndDate, opt => opt.Ignore());
    }
}
