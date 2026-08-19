using FluentValidation;

namespace EduTrack.Application.Features.Feedbacks.Commands.SubmitFeedback;

public class SubmitFeedbackCommandValidator : AbstractValidator<SubmitFeedbackCommand>
{
    public SubmitFeedbackCommandValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Feedback message is required.")
            .Length(1, 2000).WithMessage("Feedback message must be between 1 and 2000 characters.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Name));
    }
}
