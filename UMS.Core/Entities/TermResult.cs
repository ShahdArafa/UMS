using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class TermResult
    {

        public int TermResultId { get; set; } // معرف نتيجة الترم
        public int StudentId { get; set; } // معرف الطالب (مرتبط بالطالب في جدول الطلاب)
        public int TermId { get; set; } // معرف الترم (مرتبط بالترم في جدول الترمات)
        public decimal FinalGrade { get; set; } // الدرجة النهائية

        public double GPA { get; set; }
        public int TotalOfUnits { get; set; }


        // العلاقه بين الجداول:
        public virtual Student Student { get; set; }
        public virtual Term Term { get; set; }
        public ICollection<CourseResult> CourseResults { get; set; }
    }
}
