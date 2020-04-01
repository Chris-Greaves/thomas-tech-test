using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Thomas.TechTest.API.Models;
using Thomas.TechTest.Data;

namespace Thomas.TechTest.API
{
    public class CandidateRepository : ICandidateRepository
    {
        private CandidateDbContext _context;

        public CandidateRepository(CandidateDbContext context)
        {
            _context = context;
        }

        public Models.Candidate GetCandidate(Guid id)
        {
            var dbCandidate = _context.Candidates
                .Include(c => c.Assessments)
                .SingleOrDefault(c => c.Id == id);
            if (dbCandidate == null)
            {
                return null;
            }
            return ConvertToModel(dbCandidate);
        }

        public IEnumerable<Models.Candidate> GetCandidates()
        {
            return _context.Candidates
                .Include(c => c.Assessments)
                .Select(ConvertToModel);
        }

        public IEnumerable<Models.Candidate> GetCandidatesWithOutstandingAssessments()
        {
            return _context.Candidates
                .Include(c => c.Assessments)
                .Where(c => c.Assessments.Any(a => a.CompletedOn == null))
                .OrderBy(c => c.Firstname).ThenBy(c => c.Lastname)
                .Select(ConvertToModel);
        }

        // Keeping for reference in case needed for future search funtionality
        //public IEnumerable<Models.CandidateSummary> SearchForCandidates(Models.SearchFilterOptions options)
        //{
        //    var expression = Expression.Parameter(typeof(Candidate));
        //    var searchStringConstant = Expression.Constant(options.NameSearchString);

        //    // Firstname Search Expression
        //    var firstnameExProp = Expression.Property(expression, "Firstname");
        //    var firstnameEx = Expression.Call(firstnameExProp, "Contains", null, searchStringConstant);

        //    // Lastname Search Expression
        //    var lastnameExProp = Expression.Property(expression, "Lastname");
        //    var lastnameEx = Expression.Call(lastnameExProp, "Contains", null, searchStringConstant);

        //    var or = Expression.OrElse(firstnameEx, lastnameEx);

        //    var lambda = Expression.Lambda<Func<Candidate, bool>>(or, expression);

        //    return _context.Candidates
        //        .Where(lambda)
        //        .Select(ConvertToModelSummary);
        //}

        public Models.SearchResult SearchForCandidates(Models.SearchFilterOptions options)
        {
            var returnObj = new Models.SearchResult();
            var candidates = _context.Candidates.AsQueryable();

            if (!string.IsNullOrEmpty(options.NameSearchString))
            {
                candidates = candidates.Where(
                    c => c.Firstname.ToLower().Contains(options.NameSearchString.ToLower()) ||
                    c.Lastname.ToLower().Contains(options.NameSearchString.ToLower()));
            }

            if (options.AssessmentsToFilterFor != null && options.AssessmentsToFilterFor.Count() > 0)
            {
                if (options.AssessmentsToFilterFor.Contains("Aptitude"))
                {
                    candidates = candidates.Where(c => c.Assessments.Any(a => a.AssessmentType == AssessmentType.Aptitude));
                }

                if (options.AssessmentsToFilterFor.Contains("Behaviour"))
                {
                    candidates = candidates.Where(c => c.Assessments.Any(a => a.AssessmentType == AssessmentType.Behaviour));
                }
            }

            returnObj.TotalRows = candidates.Count();

            if (options.Page.HasValue && options.ResultsPerPage.HasValue)
            {
                returnObj.TotalPages = (returnObj.TotalRows + (options.ResultsPerPage.Value - 1)) / options.ResultsPerPage.Value; // Work out number of total pages using integer division

                candidates = candidates
                    .Skip((options.Page.Value - 1) * options.ResultsPerPage.Value)
                    .Take(options.ResultsPerPage.Value);
            }

            returnObj.Results = candidates
                .OrderBy(c => c.Firstname)
                .ThenBy(c => c.Lastname)
                .Select(ConvertToModelSummary);
            return returnObj;
        }


        private static Models.Candidate ConvertToModel(Data.Candidate c)
        {
            return new Models.Candidate
            {
                Id = c.Id,
                RoleId = c.RoleId,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
                AptitudeAssessment = GetlatestAptitudeAssessment(c.Assessments),
                BehaviourAssessment = GetlatestBehaviourAssessment(c.Assessments),
            };
        }

        private static Models.CandidateSummary ConvertToModelSummary(Data.Candidate c)
        {
            return new Models.CandidateSummary
            {
                Id = c.Id,
                RoleId = c.RoleId,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
            };
        }

        private static AptitudeAssessment GetlatestAptitudeAssessment(ICollection<Data.Assessment> assessments)
        {
            var latestAptAssessment = assessments.OrderBy(a => a.SentOn).FirstOrDefault(a => a.AssessmentType == AssessmentType.Aptitude);
            if (latestAptAssessment == null)
            {
                return null;
            }

            return new AptitudeAssessment
            {
                SentOn = latestAptAssessment.SentOn,
                CompletedOn = latestAptAssessment.CompletedOn,
                TrainabilityIndex = latestAptAssessment.TrainabilityIndex,
            };
        }

        private static BehaviourAssessment GetlatestBehaviourAssessment(ICollection<Data.Assessment> assessments)
        {
            var latestBehAssessment = assessments.OrderBy(a => a.SentOn).FirstOrDefault(a => a.AssessmentType == AssessmentType.Behaviour);
            if (latestBehAssessment == null)
            {
                return null;
            }

            return new BehaviourAssessment
            {
                SentOn = latestBehAssessment.SentOn,
                CompletedOn = latestBehAssessment.CompletedOn,
                WorkingStrengths = latestBehAssessment.WorkingStrengths,
            };
        }
    }
}
