using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student's phone number is updated
    /// </summary>
    public class StudentPhoneNumberUpdatedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string FullName { get; }
        public string Email { get; }
        public string? PreviousPhoneNumber { get; }
        public string? NewPhoneNumber { get; }
        public bool PhoneNumberRemoved { get; }

        public StudentPhoneNumberUpdatedEvent(
            Guid studentId,
            string fullName,
            string email,
            string? previousPhoneNumber,
            string? newPhoneNumber)
        {
            StudentId = studentId;
            FullName = fullName;
            Email = email;
            PreviousPhoneNumber = previousPhoneNumber;
            NewPhoneNumber = newPhoneNumber;
            PhoneNumberRemoved = !string.IsNullOrEmpty(previousPhoneNumber) && string.IsNullOrEmpty(newPhoneNumber);
        }
    }
}
