namespace EduTrack.Api.Models.Requests;

public record SubmitFeedbackRequest(string Message, string? Name = null);
