using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class RegisteredCourseResultDto
    {

        public string CourseName { get; set; }
        public string InstructorName { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string Location { get; set; }
        public int Units { get; set; }
    }
}
