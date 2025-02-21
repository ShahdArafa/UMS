using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.Identity
{
    public class AppUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string Role { get; set; }
        //For Students 
        public string? StudentId { get; set; }
    }
}
