using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a department head is removed
    /// </summary>
    public class DepartmentHeadRemovedEvent : DomainEvent
    {
        public Guid DepartmentId { get; }
        public string DepartmentName { get; }
        public Guid PreviousHeadId { get; }

        public DepartmentHeadRemovedEvent(Guid departmentId, string departmentName, Guid previousHeadId)
        {
            DepartmentId = departmentId;
            DepartmentName = departmentName;
            PreviousHeadId = previousHeadId;
        }
    }
}