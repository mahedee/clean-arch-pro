using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a new user account is created
    /// </summary>
    public class UserCreatedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Username { get; }
        public string Email { get; }

        public UserCreatedEvent(Guid userId, string username, string email)
        {
            UserId = userId;
            Username = username;
            Email = email;
        }
    }
}
