using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student graduates
    /// </summary>
    public class StudentGraduatedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string FullName { get; }
        public string Email { get; }
        public decimal? FinalGPA { get; }
        public DateTime GraduationDate { get; }
        public string? Degree { get; }
        public string? Major { get; }
        public string? Honors { get; }

        public StudentGraduatedEvent(
            Guid studentId,
            string fullName,
            string email,
            decimal? finalGPA = null,
            string? degree = null,
            string? major = null,
            string? honors = null)
        {
            StudentId = studentId;
            FullName = fullName;
            Email = email;
            FinalGPA = finalGPA;
            GraduationDate = DateTime.UtcNow;
            Degree = degree;
            Major = major;
            Honors = honors;
        }
    }
}
