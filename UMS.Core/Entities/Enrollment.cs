using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    // -----------------------
    //  Enrollment
    // -----------------------
    public class Enrollment

    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int? CourseGroupId { get; set; }

        public DateTime Date { get; set; }

        // Navigation
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }


        [ForeignKey(nameof(CourseGroupId))]
        public CourseGroup CourseGroup { get; set; }

        public virtual ICollection<Attendance> Attendances { get; set; }
            = new HashSet<Attendance>();

        // If it's one-to-one with assignment info
        public virtual ICollection<Assignments> Assignments { get; set; } = new HashSet<Assignments>();

        //new 4/10
        public virtual ICollection<Quiz> Quizzes { get; set; } =
            new HashSet<Quiz>();
    }


}
