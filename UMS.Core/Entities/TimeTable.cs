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
    //  TimeTable
    // -----------------------
    public class TimeTable
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int CourseId { get; set; }

        [MaxLength(20)]
        public string Day { get; set; } // e.g. "Monday"
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [MaxLength(200)]
        public string Location { get; set; }

        [MaxLength(50)]
        public string Type { get; set; } // e.g. "Lecture", "Lab"

        // Navigation
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }
    }
}
