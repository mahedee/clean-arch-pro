using MediatR;

namespace EduTrack.Application.Features.Feedbacks.Commands.MarkFeedbackAsRead;

/// <summary>
/// Command to mark a feedback entry as read.
/// </summary>
public record MarkFeedbackAsReadCommand(Guid Id) : IRequest;
