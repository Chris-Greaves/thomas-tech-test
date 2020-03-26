using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thomas.TechTest.Data
{
    public class BehaviourAssessment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime SentOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string WorkingStrengths { get; set; }
    }
}
