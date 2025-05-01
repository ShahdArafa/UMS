using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class NotificationPreference
    {
        public int Id { get; set; }

        public int UserId { get; set; }   // نربطه بالـ User
        public bool IsEnabled { get; set; }

        public User User { get; set; }
    }
}
