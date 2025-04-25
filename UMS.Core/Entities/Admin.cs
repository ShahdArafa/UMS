using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    // -----------------------
    //  Admin
    // -----------------------
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        // Link back to the User table
        public int UserId { get; set; }

        [MaxLength(100)]
        public string TaskType { get; set; }

        // Navigation
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
