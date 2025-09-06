using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a new teacher is created
    /// </summary>
    public class TeacherCreatedEvent : DomainEvent
    {
        public Guid TeacherId { get; }
        public string FullName { get; }
        public string Email { get; }
        public string EmployeeId { get; }

        public TeacherCreatedEvent(Guid teacherId, string fullName, string email, string employeeId)
        {
            TeacherId = teacherId;
            FullName = fullName;
            Email = email;
            EmployeeId = employeeId;
        }
    }
}
