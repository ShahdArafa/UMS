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

        [HttpGet("{id}/home")]
        public async Task<IActionResult> GetStudentDetails(int id)
        {
            // جلب بيانات الطالب بالإضافة إلى المواد المسجلة
            var student = await context.Students
                .Where(s => s.Id == id)
                .Include(s => s.Enrollments)  // شمول المواد المسجلة
                .ThenInclude(cr => cr.Course)  // شمول تفاصيل المادة
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound("الطالب غير موجود.");
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

       // [Authorize(Roles = "Student")]
        [HttpGet("notifications")]
        public async Task<IActionResult> GetStudentNotifications()
        {
            var userId = int.Parse("4"); // ID من جدول الطلاب

            var notifications = await context.Notifications
                .Where(n => n.StudentId == userId)
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


