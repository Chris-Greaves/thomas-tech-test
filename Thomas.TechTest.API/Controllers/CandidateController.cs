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
        private readonly ICandidateRepository _repo;

        public CandidateController(ILogger<CandidateController> logger, ICandidateRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpGet]
        public IEnumerable<Candidate> Get()
        {
            return _repo.GetCandidates();
        }
    }
}
