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
    //  Faculty
    // -----------------------
    public class Faculty
    {
        [Key]
        public int Id { get; set; }

        // Link back to the User table
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Position { get; set; }

        // Navigation
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        // A faculty member can supervise many students
        [InverseProperty(nameof(Student.Supervisor))]
        public virtual ICollection<Student> SupervisedStudents { get; set; }
            = new HashSet<Student>();

        // Navigation property for the courses (CourseGroups) taught by the Faculty
        [InverseProperty(nameof(CourseGroup.Faculty))]
        public ICollection<CourseGroup> Groups { get; set; } = new HashSet<CourseGroup>();

        [InverseProperty(nameof(CourseGroup.TeachingAssistant))]
        public ICollection<CourseGroup> AssistingGroups { get; set; } = new HashSet<CourseGroup>();


    }
}
