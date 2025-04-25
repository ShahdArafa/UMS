using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class CourseResult
    {
        public int Id { get; set; }
        public int TermResultId { get; set; }
        public int CourseId { get; set; }
        public string Grade { get; set; } // التقدير الذي حصل عليه الطالب
        public int Units { get; set; }  // الوحدات الدراسية الخاصة بالمادة

        public TermResult TermResult { get; set; }
        public Course Course { get; set; }
    }
}
