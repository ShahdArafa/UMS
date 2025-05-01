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

        [Authorize(Roles = "Student")]
        [HttpGet("{userId}/home")]
        public async Task<IActionResult> GetStudentDetails(int userId)
        {
            // جلب الطالب باستخدام UserId، وشمول بيانات المستخدم والدورات
            var student = await context.Students
                .Where(s => s.UserId == userId)
                .Include(s => s.User) // علشان نجيب الـ Email
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound("الطالب غير موجود.");
            }

            var result = new
            {
                studentId = student.Id,
                name = student.Name,
                email = student.User.Email, // من جدول الـ Users
                enrollments = student.Enrollments.Select(e => new
                {
                    courseId = e.Course.Id,
                    courseName = e.Course.Name,
                    creditHours = e.Course.Units
                }).ToList()
            };

            return Ok(result);
        }



        [Authorize(Roles = "Student")]
        [HttpGet("notifications")]
        public async Task<IActionResult> GetStudentNotifications()
        {
            var userId = int.Parse("4010"); // ID من جدول الطلاب

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


