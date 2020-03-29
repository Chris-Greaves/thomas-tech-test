using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Thomas.TechTest.API;
using Thomas.TechTest.API.Controllers;
using Thomas.TechTest.API.Models;
using Thomas.TechTest.Data;
using AptitudeAssessment = Thomas.TechTest.API.Models.AptitudeAssessment;
using BehaviourAssessment = Thomas.TechTest.API.Models.BehaviourAssessment;
using Candidate = Thomas.TechTest.API.Models.Candidate;

namespace Thomas.TechTest.Tests
{
    [TestClass]
    public class CandidateControllerTests
    {
        private Mock<ILogger<CandidateController>> _logger = new Mock<ILogger<CandidateController>>();
        private Mock<ICandidateRepository> _repo = new Mock<ICandidateRepository>();
        private CandidateController _controller;

        public CandidateControllerTests()
        {
            _controller = new CandidateController(_logger.Object, _repo.Object);
        }

        [TestMethod]
        public void CanGetAllCandidates()
        {
            var candidates = CreateCandidates();
            _repo.Setup(r => r.GetCandidates()).Returns(candidates.AsEnumerable());

            var result = _controller.Get();
            var okResult = (OkObjectResult)result.Result;
            var resultValue = (IEnumerable<Candidate>)okResult.Value;

            _repo.Verify(r => r.GetCandidates(), Times.Once);
            Assert.AreEqual(candidates.Count(), resultValue.Count());
        }

        [TestMethod]
        public void CanGetCandidateById()
        {
            var candidateId = Guid.NewGuid();
            var candidate = new Candidate
            {
                Id = candidateId,
                RoleId = Guid.NewGuid(),
                Firstname = "Someone",
                Lastname = "Cool",
                AptitudeAssessment = new AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    TrainabilityIndex = 55
                },
                BehaviourAssessment = new BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    WorkingStrengths = "W"
                }
            };
            _repo.Setup(r => r.GetCandidate(candidateId)).Returns(candidate);

            var result = _controller.GetCandidate(candidateId);
            var okResult = (OkObjectResult)result.Result;
            var resultValue = (Candidate)okResult.Value;

            _repo.Verify(r => r.GetCandidate(candidateId), Times.Once);
            Assert.AreEqual(candidate.Id, resultValue.Id);
        }

        [TestMethod]
        public void WillReturnNotFoundWhenCandidateDoesNotExist()
        {
            var candidateId = Guid.NewGuid();
            _repo.Setup(r => r.GetCandidate(candidateId)).Returns((Candidate)null);

            var result = _controller.GetCandidate(candidateId);
            var notFoundResult = (NotFoundResult)result.Result;

            _repo.Verify(r => r.GetCandidate(candidateId), Times.Once);
        }

        private static List<Candidate> CreateCandidates()
        {
            var candidates = new List<Candidate>();
            candidates.Add(new Candidate
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                Firstname = "Someone",
                Lastname = "Cool",
                AptitudeAssessment = new AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    TrainabilityIndex = 55
                },
                BehaviourAssessment = new BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    WorkingStrengths = "W"
                }
            });
            candidates.Add(new Candidate
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                Firstname = "Someone",
                Lastname = "Else",
                AptitudeAssessment = new AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(8)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
                    TrainabilityIndex = 35
                },
                BehaviourAssessment = new BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    WorkingStrengths = "S"
                }
            });
            candidates.Add(new Candidate
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                Firstname = "Random",
                Lastname = "Guy",
                AptitudeAssessment = new AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(1)),
                    TrainabilityIndex = 42
                },
                BehaviourAssessment = new BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(7)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    WorkingStrengths = "WS"
                }
            });
            return candidates;
        }
    }
}
