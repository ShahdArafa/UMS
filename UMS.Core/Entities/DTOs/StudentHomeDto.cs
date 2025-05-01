using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class StudentHomeDto
    {
        public string StudentIdentifier { get; set; }
        public string Name { get; set; }
        public double GPA { get; set; }
        public int TotalUnits { get; set; }

        public List<CourseDto> Courses { get; set; }

    }
}
