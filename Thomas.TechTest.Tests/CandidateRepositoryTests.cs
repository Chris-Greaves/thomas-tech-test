using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using Thomas.TechTest.API;
using Thomas.TechTest.API.Models;
using Thomas.TechTest.Data;

namespace Thomas.TechTest.Tests
{
    [TestClass]
    public class CandidateRepositoryTests
    {
        private CandidateDbContext _context;
        CandidateRepository _repo;

        public CandidateRepositoryTests()
        {
            var contextOptions = new DbContextOptionsBuilder<CandidateDbContext>()
                .UseSqlite("Data Source = test-database.db")
                .Options;
            _context = new CandidateDbContext(contextOptions);
            _repo = new CandidateRepository(_context);

            EnsureTestDataExists();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
            File.Delete("test-database.db");
        }

        [TestMethod]
        public void CanRetrieveACandidateById()
        {
            var dbCandidate = _context.Candidates
                .Include(c => c.AptitudeAssessment)
                .Include(c => c.BehaviourAssessment)
                .First();

            var result = _repo.GetCandidate(dbCandidate.Id);

            Assert.AreEqual(dbCandidate.Id, result.Id);
            Assert.AreEqual(dbCandidate.Firstname, result.Firstname);
            Assert.AreEqual(dbCandidate.Lastname, result.Lastname);
            Assert.AreEqual(dbCandidate.RoleId, result.RoleId);
            Assert.AreEqual(dbCandidate.AptitudeAssessment.SentOn, result.AptitudeAssessment.SentOn);
            Assert.AreEqual(dbCandidate.AptitudeAssessment.CompletedOn, result.AptitudeAssessment.CompletedOn);
            Assert.AreEqual(dbCandidate.AptitudeAssessment.TrainabilityIndex, result.AptitudeAssessment.TrainabilityIndex);
            Assert.AreEqual(dbCandidate.BehaviourAssessment.SentOn, result.BehaviourAssessment.SentOn);
            Assert.AreEqual(dbCandidate.BehaviourAssessment.CompletedOn, result.BehaviourAssessment.CompletedOn);
            Assert.AreEqual(dbCandidate.BehaviourAssessment.WorkingStrengths, result.BehaviourAssessment.WorkingStrengths);
        }

        [TestMethod]
        public void WillReturnNullIfNoCandidateFound()
        {
            var result = _repo.GetCandidate(Guid.NewGuid());

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void CanReceiveAllCandidates()
        {
            var ids = _context.Candidates.Select(c => c.Id);

            var result = _repo.GetCandidates();

            foreach (var id in ids)
            {
                Assert.IsTrue(result.Any(c => c.Id == id)); 
            }
        }

        [TestMethod]
        public void CanReceiveAllCandidatesWithOutstandingAssessmentsOnly()
        {
            var ids = _context.Candidates.Where(c => c.AptitudeAssessment.CompletedOn == null || c.BehaviourAssessment.CompletedOn == null).Select(c => c.Id);

            var result = _repo.GetCandidatesWithOutstandingAssessments();

            Assert.AreEqual(ids.Count(), result.Count());
            foreach (var id in ids)
            {
                Assert.IsTrue(result.Any(c => c.Id == id));
            }
        }

        [TestMethod]
        public void CanRetrieveAllCandidatesUsingSearch()
        {
            var options = new SearchFilterOptions
            {
                NameSearchString = ""
            };

            var result = _repo.SearchForCandidates(options);

            Assert.AreEqual(_context.Candidates.Count(), result.Count());
        }

        [TestMethod]
        public void CanRetrieveSpecificCandidatesUsingFirstnameInNameSearch()
        {
            var randy = _context.Candidates.Single(c => c.Firstname == "Randy");

            var options = new SearchFilterOptions
            {
                NameSearchString = "Randy"
            };

            var result = _repo.SearchForCandidates(options);

            Assert.AreEqual(randy.Id, result.First().Id);
        }

        [TestMethod]
        public void CanRetrieveSpecificCandidatesUsingPartialFirstnameInNameSearch()
        {
            var randy = _context.Candidates.Single(c => c.Firstname == "Randy");

            var options = new SearchFilterOptions
            {
                NameSearchString = "Ran"
            };

            var result = _repo.SearchForCandidates(options);

            Assert.AreEqual(randy.Id, result.First().Id);
        }

        [TestMethod]
        public void CanRetrieveSpecificCandidatesUsingCaseInsensitiveFirstnameInNameSearch()
        {
            var randy = _context.Candidates.Single(c => c.Firstname == "Randy");

            var options = new SearchFilterOptions
            {
                NameSearchString = "rAnDY"
            };

            var result = _repo.SearchForCandidates(options);

            Assert.AreEqual(randy.Id, result.First().Id);
        }

        [TestMethod]
        public void CanRetrieveMultipleCandidatesUsingNameSearch()
        {
            var ids = _context.Candidates.Where(c => c.Firstname.Contains("Chri")).Select(c => c.Id);

            var options = new SearchFilterOptions
            {
                NameSearchString = "chri"
            };

            var result = _repo.SearchForCandidates(options);

            Assert.AreEqual(ids.Count(), result.Count());
            foreach (var id in ids)
            {
                Assert.IsTrue(result.Any(r=>r.Id == id)); 
            }
        }

        private void EnsureTestDataExists()
        {
            _context.Database.EnsureCreated();

            // Model Candidate
            _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "Someone",
                Lastname = "Cool",
                BehaviourAssessment = new Data.BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(8)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    WorkingStrengths = "WS"
                },
                AptitudeAssessment = new Data.AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(8)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(7)),
                    TrainabilityIndex = 54
                }
            });

            // Candidate with outstanding Behaviour Assessment
            _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "ajsdnfksa",
                Lastname = "dlfjsdlkf",
                BehaviourAssessment = new Data.BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(16)),
                },
                AptitudeAssessment = new Data.AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(14)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(11)),
                    TrainabilityIndex = 54
                }
            });

            // Candidate with outstanding Aptitude Assessment
            _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "sdfljsodifjs",
                Lastname = "dsflksdlkfks",
                BehaviourAssessment = new Data.BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    WorkingStrengths = "S"
                },
                AptitudeAssessment = new Data.AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                }
            });

            // Candidate with Specific Name to be found be Search functionality
            _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "Randy",
                Lastname = "Dillion",
                BehaviourAssessment = new Data.BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    WorkingStrengths = "S"
                },
                AptitudeAssessment = new Data.AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(9)),
                    TrainabilityIndex = 34
                }
            });

            // Candidates with generic Names to be found be Search functionality
            _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "Chris",
                Lastname = "Awesome",
                BehaviourAssessment = new Data.BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    WorkingStrengths = "S"
                },
                AptitudeAssessment = new Data.AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(9)),
                    TrainabilityIndex = 34
                }
            });
            _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "Christ",
                Lastname = "The one",
                BehaviourAssessment = new Data.BehaviourAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    WorkingStrengths = "S"
                },
                AptitudeAssessment = new Data.AptitudeAssessment
                {
                    SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                    CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(9)),
                    TrainabilityIndex = 34
                }
            });
            _context.SaveChanges();
        }
    }
}
