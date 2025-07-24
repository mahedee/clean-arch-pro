using EduTrack.Domain.Repositories;
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

            // Use domain methods to update the student
            student.UpdateFullName(request.FullName);
            student.UpdateContactInformation(request.Email ?? string.Empty);
            
            // Note: DateOfBirth is not changeable after creation in this domain model
            // This is a business rule - if needed, we could add a specific method for correction

            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
