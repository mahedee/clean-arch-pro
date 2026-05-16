using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when department contact information is updated
    /// </summary>
    public class DepartmentContactUpdatedEvent : DomainEvent
    {
        public Guid DepartmentId { get; }
        public string DepartmentName { get; }
        public string? ContactEmail { get; }
        public string? ContactPhone { get; }

        public DepartmentContactUpdatedEvent(Guid departmentId, string departmentName, string? contactEmail, string? contactPhone)
        {
            DepartmentId = departmentId;
            DepartmentName = departmentName;
            ContactEmail = contactEmail;
            ContactPhone = contactPhone;
        }
    }
}