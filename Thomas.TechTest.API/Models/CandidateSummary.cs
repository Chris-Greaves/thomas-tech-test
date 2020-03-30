using System;

namespace Thomas.TechTest.API.Models
{
    public class CandidateSummary
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}