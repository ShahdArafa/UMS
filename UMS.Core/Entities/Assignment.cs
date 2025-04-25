using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    // -----------------------
    //  AssignmentQuiz
    // -----------------------
    public class Assignments

    {
        [Key]
        public int Id { get; set; }

        // Link to an enrollment
        public int? EnrollmentId { get; set; }

        public double? Grade { get; set; }

        public int CourseGroupId { get; set; }
        public string? FilePath { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Deadline { get; set; }

        public string WeekNumber { get; set; }


        // Navigation
        [ForeignKey(nameof(EnrollmentId))]
        public virtual Enrollment Enrollment { get; set; }

        [ForeignKey(nameof(CourseGroupId))]
        public CourseGroup CourseGroup { get; set; }
    }
}
