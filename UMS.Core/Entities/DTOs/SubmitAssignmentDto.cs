using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class SubmitAssignmentDto
    {
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
    }
}
