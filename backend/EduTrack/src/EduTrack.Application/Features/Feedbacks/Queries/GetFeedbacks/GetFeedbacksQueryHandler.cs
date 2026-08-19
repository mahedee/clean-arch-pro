using AutoMapper;
using EduTrack.Application.Features.Feedbacks.Dtos;
using EduTrack.Domain.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.Features.Feedbacks.Queries.GetFeedbacks;

public class GetFeedbacksQueryHandler : IRequestHandler<GetFeedbacksQuery, List<FeedbackDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetFeedbacksQueryHandler> _logger;

    public GetFeedbacksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetFeedbacksQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<FeedbackDto>> Handle(GetFeedbacksQuery request, CancellationToken cancellationToken)
    {
        var feedbacks = request.UnreadOnly
            ? await _unitOfWork.Feedbacks.GetUnreadAsync(cancellationToken)
            : await _unitOfWork.Feedbacks.GetAllAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} feedback entries (unreadOnly={UnreadOnly})", feedbacks.Count, request.UnreadOnly);

        return _mapper.Map<List<FeedbackDto>>(feedbacks);
    }
}
