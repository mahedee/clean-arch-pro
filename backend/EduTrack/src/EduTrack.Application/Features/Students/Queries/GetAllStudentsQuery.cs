using EduTrack.Application.Features.Students.Dtos;
using MediatR;

namespace EduTrack.Application.Features.Students.Queries
{
    public class GetAllStudentsQuery : IRequest<List<StudentDto>>
    {
        // You can add any query parameters here in the future (e.g., filtering, paging)
    }
}
