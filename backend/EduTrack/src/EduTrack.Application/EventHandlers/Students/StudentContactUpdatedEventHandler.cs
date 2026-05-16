using MediatR;
using EduTrack.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.EventHandlers.Students;

/// <summary>
/// Handles StudentContactUpdatedEvent to perform contact update tasks
/// </summary>
public class StudentContactUpdatedEventHandler : INotificationHandler<StudentContactUpdatedEvent>
{
    private readonly ILogger<StudentContactUpdatedEventHandler> _logger;

    public StudentContactUpdatedEventHandler(ILogger<StudentContactUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(StudentContactUpdatedEvent notification, CancellationToken cancellationToken)
    {
        // Security: contact updates involve PII — log only the student ID, never the new email/phone
        _logger.LogInformation(
            "Student contact information updated. StudentId: {StudentId}",
            notification.StudentId);

        // TODO: Update email in external systems (LMS, library), send verification email
        await Task.CompletedTask;
    }
}
