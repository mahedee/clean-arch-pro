using MediatR;
using EduTrack.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.EventHandlers.Students;

/// <summary>
/// Handles StudentStatusChangedEvent to perform status-related tasks
/// </summary>
public class StudentStatusChangedEventHandler : INotificationHandler<StudentStatusChangedEvent>
{
    private readonly ILogger<StudentStatusChangedEventHandler> _logger;

    public StudentStatusChangedEventHandler(ILogger<StudentStatusChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(StudentStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        // Security: log status enums and reason (no email or PII)
        _logger.LogInformation(
            "Student status changed. StudentId: {StudentId}, From: {PreviousStatus}, To: {NewStatus}, Reason: {Reason}",
            notification.StudentId,
            notification.PreviousStatus,
            notification.NewStatus,
            notification.Reason ?? "none");

        // TODO: Cancel enrollments (Inactive), disable portal (Inactive), send reactivation email (Active)
        await Task.CompletedTask;
    }
}
