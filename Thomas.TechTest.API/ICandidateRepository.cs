using System.Collections.Generic;

namespace Thomas.TechTest.API
{
    public interface ICandidateRepository
    {
        IEnumerable<Models.Candidate> GetCandidates();
    }
}
