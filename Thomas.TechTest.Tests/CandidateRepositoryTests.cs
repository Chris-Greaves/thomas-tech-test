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
                .Include(c => c.Assessments)
                .First();
            var aptitudeAssessment = dbCandidate.Assessments.First(a => a.AssessmentType == AssessmentType.Aptitude);
            var behaviourAssessment = dbCandidate.Assessments.First(a => a.AssessmentType == AssessmentType.Behaviour);

            var result = _repo.GetCandidate(dbCandidate.Id);

            Assert.AreEqual(dbCandidate.Id, result.Id);
            Assert.AreEqual(dbCandidate.Firstname, result.Firstname);
            Assert.AreEqual(dbCandidate.Lastname, result.Lastname);
            Assert.AreEqual(dbCandidate.RoleId, result.RoleId);
            Assert.AreEqual(aptitudeAssessment.SentOn, result.AptitudeAssessment.SentOn);
            Assert.AreEqual(aptitudeAssessment.CompletedOn, result.AptitudeAssessment.CompletedOn);
            Assert.AreEqual(aptitudeAssessment.TrainabilityIndex, result.AptitudeAssessment.TrainabilityIndex);
            Assert.AreEqual(behaviourAssessment.SentOn, result.BehaviourAssessment.SentOn);
            Assert.AreEqual(behaviourAssessment.CompletedOn, result.BehaviourAssessment.CompletedOn);
            Assert.AreEqual(behaviourAssessment.WorkingStrengths, result.BehaviourAssessment.WorkingStrengths);
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
            var ids = _context.Candidates.Where(c => c.Assessments.Any(a => a.CompletedOn == null)).Select(c => c.Id);

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

            Assert.AreEqual(_context.Candidates.Count(), result.Results.Count());
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

            Assert.AreEqual(randy.Id, result.Results.First().Id);
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

            Assert.AreEqual(randy.Id, result.Results.First().Id);
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

            Assert.AreEqual(randy.Id, result.Results.First().Id);
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

            Assert.AreEqual(ids.Count(), result.Results.Count());
            foreach (var id in ids)
            {
                Assert.IsTrue(result.Results.Any(r => r.Id == id));
            }
        }

        [TestMethod]
        public void CanSelectPageOfResultsUsingSearch()
        {
            var expectedTotal = _context.Candidates.Count();
            var options = new SearchFilterOptions
            {
                Page = 2,
                ResultsPerPage = 3
            };

            var result = _repo.SearchForCandidates(options);

            Assert.AreEqual(3, result.Results.Count());
            Assert.AreEqual(expectedTotal, result.TotalRows);
        }

        [TestMethod]
        public void NumberOfTotalPagesIsCorrect()
        {
            var expectedTotal = _context.Candidates.Count();
            var options = new SearchFilterOptions
            {
                Page = 2,
                ResultsPerPage = 3
            };

            var result = _repo.SearchForCandidates(options);

            Assert.AreEqual(3, result.Results.Count());
            Assert.AreEqual(expectedTotal, result.TotalRows);

            var totalPagesRoundDown = expectedTotal / 3;
            var totalPagesRemainder = expectedTotal % 3;
            if (totalPagesRemainder > 0)
            {
                Assert.AreEqual(totalPagesRoundDown + 1, result.TotalPages);
            }
            else
            {
                Assert.AreEqual(totalPagesRoundDown, result.TotalPages);
            }
        }

        [TestMethod]
        public void CanFilterForCandidatesWithSentAptitudeTests()
        {
            var candidateWithAptitudeAssessment = _context.Candidates.First(c => c.Assessments.Any(a => a.AssessmentType == AssessmentType.Aptitude));
            var options = new SearchFilterOptions
            {
                AssessmentsToFilterFor = new string[]
                {
                    "Aptitude"
                }
            };

            var result = _repo.SearchForCandidates(options);

            Assert.IsTrue(result.Results.Any(r => r.Id == candidateWithAptitudeAssessment.Id));
        }

        [TestMethod]
        public void CanFilterForCandidatesWithSentBehaviourTests()
        {
            var candidateWithBehaviourAssessment = _context.Candidates.First(c => c.Assessments.Any(a => a.AssessmentType == AssessmentType.Behaviour));
            var options = new SearchFilterOptions
            {
                AssessmentsToFilterFor = new string[]
                {
                    "Behaviour"
                }
            };

            var result = _repo.SearchForCandidates(options);

            Assert.IsTrue(result.Results.Any(r => r.Id == candidateWithBehaviourAssessment.Id));
        }

        [TestMethod]
        public void CandidateWithoutAptitudeDoesNotAppearWhenFilteredForAptitude()
        {
            var candidateWithoutBehaviourAssessment = _context.Candidates.First(c => !c.Assessments.Any(a => a.AssessmentType == AssessmentType.Aptitude));
            var options = new SearchFilterOptions
            {
                AssessmentsToFilterFor = new string[]
                {
                    "Aptitude"
                }
            };

            var result = _repo.SearchForCandidates(options);

            Assert.IsTrue(!result.Results.Any(r => r.Id == candidateWithoutBehaviourAssessment.Id));
        }

        [TestMethod]
        public void CandidateWithoutBehaviourDoesNotAppearWhenFilteredForBehaviour()
        {
            var candidateWithoutBehaviourAssessment = _context.Candidates.First(c => !c.Assessments.Any(a => a.AssessmentType == AssessmentType.Behaviour));
            var options = new SearchFilterOptions
            {
                AssessmentsToFilterFor = new string[]
                {
                    "Behaviour"
                }
            };

            var result = _repo.SearchForCandidates(options);

            Assert.IsTrue(!result.Results.Any(r => r.Id == candidateWithoutBehaviourAssessment.Id));
        }

        private void EnsureTestDataExists()
        {
            _context.Database.EnsureCreated();

            // Model Candidate
            var candidate = _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "Someone",
                Lastname = "Cool"
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Behaviour,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(8)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                WorkingStrengths = "WS",
                Candidate = candidate.Entity
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Aptitude,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(8)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(7)),
                TrainabilityIndex = 54,
                Candidate = candidate.Entity
            });

            // Candidate with outstanding Behaviour Assessment
            candidate = _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "ajsdnfksa",
                Lastname = "dlfjsdlkf"
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Behaviour,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(16)),
                Candidate = candidate.Entity
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Aptitude,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(14)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(11)),
                TrainabilityIndex = 54,
                Candidate = candidate.Entity
            });

            // Candidate with outstanding Aptitude Assessment
            candidate = _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "sdfljsodifjs",
                Lastname = "dsflksdlkfks",
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Behaviour,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                WorkingStrengths = "S",
                Candidate = candidate.Entity
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Aptitude,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                Candidate = candidate.Entity
            });

            // Candidate with Specific Name to be found be Search functionality
            candidate = _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "Randy",
                Lastname = "Dillion"
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Behaviour,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                WorkingStrengths = "S",
                Candidate = candidate.Entity
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Aptitude,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(9)),
                TrainabilityIndex = 34,
                Candidate = candidate.Entity
            });

            // Candidates with generic Names to be found be Search functionality
            candidate = _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "Chris",
                Lastname = "Awesome"
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Behaviour,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                WorkingStrengths = "S",
                Candidate = candidate.Entity
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Aptitude,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(9)),
                TrainabilityIndex = 34,
                Candidate = candidate.Entity
            });
            candidate = _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "Christ",
                Lastname = "The one"
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Behaviour,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                WorkingStrengths = "S",
                Candidate = candidate.Entity
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Aptitude,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(9)),
                TrainabilityIndex = 34,
                Candidate = candidate.Entity
            });

            // Candidate with No Aptitude Assessment
            candidate = _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "No",
                Lastname = "Personality"
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Behaviour,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                WorkingStrengths = "S",
                Candidate = candidate.Entity
            });

            // Candidate with No Behaviour Assessment
            candidate = _context.Candidates.Add(new Data.Candidate
            {
                RoleId = Guid.NewGuid(),
                Firstname = "Bad",
                Lastname = "Behaviour"
            });
            _context.Assessments.Add(new Data.Assessment
            {
                AssessmentType = AssessmentType.Aptitude,
                SentOn = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                CompletedOn = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                TrainabilityIndex = 65,
                Candidate = candidate.Entity
            });

            _context.SaveChanges();
        }
    }
}
