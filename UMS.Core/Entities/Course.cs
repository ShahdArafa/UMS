using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UMS.Core.Entities
{
    // -----------------------
    //  Course
    // -----------------------
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Schedule { get; set; }

        public int Units { get; set; }

        public string Department { get; set; }

        public int Semester { get; set; }

        // Navigation
        public virtual ICollection<Enrollment> Enrollments { get; set; }
            = new HashSet<Enrollment>();

        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; }
            = new HashSet<ExamSchedule>();

        public virtual ICollection<TimeTable> TimeTables { get; set; }
            = new HashSet<TimeTable>();

        public virtual ICollection<CourseGroup> Groups { get; set; } = new List<CourseGroup>();
       
         public int? PrerequisiteCourseId { get; set; }

        [ForeignKey(nameof(PrerequisiteCourseId))]
        public virtual Course PrerequisiteCourse { get; set; }

       

        
    }
}
