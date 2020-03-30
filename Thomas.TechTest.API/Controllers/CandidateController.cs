using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Thomas.TechTest.API.Models;
using Candidate = Thomas.TechTest.API.Models.Candidate;

namespace Thomas.TechTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        [Route("{id}")]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        [ProducesResponseType(typeof(ActionResult<Candidate>), 200)]
        public ActionResult<Candidate> GetCandidate(Guid id)
        {
            var candidate = _repo.GetCandidate(id);
            if (candidate == null)
            {
                return NotFound();
            }
            return Ok(candidate);
        }

        [HttpGet]
        [Route("/all/uncompleted")]
        [ProducesResponseType(typeof(ActionResult<Candidate>), 200)]
        public ActionResult<IEnumerable<Candidate>> GetCandidatesWithOutstandingAssessments()
        {
            return Ok(_repo.GetCandidatesWithOutstandingAssessments());
        }

        [HttpPost]
        [Route("/search")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<CandidateSummary>>), 200)]
        public ActionResult<IEnumerable<CandidateSummary>> GetCandidatesWithFilterOptions(SearchFilterOptions options)
        {
            return Ok(_repo.SearchForCandidates(options));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Candidate>> Get()
        {
            return Ok(_repo.GetCandidates());
        }
    }
}
