using System;
using System.Collections.Generic;

namespace Thomas.TechTest.API
{
    public interface ICandidateRepository
    {
        IEnumerable<Models.Candidate> GetCandidates();
        IEnumerable<Models.Candidate> GetCandidatesWithOutstandingAssessments();
        Models.Candidate GetCandidate(Guid id);
        Models.SearchResult SearchForCandidates(Models.SearchFilterOptions options);
    }
}
