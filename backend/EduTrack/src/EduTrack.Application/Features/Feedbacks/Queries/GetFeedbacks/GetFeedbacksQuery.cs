using EduTrack.Application.Features.Feedbacks.Dtos;
using MediatR;

namespace EduTrack.Application.Features.Feedbacks.Queries.GetFeedbacks;

/// <summary>
/// Query to retrieve feedback entries. Set UnreadOnly = true to get only unread feedback.
/// </summary>
public record GetFeedbacksQuery(bool UnreadOnly = false) : IRequest<List<FeedbackDto>>;
