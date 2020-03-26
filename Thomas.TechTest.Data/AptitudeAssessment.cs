using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thomas.TechTest.Data
{
    public class AptitudeAssessment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime SentOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public int? TrainabilityIndex { get; set; }
    }
}
