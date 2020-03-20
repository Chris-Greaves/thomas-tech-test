using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Thomas.TechTest.API.Models;

namespace Thomas.TechTest.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly ILogger<CandidateController> _logger;

        public CandidateController(ILogger<CandidateController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Candidate> Get()
        {
            return null;
        }
    }
}
