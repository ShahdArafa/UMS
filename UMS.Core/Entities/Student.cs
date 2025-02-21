using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class Student:User
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public double GPA { get; set; }
        public String Supervisor { get; set; }
    }
}
