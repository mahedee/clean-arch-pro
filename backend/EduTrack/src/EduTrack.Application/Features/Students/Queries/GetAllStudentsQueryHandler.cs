using AutoMapper;
using EduTrack.Application.Features.Students.Dtos;
using EduTrack.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace EduTrack.Application.Features.Students.Queries
{
    public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, List<StudentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllStudentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _unitOfWork.Students.GetAllAsync(cancellationToken);

            // Using AutoMapper to map students to StudentDto
            return _mapper.Map<List<StudentDto>>(students);
        }
    }
}
