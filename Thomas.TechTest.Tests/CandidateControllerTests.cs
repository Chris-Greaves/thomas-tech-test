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

        [TestMethod]
        public void CanRetrieveJustTheCandidatesThatHaveOutstandingAssessments()
        {
            var candidates = CreateCandidates();
            candidates.Add(new Candidate
            {
                Id = new Guid("495c0cbb-91e6-4e9a-bab3-80661c07199f"),
                RoleId = Guid.NewGuid(),
                Firstname = "Late",
                Lastname = "Finisher1",
                AptitudeAssessment = new AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                },
                BehaviourAssessment = new BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
                    WorkingStrengths = "WS"
                }
            });
            candidates.Add(new Candidate
            {
                Id = new Guid("777e0f68-d90c-46e7-a473-57c2233ee99e"),
                RoleId = Guid.NewGuid(),
                Firstname = "Late",
                Lastname = "Finisher2",
                AptitudeAssessment = new AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(1)),
                    TrainabilityIndex = 24
                },
                BehaviourAssessment = new BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                }
            });
            _repo.Setup(r => r.GetCandidatesWithOutstandingAssessments()).Returns(candidates.Where(c => c.AptitudeAssessment.CompletedOn == null || c.BehaviourAssessment.CompletedOn == null));

            var result = _controller.GetCandidatesWithOutstandingAssessments();
            var okResult = (OkObjectResult)result.Result;
            var resultValue = (IEnumerable<Candidate>)okResult.Value;

            _repo.Verify(r => r.GetCandidatesWithOutstandingAssessments(), Times.Once);
            Assert.AreEqual(2, resultValue.Count());
        }

        [TestMethod]
        public void WillReturnAnEmptyArrayWhenNoCandidates()
        {
            _repo.Setup(r => r.GetCandidatesWithOutstandingAssessments()).Returns(new Candidate[0].AsEnumerable());

            var result = _controller.GetCandidatesWithOutstandingAssessments();
            var okResult = (OkObjectResult)result.Result;
            var resultValue = (IEnumerable<Candidate>)okResult.Value;

            _repo.Verify(r => r.GetCandidatesWithOutstandingAssessments(), Times.Once);
            Assert.AreEqual(0, resultValue.Count());
        }

        [TestMethod]
        public void CanRetrieveAllCandidatesUsingSearch()
        {
            var candidates = CreateCandidateSummaries();
            var options = new SearchFilterOptions
            {
                NameSearchString = ""
            };
            _repo.Setup(r => r.SearchForCandidates(options)).Returns(candidates);

            var result = _controller.GetCandidatesWithFilterOptions(options);
            var okResult = (OkObjectResult)result.Result;
            var resultValue = (IEnumerable<CandidateSummary>)okResult.Value;

            _repo.Verify(r => r.SearchForCandidates(options), Times.Once);
            Assert.AreEqual(candidates.Count(), resultValue.Count());
        }

        [TestMethod]
        public void WillReturnAnEmptyArrayInsteadOf404WhenNoCandidatesMatchStringSearch()
        {
            var options = new SearchFilterOptions
            {
                NameSearchString = "Does not exist"
            };
            _repo.Setup(r => r.SearchForCandidates(options)).Returns(new List<CandidateSummary>().AsEnumerable());

            var result = _controller.GetCandidatesWithFilterOptions(options);
            var okResult = (OkObjectResult)result.Result;
            var resultValue = (IEnumerable<CandidateSummary>)okResult.Value;

            _repo.Verify(r => r.SearchForCandidates(options), Times.Once);
            Assert.AreEqual(0, resultValue.Count());
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

        private static List<CandidateSummary> CreateCandidateSummaries()
        {
            var candidates = new List<CandidateSummary>();
            candidates.Add(new CandidateSummary
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                Firstname = "Someone",
                Lastname = "Cool"
            });
            candidates.Add(new CandidateSummary
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                Firstname = "Someone",
                Lastname = "Else"
            });
            candidates.Add(new CandidateSummary
            {
                Id = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                Firstname = "Random",
                Lastname = "Guy"
            });
            return candidates;
        }
    }
}
