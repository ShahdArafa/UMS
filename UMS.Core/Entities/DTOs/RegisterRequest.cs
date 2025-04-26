using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class RegisterRequest
    {
       
        public int CourseId { get; set; }
       




        //For Students 
        public int? StudentId { get; set; }
        public int?  GroupId { get; set; }
    }
}
