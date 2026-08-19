using EduTrack.Domain.Common.Exceptions;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.Features.Feedbacks.Commands.MarkFeedbackAsRead;

public class MarkFeedbackAsReadCommandHandler : IRequestHandler<MarkFeedbackAsReadCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarkFeedbackAsReadCommandHandler> _logger;

    public MarkFeedbackAsReadCommandHandler(IUnitOfWork unitOfWork, ILogger<MarkFeedbackAsReadCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(MarkFeedbackAsReadCommand request, CancellationToken cancellationToken)
    {
        var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(request.Id, cancellationToken);
        if (feedback is null)
            throw new EntityNotFoundException(nameof(Feedback), request.Id);

        feedback.MarkAsRead();
        _unitOfWork.Feedbacks.Update(feedback);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Feedback {FeedbackId} marked as read", request.Id);
    }
}
