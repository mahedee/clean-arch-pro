namespace EduTrack.Application.Features.Feedbacks.Dtos
{
    public class FeedbackDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
