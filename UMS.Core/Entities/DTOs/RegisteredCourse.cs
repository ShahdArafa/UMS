using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class RegisteredCourse
    {
        public int StudentId { get; set; }
        public List<int> SelectedCourseIds { get; set; }

        public List<int> selectedGroupIds { get; set; }


    }
}
