using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thomas.TechTest.Data;
using Thomas.TechTest.API;
using Microsoft.EntityFrameworkCore;

namespace Thomas.TechTest.API
{
    public class CandidateRepository : ICandidateRepository
    {
        private CandidateDbContext _context;

        public CandidateRepository(CandidateDbContext context)
        {
            _context = context;
        }

        public Models.Candidate GetCandidate(Guid id)
        {
            var dbCandidate = _context.Candidates
                .Include(c => c.AptitudeAssessment)
                .Include(c => c.BehaviourAssessment)
                .SingleOrDefault(c => c.Id == id);
            if (dbCandidate == null)
            {
                return null;
            }
            return ConvertToModel(dbCandidate);
        }

        public IEnumerable<Models.Candidate> GetCandidates()
        {
            return _context.Candidates
                .Include(c => c.AptitudeAssessment)
                .Include(c => c.BehaviourAssessment)
                .Select(ConvertToModel);
        }

        private static Models.Candidate ConvertToModel(Candidate c)
        {
            return new Models.Candidate
            {
                Id = c.Id,
                RoleId = c.RoleId,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
                AptitudeAssessment = ConvertToModel(c.AptitudeAssessment),
                BehaviourAssessment = ConvertToModel(c.BehaviourAssessment),
            };
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

        private static Models.BehaviourAssessment ConvertToModel(Data.BehaviourAssessment ass)
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
