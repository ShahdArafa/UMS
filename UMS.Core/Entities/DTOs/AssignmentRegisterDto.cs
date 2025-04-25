using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class AssignmentRegisterDto
    {

        public int CourseGroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string WeekNumber { get; set; }
        public IFormFile? File { get; set; }
    }
}
