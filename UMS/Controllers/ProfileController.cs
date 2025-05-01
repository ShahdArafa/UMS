using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UMS.Core.Entities;
using UMS.Core.Entities.DTOs;

namespace UMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly StoreContext _context;

        public ProfileController(StoreContext context)
        {
            _context = context;
        }
        [Authorize (Roles = "Student,Admin,Faculty")]
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            // 1- نجيب UserId من التوكين (JWT)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new { message = "Unauthorized" });

            var userId = int.Parse(userIdClaim.Value);

            // 2- نجيب الطالب اللي مربوط بالـ User
            var student = await _context.Students
                .Include(s => s.User) // نجيب بيانات اليوزر عشان الإيميل الجامعي
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                return NotFound(new { message = "Student profile not found." });

            // 3- نجهز الـ DTO
            var profile = new StudentProfileDto
            {
                StudentIdentifier = student.StudentIdentifier,
                FullName = student.Name,
                UniversityEmail = student.User.Email, // إيميل الجامعي
                ProfileImageUrl = student.Image // ممكن يكون فاضي
            };

            return Ok(profile);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized(new { message = "Invalid or missing token" });

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Password != dto.CurrentPassword)
                return BadRequest(new { message = "Invalid current password" });

            user.Password = dto.NewPassword;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password updated successfully" }); ;
        }
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotificationSetting()
        {
            // استخراج userId من Claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // التحقق من صحة الـ userId
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Invalid or missing user identifier." });
            }

            // البحث عن تفضيلات الإشعارات للمستخدم باستخدام userId
            var preference = await _context.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId); // استخدام FirstOrDefaultAsync للبحث

            // التحقق من وجود السجل
            if (preference == null)
            {
                return NotFound(new { message = "Notification preference not found." });

            }

            // إعادة النتيجة مع حالة التفعيل
            return Ok(new { enabled = preference.IsEnabled == true });
        }


        [Authorize(Roles = "Student,Admin,Faculty")]
        [HttpPut("profile/notifications")]
        public async Task<IActionResult> ToggleNotifications([FromBody] NotificationsSettings dto)
        {
            // نجيب UserId من التوكين
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Unauthorized" });

            var userId = int.Parse(userIdClaim.Value);

            // نتاكد انه عنده Preference او نعمل جديد
            var preference = await _context.NotificationPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
            if (preference == null)
            {
                preference = new NotificationPreference
                {
                    UserId = userId,
                    IsEnabled = dto.IsEnabled ?? true
                   
                };
                _context.NotificationPreferences.Add(preference);
            }
            else
            {
                preference.IsEnabled = dto.IsEnabled ?? true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification preference updated", isEnabled = preference.IsEnabled });
        }


    }
}
