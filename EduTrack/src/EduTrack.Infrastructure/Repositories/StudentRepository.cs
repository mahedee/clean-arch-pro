using EduTrack.Domain.Entities;
using EduTrack.Infrastructure.Data;
using EduTrack.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Students.ToListAsync(cancellationToken);
        }

        public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
        {
            await _context.Students.AddAsync(student, cancellationToken);
        }

        public void Update(Student student)
        {
            _context.Students.Update(student);
        }

        public void Delete(Student student)
        {
            _context.Students.Remove(student);
        }
    }
}
