using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities.DTOs
{
    public class NotificationsSettings
    {
        [Key]
        public int UserId { get; set; }

        public bool? IsEnabled { get; set; }
    }
}
