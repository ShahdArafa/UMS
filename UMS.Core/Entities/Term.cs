using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class Term
    {
            public int TermId { get; set; } // معرف الترم
            public string TermName { get; set; } // اسم الترم (مثال: "ترم أول 2025")
            public DateTime StartDate { get; set; } // تاريخ بداية الترم
            public DateTime EndDate { get; set; } // تاريخ نهاية الترم
          public ICollection<TermResult> TermResults { get; set; }

    }
}
