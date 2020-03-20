using System;

namespace Thomas.TechTest.API.Models
{
    public class Candidate
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public AssessmentGroup[] Assessments { get; set; }
    }
}