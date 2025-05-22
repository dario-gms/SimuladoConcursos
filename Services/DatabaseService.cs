using SimuladoConcursos.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimuladoConcursos.Data;

namespace SimuladoConcursos.Services
{
    public class DatabaseService
    {
        private readonly AppDbContext _context;

        public DatabaseService()
        {
            _context = new AppDbContext();
            _context.Database.EnsureCreated();
        }

        public async Task AddQuestionAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            return await _context.Questions
                .Include(q => q.Opcoes)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.Opcoes)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}