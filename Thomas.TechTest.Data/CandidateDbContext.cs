using Microsoft.EntityFrameworkCore;

namespace Thomas.TechTest.Data
{
    public class CandidateDbContext : DbContext
    {
        public CandidateDbContext(DbContextOptions<CandidateDbContext> options) : base(options) { }

        public DbSet<Candidate> Candidates { get; set; }

        // Spliting Assessments into individual tables so they can be individually extended if needed
        public DbSet<BehaviourAssessment> BehaviourAssessments { get; set; }
        public DbSet<AptitudeAssessment> AptitudeAssessments { get; set; }
    }
}
