using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thomas.TechTest.Data;
using Thomas.TechTest.API;

namespace Thomas.TechTest.API
{
    public class CandidateRepository : ICandidateRepository
    {
        private CandidateDbContext _context;

        public CandidateRepository(CandidateDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Models.Candidate> GetCandidates()
        {
            return _context.Candidates.Select(c => new Models.Candidate
            {
                Id = c.Id,
                RoleId = c.RoleId,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
                AptitudeAssessment = ConvertToModel(c.AptitudeAssessment),
                BehaviourAssessment = ConvertToModel(c.BehaviourAssessment),
            });
        }

        private static Models.AptitudeAssessment ConvertToModel(AptitudeAssessment ass)
        {
            return new Models.AptitudeAssessment
            {
                SentOn = ass.SentOn,
                CompletedOn = ass.CompletedOn,
                TrainabilityIndex = ass.TrainabilityIndex
            };
        }

        private static Models.BehaviourAssessment ConvertToModel(BehaviourAssessment ass)
        {
            return new Models.BehaviourAssessment
            {
                SentOn = ass.SentOn,
                CompletedOn = ass.CompletedOn,
                WorkingStrengths = ass.WorkingStrengths
            };
        }
    }
}
