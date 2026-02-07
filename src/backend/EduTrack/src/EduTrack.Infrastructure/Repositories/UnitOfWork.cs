using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Infrastructure.Data;

namespace EduTrack.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IStudentRepository? _studentRepository;
        public IStudentRepository Students => _studentRepository ??= new StudentRepository(_context);

        private ICourseRepository? _courseRepository;
        public ICourseRepository Courses => _courseRepository ??= new CourseRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
