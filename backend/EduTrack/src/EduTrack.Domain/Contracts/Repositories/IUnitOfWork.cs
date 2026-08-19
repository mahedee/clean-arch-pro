namespace EduTrack.Domain.Contracts.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository Students { get; }
        ICourseRepository Courses { get; }
        ITeacherRepository Teachers { get; }
        IDepartmentRepository Departments { get; }
        IFeedbackRepository Feedbacks { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}