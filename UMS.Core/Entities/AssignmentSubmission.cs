using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
     public class AssignmentSubmission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTime SubmittedAt { get; set; }

        public Assignments Assignment { get; set; }
        public Student Student { get; set; }
    }
}
