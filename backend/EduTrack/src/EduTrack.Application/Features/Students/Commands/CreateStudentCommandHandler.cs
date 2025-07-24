using AutoMapper;
using EduTrack.Domain.Entities;
using EduTrack.Domain.Repositories;
using MediatR;

namespace EduTrack.Application.Features.Students.Commands
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateStudentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            var student = Student.Create(
                request.FullName,
                request.DateOfBirth,
                request.Email ?? string.Empty);

            await _unitOfWork.Students.AddAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return student.Id;
        }
    }
}
