using System;
using System.Collections.Generic;

namespace Thomas.TechTest.Data
{
    public class Candidate
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public ICollection<Assessment> Assessments { get; set; }
    }
}
