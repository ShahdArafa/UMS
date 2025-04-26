using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;

namespace UMS.Core.Entities
{
    // -----------------------
    //  Student
    // -----------------------
    public class Student
    {
        [Key]
        public int Id { get; set; }

        // Link back to the User table
        public int UserId { get; set; }

        public int Semester { get; set; }
        public string? StudentIdentifier { get; set; }

        // Optionally supervised by a Faculty
        public int? SupervisorId { get; set; }

        [MaxLength(250)]
        public string? Image { get; set; }

        public double GPA { get; set; }
        public int? TotalUnits { get; set; }
        public string  DepartmentName { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public bool IsFirstSemester { get; set; }
        public List<Registeration> Registrations { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        // This is the **single** Supervisor property
        [ForeignKey(nameof(SupervisorId))]
        [InverseProperty(nameof(Faculty.SupervisedStudents))]
        public virtual Faculty Supervisor { get; set; }


        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
            = new HashSet<Enrollment>();

        public virtual ICollection<TimeTable> TimeTables { get; set; }
            = new HashSet<TimeTable>();
    }




}
