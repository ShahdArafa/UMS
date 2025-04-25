using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class QuizRegisterDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime QuizDate { get; set; }
        public string WeekNumber { get; set; }
        public int CourseGroupId { get; set; }
    }
}
