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
        
        [ForeignKey("Candidate")]
        public Guid CandidateId { get; set; }
        public Candidate Candidate { get; set; }

        public AssessmentType AssessmentType { get; set; }
        public DateTime SentOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public int? TrainabilityIndex { get; set; }
        public string WorkingStrengths { get; set; }
    }
}
