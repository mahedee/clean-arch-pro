using MediatR;
using EduTrack.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.EventHandlers.Students;

/// <summary>
/// Handles StudentGPAUpdatedEvent to perform GPA-related tasks
/// </summary>
public class StudentGPAUpdatedEventHandler : INotificationHandler<StudentGPAUpdatedEvent>
{
    private readonly ILogger<StudentGPAUpdatedEventHandler> _logger;

    public StudentGPAUpdatedEventHandler(ILogger<StudentGPAUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(StudentGPAUpdatedEvent notification, CancellationToken cancellationToken)
    {
        // Security: GPA is academic record — log only at Debug so it is suppressed in production
        _logger.LogDebug(
            "Student GPA updated. StudentId: {StudentId}, IsFirstGPA: {IsFirstGPA}",
            notification.StudentId,
            notification.IsFirstGPA);

        // TODO: Check academic standing, update scholarship eligibility, notify advisors
        await Task.CompletedTask;
    }
}
