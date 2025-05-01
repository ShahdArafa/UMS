using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class StudentAccounts
    {
        public int Id { get; set; }
        public string UniversityEmail { get; set; }
        public string HashedPassword { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
