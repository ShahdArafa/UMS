using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class Quiz
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime QuizDate { get; set; }
        public string Description { get; set; }

        public string WeekNumber { get; set; }
        public int? CourseId { get; set; }

        public Course? Course { get; set; }

        public int CourseGroupId { get; set; }

        // Navigation
        [ForeignKey(nameof(CourseGroupId))]
        public CourseGroup CourseGroup { get; set; }
    }
}
