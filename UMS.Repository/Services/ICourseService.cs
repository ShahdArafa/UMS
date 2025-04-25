using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities.DTOs;

namespace UMS.Repository.Services
{
    public interface ICourseService
    {
        int GetMaxAllowedHours(double gpa, int semester);
        Task<List<SuggestedCourseDto>> GetSuggestedCoursesAsync(int studentId);
        Task<object> RegisterCoursesAsync(int studentId, List<int> selectedGroupIds);
    }
}
