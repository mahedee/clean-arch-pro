using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduTrack.Application.Features.Feedbacks.Commands.SubmitFeedback;
using EduTrack.Application.Features.Feedbacks.Commands.MarkFeedbackAsRead;
using EduTrack.Application.Features.Feedbacks.Queries.GetFeedbacks;
using EduTrack.Application.Features.Feedbacks.Dtos;
using EduTrack.Api.Models.Requests;

[ApiController]
[Route("api/[controller]")]
public class FeedbacksController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeedbacksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Submit feedback. Name is optional; leave it out to submit anonymously.
    /// </summary>
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<Guid>> SubmitFeedback([FromBody] SubmitFeedbackRequest request)
    {
        var command = new SubmitFeedbackCommand(request.Message, request.Name);
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetFeedbacks), new { }, id);
    }

    /// <summary>
    /// Get all feedback entries (admin only). Use ?unreadOnly=true to filter unread.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<FeedbackDto>>> GetFeedbacks([FromQuery] bool unreadOnly = false)
    {
        var result = await _mediator.Send(new GetFeedbacksQuery(unreadOnly));
        return Ok(result);
    }

    /// <summary>
    /// Mark a feedback entry as read (admin only).
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        await _mediator.Send(new MarkFeedbackAsReadCommand(id));
        return NoContent();
    }
}
