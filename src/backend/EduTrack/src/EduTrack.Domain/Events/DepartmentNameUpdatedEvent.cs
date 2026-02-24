using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a department name is updated
    /// </summary>
    public class DepartmentNameUpdatedEvent : DomainEvent
    {
        public Guid DepartmentId { get; }
        public string PreviousName { get; }
        public string NewName { get; }

        public DepartmentNameUpdatedEvent(Guid departmentId, string previousName, string newName)
        {
            DepartmentId = departmentId;
            PreviousName = previousName;
            NewName = newName;
        }
    }
}