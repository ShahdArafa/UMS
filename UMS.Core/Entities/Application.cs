using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class Application : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DesiredDepartment { get; set; }
       
        public string ImagePath { get; set; }
        public int? StudentId { get; set; }
        public Student Student { get; set; }
        public string HighSchoolCertificatePath { get; set; }
        public string BirthCertificatePath { get; set; }

        public string AdissmioncardPath { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";// "Pending", "Accepted", "Rejected"
        public string OcrResult { get; set; } // النص المستخرج من الصورة


    }


   

      
}

