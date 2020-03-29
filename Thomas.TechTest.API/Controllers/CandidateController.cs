﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        public ActionResult<IEnumerable<Candidate>> Get()
        {
            return Ok(_repo.GetCandidates());
        }
    }
}
