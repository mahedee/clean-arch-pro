using MediatR;
using EduTrack.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.EventHandlers.Students;

/// <summary>
/// Handles StudentGraduatedEvent to perform graduation-related tasks
/// </summary>
public class StudentGraduatedEventHandler : INotificationHandler<StudentGraduatedEvent>
{
    private readonly ILogger<StudentGraduatedEventHandler> _logger;

    public StudentGraduatedEventHandler(ILogger<StudentGraduatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(StudentGraduatedEvent notification, CancellationToken cancellationTask)
    {
        // Security: log degree/major/honors (non-sensitive academic metadata) but NOT final GPA or email
        _logger.LogInformation(
            "Student graduated. StudentId: {StudentId}, Degree: {Degree}, Major: {Major}, Honors: {Honors}",
            notification.StudentId,
            notification.Degree ?? "N/A",
            notification.Major ?? "N/A",
            notification.Honors ?? "none");

        // TODO: Generate diploma, update alumni database, disable student portal access
        await Task.CompletedTask;
    }
}
