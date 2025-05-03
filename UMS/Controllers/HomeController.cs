using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace UMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly StoreContext context;

        public HomeController(StoreContext context)
        {
            this.context = context;
        }
        [Authorize(Policy = "StudentPolicy")]
        [HttpGet("student-details/{userId}")]
        public async Task<IActionResult> GetStudentDetails( [FromRoute]int userId)
        {
            // جلب بيانات الطالب بالإضافة إلى المواد المسجلة
            var student = await context.Students
                .Where(s => s.UserId == userId)
                .Include(s => s.Enrollments)  // شمول المواد المسجلة
                .ThenInclude(cr => cr.Course) // شمول تفاصيل المادة
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound(new { message = "الطالب غير موجود." });
            }

            // تجهيز البيانات للعرض
            var studentData = new
            {
                student.StudentIdentifier,
                student.Name,
                student.GPA,
                student.TotalUnits,
                Courses = student.Enrollments.Select(cr => new
                {
                    cr.Course.Id,
                    cr.Course.Name,
                    cr.Course.Department
                }).ToList()
            };

            return Ok(studentData);
        }


       [Authorize(Roles = "Student")]
        [HttpGet("notifications/{userId}")]
        public async Task<IActionResult> GetStudentNotifications(int userId)
        {
            // احصل على رقم الطالب (StudentId) من خلال userId
            var student = await context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                return NotFound(new { message = "الطالب غير موجود." });
            }

            var notifications = await context.Notifications
                .Where(n => n.StudentId == student.Id)  // نستخدم StudentId الصحيح
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }



        [Authorize(Roles = "Student")]
        [HttpGet("notifications/unread-count")]
        public async Task<IActionResult> GetUnreadNotificationsCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // التحقق من الـ userId إذا كان عدد صحيح
            if (!int.TryParse(userId, out var studentId))
            {
                return BadRequest("Invalid user ID.");
            }

            var unreadCount = await context.Notifications
                .Where(n => n.StudentId == studentId)
                .CountAsync();

            return Ok(new { unreadCount });
        }
    }

} 


