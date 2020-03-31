using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thomas.TechTest.API.Models
{
    public class SearchResult
    {
        public IEnumerable<CandidateSummary> Results { get; set; }
        public int TotalRows { get; set; }
        public int TotalPages { get; set; }
    }
}
