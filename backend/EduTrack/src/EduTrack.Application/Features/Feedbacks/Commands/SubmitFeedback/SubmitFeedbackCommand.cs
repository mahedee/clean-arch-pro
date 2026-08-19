using MediatR;

namespace EduTrack.Application.Features.Feedbacks.Commands.SubmitFeedback;

/// <summary>
/// Command to submit a new feedback entry. Name is optional (null = anonymous).
/// </summary>
public record SubmitFeedbackCommand(
    string Message,
    string? Name = null
) : IRequest<Guid>;
