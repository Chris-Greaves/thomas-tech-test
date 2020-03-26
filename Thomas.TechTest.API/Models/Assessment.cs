using System;

namespace Thomas.TechTest.API.Models
{
    public class Assessment
    {
        public DateTime SentOn { get; set; }
        public DateTime? CompletedOn { get; set; }
    }
}