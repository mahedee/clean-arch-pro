using AutoMapper;
using EduTrack.Application.Features.Feedbacks.Dtos;
using EduTrack.Domain.Entities;

namespace EduTrack.Application.Features.Feedbacks.Mappings
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<Feedback, FeedbackDto>();
        }
    }
}
