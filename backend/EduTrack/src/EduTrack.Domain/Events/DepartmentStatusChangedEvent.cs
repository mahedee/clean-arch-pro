using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;
using EduTrack.Domain.Entities;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when department status changes
    /// </summary>
    public class DepartmentStatusChangedEvent : DomainEvent
    {
        public Guid DepartmentId { get; }
        public string DepartmentName { get; }
        public DepartmentStatus PreviousStatus { get; }
        public DepartmentStatus NewStatus { get; }
        public string? Reason { get; }

        public DepartmentStatusChangedEvent(Guid departmentId, string departmentName, DepartmentStatus previousStatus, DepartmentStatus newStatus, string? reason)
        {
            DepartmentId = departmentId;
            DepartmentName = departmentName;
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
            Reason = reason;
        }
    }
}