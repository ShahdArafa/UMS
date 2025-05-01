using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class EventPost
    {
        
            [Key]
            public int Id { get; set; }

            [Required]
            public string Title { get; set; }

            public string? Description { get; set; }

        public byte[]? ImageData { get; set; } // لازم يكون Byte[]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

           
        }

    
}
