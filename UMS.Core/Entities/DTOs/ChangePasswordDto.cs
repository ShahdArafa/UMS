using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }  // كلمة السر الحالية
        public string NewPassword { get; set; }      // كلمة السر الجديدة
        public string ConfirmNewPassword { get; set; } 
    }
}
