using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class AssignmentDto
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string GroupName { get; set; }
        [Required]
        public string WeekNumber { get; set; }
        public string CourseName { get; set; }  // اسم المادة التي ينتمي إليها الواجب

       
        
       
    }

}
