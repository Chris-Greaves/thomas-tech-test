using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thomas.TechTest.Data
{
    public class Assessment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("AssignedCandidate")]
        public Guid AssignedCandidateId { get; set; }
        public Candidate AssignedCandidate { get; set; }

        public AssessmentType AssessmentType { get; set; }
        public DateTime SentOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public int? TrainabilityIndex { get; set; }
    }
}
