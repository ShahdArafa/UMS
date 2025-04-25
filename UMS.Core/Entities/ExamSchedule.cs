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
    //  ExamSchedule
    // -----------------------
    public class ExamSchedule
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }

        [MaxLength(50)]
        public string ExamType { get; set; } // e.g. "Midterm", "Final"

        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        [MaxLength(200)]
        public string Location { get; set; }

        // Navigation
        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }
    }
}