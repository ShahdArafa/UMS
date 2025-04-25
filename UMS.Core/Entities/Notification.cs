using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public int? StudentId { get; set; }

        public int? UserId { get; set; }

        public User? User { get; set; }
        public int? CourseId { get; set; }
        public string Title { get; set; }

        public int? FacultyId { get; set; }

        public string Message { get; set; }
        public string? FileUrl { get; set; } // لينك الجدول
        public bool? IsRead { get; set; } = false;

        public int? ApplicationId { get; set; }
        [ForeignKey(nameof(ApplicationId))]
        public virtual Application? Application { get; set; }

        public string? Type{ get; set; } // "Assignment" أو "Quiz"
        public int RelatedItemId { get; set; }

        public int? AssignmentId { get; set; }
        public Assignments? Assignment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
       

        public Student? Student { get; set; }

        public Course? Course { get; set; }

    }
}
