using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CODE81_Assessment.Infrastructure.Repositories
{
    public class LibraryMemberRepository(AppDbContext context) : ILibraryMemberRepository
    {
        private readonly AppDbContext _context = context;

        #region CRUD

        public async Task<LibraryMember?> GetByIdAsync(int id, bool asNoTracking = true)
        {
            var query = _context.LibraryMembers.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(IEnumerable<LibraryMember>, int)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.LibraryMembers.AsNoTracking();

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }
        public async Task AddAsync(LibraryMember member)
        {
            await _context.LibraryMembers.AddAsync(member);
        }

        public void Update(LibraryMember member)
        {
            _context.LibraryMembers.Update(member);
        }

        public void Delete(LibraryMember member)
        {
            _context.LibraryMembers.Remove(member);
        }

        #endregion
    }
}
