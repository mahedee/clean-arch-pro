using MediatR;
using EduTrack.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.EventHandlers.Students;

/// <summary>
/// Handles StudentAcademicStandingChangedEvent to perform academic standing tasks
/// </summary>
public class StudentAcademicStandingChangedEventHandler : INotificationHandler<StudentAcademicStandingChangedEvent>
{
    private readonly ILogger<StudentAcademicStandingChangedEventHandler> _logger;

    public StudentAcademicStandingChangedEventHandler(ILogger<StudentAcademicStandingChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(StudentAcademicStandingChangedEvent notification, CancellationToken cancellationToken)
    {
        // Security: log standing labels and reason — GPA value is academic record, suppress at Info level
        _logger.LogInformation(
            "Student academic standing changed. StudentId: {StudentId}, From: {PreviousStanding}, To: {NewStanding}, Reason: {Reason}",
            notification.StudentId,
            notification.PreviousStanding,
            notification.NewStanding,
            notification.Reason ?? "none");

        // TODO: Probation → restrict course load; Dean's List → update honours record; Suspension → disable enrollment
        await Task.CompletedTask;
    }
}
