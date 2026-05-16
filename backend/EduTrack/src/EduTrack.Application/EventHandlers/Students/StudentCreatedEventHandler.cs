using MediatR;
using EduTrack.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.EventHandlers.Students;

/// <summary>
/// Handles StudentCreatedEvent to perform post-creation tasks
/// </summary>
public class StudentCreatedEventHandler : INotificationHandler<StudentCreatedEvent>
{
    private readonly ILogger<StudentCreatedEventHandler> _logger;

    public StudentCreatedEventHandler(ILogger<StudentCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(StudentCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Security: log only the student ID (GUID) — never email, name, or other PII
        _logger.LogInformation(
            "Student created. StudentId: {StudentId}",
            notification.StudentId);

        // TODO: Send welcome email, create student portal account, assign advisors
        await Task.CompletedTask;
    }
}
