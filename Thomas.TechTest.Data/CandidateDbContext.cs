using Microsoft.EntityFrameworkCore;

namespace Thomas.TechTest.Data
{
    public class CandidateDbContext : DbContext
    {
        public CandidateDbContext(DbContextOptions<CandidateDbContext> options) : base(options) { }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
    }
}
