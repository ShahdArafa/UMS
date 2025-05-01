using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    // -----------------------
    //  User
    // -----------------------
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string Password { get; set; }

        [Required, MaxLength(50)]
        public string Role { get; set; } // e.g. "Student", "Faculty", "Admin"

        public NotificationPreference NotificationPreference { get; set; }
    }
}
