using System;

namespace Thomas.TechTest.API.Models
{
    public class Candidate
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public BehaviourAssessment BehaviourAssessment { get; set; }
        public AptitudeAssessment AptitudeAssessment { get; set; }
    }
}