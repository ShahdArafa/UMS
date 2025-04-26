using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class Registeration
    {
        [Key]
        public int RegistrationId { get; set; }
        public int? StudentId { get; set; }
        public int? GroupId { get; set; }
        public DateTime DateRegistered { get; set; }
        public CourseGroup Group { get; set; }
    }
}
