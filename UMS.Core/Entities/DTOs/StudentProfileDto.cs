using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class StudentProfileDto
    {

        public string StudentIdentifier { get; set; }
        public string FullName { get; set; }
        public string UniversityEmail { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
