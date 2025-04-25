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
    //  Attendance
    // -----------------------
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        public int EnrollmentId { get; set; }

        public bool LectureAttendance { get; set; }
        public bool SectionAttendance { get; set; }
        public DateTime Date { get; set; }
        public string WeekNumber { get; set; }

        // Navigation
        [ForeignKey(nameof(EnrollmentId))]
        public virtual Enrollment Enrollment { get; set; }

       
    }
}
