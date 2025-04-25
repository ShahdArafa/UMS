using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities.DTOs;

namespace UMS.Core.Entities
{
    public class CourseGroup
    {
        public int Id { get; set; }
        public int CourseId { get; set; }

        public string GroupName { get; set; }
        public string InstructorName { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string Location { get; set; }
        public int MaxSeats { get; set; }
        public int RegisteredCount { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }

        public int FacultyId { get; set; }  // FacultyId is the foreign key

        // Navigation property to the Faculty
        [ForeignKey(nameof(FacultyId))]
        public Faculty Faculty { get; set; }  // This links the course group to a faculty member
        public int? TeachingAssistantId { get; set; }


        [ForeignKey(nameof(TeachingAssistantId))]
        [InverseProperty(nameof(Faculty.AssistingGroups))]
        public Faculty TeachingAssistant { get; set; }

        public virtual ICollection<Assignments> Assignments { get; set; } = new HashSet<Assignments>();

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
    }
}
