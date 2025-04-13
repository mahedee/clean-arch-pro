using EduTrack.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace EduTrack.Application.Features.Students.Commands
{
    public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.Id);

            if (student == null)
                return false;

            student.FullName = request.FullName;
            student.DateOfBirth = request.DateOfBirth;
            student.Email = request.Email;

            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
