using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace UMS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        private readonly StoreContext context;

        public ResultController(StoreContext context)
        {
            this.context = context;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("get-terms-results")]
        public async Task<IActionResult> GetTermsResults()
        {
            var userId =int.Parse( User.FindFirstValue(ClaimTypes.NameIdentifier));

            var termsResults = await context.TermResults
                .Where(tr => tr.StudentId == userId)
                .OrderByDescending(tr => tr.Term.TermName)
                .ToListAsync();

            return Ok(termsResults);
        }
        [Authorize(Roles = "Student")]
        [HttpGet("term-result-details/{termId}")]
        public async Task<IActionResult> GetDetailedTermResult(int termId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out int studentId))
                return BadRequest("Invalid user ID.");

            // 1. بيانات الطالب
            var student = await context.Students
                .Where(s => s.Id == studentId)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    Department = s.DepartmentName,
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return NotFound("Student not found.");

            // 2. حساب المعدل التراكمي (Cumulative GPA) و مجموع الوحدات
            var allTermResults = await context.TermResults
                .Where(tr => tr.StudentId == studentId)
                .ToListAsync();

            var cumulativeGPA = allTermResults.Any()
                ? Math.Round(allTermResults.Average(tr => (double)tr.GPA), 2)
                : 0;

            var completedUnits = allTermResults.Sum(tr => tr.TotalOfUnits);
            var totalUnitsInProgram = await context.Courses.SumAsync(c => c.Units);
            var remainingUnits = totalUnitsInProgram - completedUnits;

            // 3. تفاصيل الترم المطلوب
            var termResult = await context.TermResults
                .Where(tr => tr.StudentId == studentId && tr.TermId == termId)
                .Include(tr => tr.CourseResults)
                    .ThenInclude(cr => cr.Course)
                .FirstOrDefaultAsync();

            if (termResult == null)
                return NotFound("No result found for this term.");

            var courseDetails = termResult.CourseResults.Select(cr => new
            {
                CourseName = cr.Course.Name,
                Grade = cr.Grade,
                Credits = cr.Course.Units
            });

            // 4. بناء الريسبونس النهائي
            var result = new
            {
                StudentName = student.Name,
                Department = student.Department,
                CumulativeGPA = cumulativeGPA,
                CompletedUnits = completedUnits,
                RemainingUnits = remainingUnits,
                TermId = termId,
                TermGPA = termResult.GPA,
                Courses = courseDetails
            };

            return Ok(result);
        }

    }

}