using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }

        public string UserEmail { get; set; }

        public StudentHomeDto StudentInfo { get; set; }
    }
}
