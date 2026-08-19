using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace EduTrack.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(ApplicationDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<UnitOfWork>();
        }

        private IStudentRepository? _studentRepository;
        public IStudentRepository Students => _studentRepository ??= new StudentRepository(_context, _loggerFactory.CreateLogger<StudentRepository>());

        private ICourseRepository? _courseRepository;
        public ICourseRepository Courses => _courseRepository ??= new CourseRepository(_context, _loggerFactory.CreateLogger<CourseRepository>());

        private ITeacherRepository? _teacherRepository;
        public ITeacherRepository Teachers => _teacherRepository ??= new TeacherRepository(_context, _loggerFactory.CreateLogger<TeacherRepository>());

        private IDepartmentRepository? _departmentRepository;
        public IDepartmentRepository Departments => _departmentRepository ??= new DepartmentRepository(_context, _loggerFactory.CreateLogger<DepartmentRepository>());

        private IFeedbackRepository? _feedbackRepository;
        public IFeedbackRepository Feedbacks => _feedbackRepository ??= new FeedbackRepository(_context, _loggerFactory.CreateLogger<FeedbackRepository>());

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Persisting pending changes to database...");
            var count = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Persisted {ChangeCount} changes to database", count);
            return count;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
