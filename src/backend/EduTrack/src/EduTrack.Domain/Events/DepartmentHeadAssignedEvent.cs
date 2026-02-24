using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a department head is assigned
    /// </summary>
    public class DepartmentHeadAssignedEvent : DomainEvent
    {
        public Guid DepartmentId { get; }
        public string DepartmentName { get; }
        public Guid? PreviousHeadId { get; }
        public Guid NewHeadId { get; }

        public DepartmentHeadAssignedEvent(Guid departmentId, string departmentName, Guid? previousHeadId, Guid newHeadId)
        {
            DepartmentId = departmentId;
            DepartmentName = departmentName;
            PreviousHeadId = previousHeadId;
            NewHeadId = newHeadId;
        }
    }
}