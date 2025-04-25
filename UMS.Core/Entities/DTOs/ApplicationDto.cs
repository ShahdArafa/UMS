using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class ApplicationDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string DesiredDepartment { get; set; }

        public IFormFile Image { get; set; }
        public IFormFile AdmissionCard { get; set; }

        public IFormFile HighSchoolCertificate { get; set; }
        public IFormFile BirthCertificate
        {
            get; set;



        }
    }
}
