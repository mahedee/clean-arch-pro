using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a grade is published and made visible to students
    /// </summary>
    public class GradePublishedEvent : DomainEvent
    {
        public Guid GradeId { get; }
        public Guid StudentId { get; }
        public Guid CourseId { get; }
        public decimal Score { get; }
        public string? LetterGrade { get; }

        public GradePublishedEvent(Guid gradeId, Guid studentId, Guid courseId, decimal score, string? letterGrade)
        {
            GradeId = gradeId;
            StudentId = studentId;
            CourseId = courseId;
            Score = score;
            LetterGrade = letterGrade;
        }
    }
}
