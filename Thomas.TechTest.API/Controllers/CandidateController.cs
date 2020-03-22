using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Thomas.TechTest.Data;
using Candidate = Thomas.TechTest.API.Models.Candidate;
using Assessment = Thomas.TechTest.API.Models.Assessment;
using Thomas.TechTest.API.Models;

namespace Thomas.TechTest.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly ILogger<CandidateController> _logger;
        private readonly CandidateDbContext _context;

        public CandidateController(ILogger<CandidateController> logger, CandidateDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Candidate> Get()
        {
            return _context.Candidates.Select(c => new Candidate
            {
                Id = c.Id,
                Firstname = c.Firstname,
                Surname = c.Surname,
                Assessments = new[]{ new AssessmentGroup
                    {
                        Behaviour = ConvertDbToModel(c.Assessments.SingleOrDefault(a => a.AssessmentType == AssessmentType.Behaviour && a.AssignedCandidateId == c.Id)),
                        Aptitude = ConvertDbToModel(c.Assessments.SingleOrDefault(a => a.AssessmentType == AssessmentType.Aptitude && a.AssignedCandidateId == c.Id))
                    }
                }
            });
        }

        private static Assessment ConvertDbToModel(Data.Assessment assessment)
        {
            return new Assessment
            {
                SentOn = assessment.SentOn,
                CompletedOn = assessment.CompletedOn,
                TrainabilityIndex = assessment.TrainabilityIndex
            };
        }
    }
}
