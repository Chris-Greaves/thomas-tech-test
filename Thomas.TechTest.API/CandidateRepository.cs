using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .Include(c => c.AptitudeAssessment)
                .Include(c => c.BehaviourAssessment)
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
                .Include(c => c.AptitudeAssessment)
                .Include(c => c.BehaviourAssessment)
                .Select(ConvertToModel);
        }

        public IEnumerable<Models.Candidate> GetCandidatesWithOutstandingAssessments()
        {
            return _context.Candidates
                .Include(c => c.AptitudeAssessment)
                .Include(c => c.BehaviourAssessment)
                .Where(c => c.BehaviourAssessment.CompletedOn == null || c.AptitudeAssessment.CompletedOn == null)
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

            returnObj.TotalRows = candidates.Count();

            if (options.Page.HasValue && options.ResultsPerPage.HasValue)
            {
                returnObj.TotalPages = (returnObj.TotalRows + (options.ResultsPerPage.Value - 1)) / options.ResultsPerPage.Value; // Work out number of total pages using integer division

                candidates = candidates
                    .Skip((options.Page.Value - 1) * options.ResultsPerPage.Value)
                    .Take(options.ResultsPerPage.Value);
            }

            returnObj.Results = candidates.Select(ConvertToModelSummary);
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
                AptitudeAssessment = ConvertToModel(c.AptitudeAssessment),
                BehaviourAssessment = ConvertToModel(c.BehaviourAssessment),
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

        private static Models.AptitudeAssessment ConvertToModel(Data.AptitudeAssessment ass)
        {
            if (ass == null)
            {
                return null;
            }

            return new Models.AptitudeAssessment
            {
                SentOn = ass.SentOn,
                CompletedOn = ass.CompletedOn,
                TrainabilityIndex = ass.TrainabilityIndex
            };
        }

        private static Models.BehaviourAssessment ConvertToModel(Data.BehaviourAssessment ass)
        {
            if (ass == null)
            {
                return null;
            }

            return new Models.BehaviourAssessment
            {
                SentOn = ass.SentOn,
                CompletedOn = ass.CompletedOn,
                WorkingStrengths = ass.WorkingStrengths
            };
        }
    }
}
