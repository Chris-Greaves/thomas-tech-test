using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using Thomas.TechTest.API.Controllers;
using Thomas.TechTest.Data;

namespace Thomas.TechTest.Tests
{
    [TestClass]
    public class CandidateControllerTests
    {
        private DbContextOptions<CandidateDbContext> _contextOptions;
        private Mock<ILogger<CandidateController>> _logger = new Mock<ILogger<CandidateController>>();

        public CandidateControllerTests()
        {
            _contextOptions = new DbContextOptionsBuilder<CandidateDbContext>()
            .UseInMemoryDatabase(databaseName: "MovieListDatabase")
            .Options;
        }

        [TestMethod]
        public void CanGetAllCandidates()
        {
            //using (var context = new CandidateDbContext(_contextOptions))
            //{
            //    var canId = Guid.NewGuid();
            //    context.Candidates.Add(new Candidate
            //    {
            //        Id = canId,
            //        Firstname = "Steve",
            //        Surname = "Woods",
            //        Assessments = new[]
            //        {
            //            new BehaviourAssessment
            //            {
            //                Id = 1,
            //                AssignedCandidateId = canId,
            //                AssessmentType = AssessmentType.Aptitude,
            //                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
            //                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
            //                TrainabilityIndex = 50
            //            },
            //            new BehaviourAssessment
            //            {
            //                Id = 2,
            //                AssignedCandidateId = canId,
            //                AssessmentType = AssessmentType.Behaviour,
            //                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
            //                CompletedOn = null,
            //                TrainabilityIndex = null
            //            }
            //        }
            //    });
            //    canId = Guid.NewGuid();
            //    context.Candidates.Add(new Candidate
            //    {
            //        Id = canId,
            //        Firstname = "Colin",
            //        Surname = "Henwood",
            //        Assessments = new[]
            //        {
            //            new BehaviourAssessment
            //            {
            //                Id = 3,
            //                AssignedCandidateId = canId,
            //                AssessmentType = AssessmentType.Aptitude,
            //                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
            //                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
            //                TrainabilityIndex = 35
            //            },
            //            new BehaviourAssessment
            //            {
            //                Id = 4,
            //                AssignedCandidateId = canId,
            //                AssessmentType = AssessmentType.Behaviour,
            //                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
            //                CompletedOn = null,
            //                TrainabilityIndex = null
            //            }
            //        }
            //    });
            //    context.SaveChanges();
            //}

            //// Use a clean instance of the context to run the test
            //using (var context = new CandidateDbContext(_contextOptions))
            //{
            //    var controller = new CandidateController(_logger.Object, context);
            //    var candidates = controller.Get();

            //    Assert.AreEqual(2, candidates.Count());
            //}
        }
    }
}
