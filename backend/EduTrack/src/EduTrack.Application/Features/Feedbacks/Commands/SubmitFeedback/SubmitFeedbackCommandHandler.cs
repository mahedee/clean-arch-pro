using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.Features.Feedbacks.Commands.SubmitFeedback;

public class SubmitFeedbackCommandHandler : IRequestHandler<SubmitFeedbackCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitFeedbackCommandHandler> _logger;

    public SubmitFeedbackCommandHandler(IUnitOfWork unitOfWork, ILogger<SubmitFeedbackCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
    {
        var feedback = Feedback.Create(request.Message, request.Name);

        await _unitOfWork.Feedbacks.AddAsync(feedback, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Feedback submitted with ID {FeedbackId}", feedback.Id);

        return feedback.Id;
    }
}
