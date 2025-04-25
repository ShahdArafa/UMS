using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Core.Entities
{
    public class OcrResult
    {
        public string Department { get; set; }
        public double TotalScore { get; set; }
        public string RawText { get; set; }
        public string Error { get; set; }
    }
}
