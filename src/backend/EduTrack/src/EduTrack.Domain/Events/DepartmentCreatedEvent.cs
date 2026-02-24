using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a new department is created
    /// </summary>
    public class DepartmentCreatedEvent : DomainEvent
    {
        public Guid DepartmentId { get; }
        public string Name { get; }
        public string Code { get; }

        public DepartmentCreatedEvent(Guid departmentId, string name, string code)
        {
            DepartmentId = departmentId;
            Name = name;
            Code = code;
        }
    }
}