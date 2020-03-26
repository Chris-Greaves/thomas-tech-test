using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thomas.TechTest.Data
{
    public class Candidate
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [ForeignKey("BehaviourAssessment")]
        public long BehaviourAssessmentId { get; set; }
        public BehaviourAssessment BehaviourAssessment { get; set; }

        [ForeignKey("AptitudeAssessment")]
        public long AptitudeAssessmentId { get; set; }
        public AptitudeAssessment AptitudeAssessment { get; set; }
    }
}
