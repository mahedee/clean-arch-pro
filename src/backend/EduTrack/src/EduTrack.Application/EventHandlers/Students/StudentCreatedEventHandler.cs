using MediatR;
using EduTrack.Domain.Events;

namespace EduTrack.Application.EventHandlers.Students;

/// <summary>
/// Handles StudentCreatedEvent to perform post-creation tasks
/// </summary>
public class StudentCreatedEventHandler : INotificationHandler<StudentCreatedEvent>
{
    public async Task Handle(StudentCreatedEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Add business logic such as:
        // - Send welcome email to student
        // - Create student portal account
        // - Assign default course recommendations
        // - Notify academic advisors
        // - Create audit log entry

        await Task.CompletedTask;
    }
}
