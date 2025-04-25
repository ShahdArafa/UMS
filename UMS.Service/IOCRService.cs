using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities;

namespace UMS.Service
{
    public interface IOCRService
    {
        Task<string> AnalyzeStudentImageAsync(string imagePath);
    }
}
