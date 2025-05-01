using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UMS.Core.Entities;
using UMS.Core.Entities.DTOs;
using UMS.Repository.Services;
using UMS.Service;

namespace UMS.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly StoreContext _context;
        private readonly ICourseService _courseService;

        public CourseController(StoreContext context , ICourseService courseService)
        {
            _context = context;
            _courseService = courseService;
        }
        [Authorize(Roles = "Student")]
        [HttpGet("assignments")]
        public async Task<IActionResult> GetAssignments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid or missing token.");
            }

            // جلب StudentId بناءً على UserId
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                return NotFound(new { message = "Student not found." });
            }

            var assignments = await _context.Assignments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.CourseGroup)
                        .ThenInclude(g => g.Course)
                .Where(a => a.Enrollment.StudentId == student.Id)
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    WeekNumber = a.WeekNumber,
                    Deadline = a.Deadline,
                    CourseName = a.Enrollment.CourseGroup.Course.Name,
                    GroupName = a.Enrollment.CourseGroup.GroupName
                })
                .ToListAsync();

            if (assignments == null || !assignments.Any())
            {
                return NotFound(new { message = "No assignments found for this student." });
            }

            return Ok(assignments);
        }




        [Authorize(Roles = "Student")]
        [HttpGet("course/{courseId}/quizzes")]
        public async Task<IActionResult> GetQuizzesForCourse(int courseId)
        {
            // استخراج UserId من التوكن
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing token." });
            }

            // جلب StudentId بناءً على UserId
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                return NotFound(new { message = "Student not found." });
            }

            // تحقق إن الطالب مسجل في الكورس
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == student.Id && e.CourseId == courseId);

            if (!isEnrolled)
            {
                return Unauthorized(new { message = "Student is not enrolled in this course." });
            }

            // جلب الكويزات الخاصة بالكورس
            var quizzes = await _context.Quizzes
                .Where(q => q.CourseId == courseId)
                .Select(q => new
                {
                    q.Id,
                    q.Title,
                    q.QuizDate,
                    q.WeekNumber
                })
                .ToListAsync();

            return Ok(quizzes);
        }


        [Authorize(Roles = "Student")]
        [HttpGet("courses/{courseId}/attendance")]
        public async Task<IActionResult> GetAttendanceForCourse(int courseId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid or missing token.");

            // جرب تجيب StudentId من جدول الطلاب
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
                return NotFound("Student not found.");

            var attendanceRecords = await _context.Attendances
                .Include(a => a.Enrollment)
                .Where(a =>
                    a.Enrollment.StudentId == student.Id &&
                    a.Enrollment.CourseId == courseId)
                .Select(a => new
                {
                    Week = a.WeekNumber,
                    Lecture = a.LectureAttendance ? "Present" : "Absent",
                    Section = a.SectionAttendance ? "Present" : "Absent"
                })
                .ToListAsync();

            return Ok(attendanceRecords);
        }




        [Authorize(Roles = "Student")]
        [HttpGet("suggested-courses/{studentId}")]
        public async Task<IActionResult> GetSuggestedCourses(int studentId)
        {
            var courses = await _courseService.GetSuggestedCoursesAsync(studentId);
            return Ok(courses);
        }

        [Authorize(Roles = "Student")]
        [HttpPost("register-course")]
        public async Task<IActionResult> RegisterCourse( RegisteredCourse dto)
        {
            var result = await _courseService.RegisterCoursesAsync(dto.StudentId, dto.selectedGroupIds);
            return Ok(new { message = result });
        }

        [Authorize(Roles = "Student")]
        [HttpPost("courses/submit-assignment")]
        public async Task<IActionResult> SubmitAssignment([FromForm] SubmitAssignmentDto dto)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.CourseGroup)
                        .ThenInclude(g => g.Faculty)
                .FirstOrDefaultAsync(a => a.Id == dto.AssignmentId);

            if (assignment == null)
                return NotFound(new { message = "Assignment not found." });

            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null)
                return NotFound(new { message = "Student not found." });

            // تأكد الربط بين الـ Assignment والفـ Faculty
            if (assignment.Enrollment?.CourseGroup?.Faculty == null)
                return BadRequest(new { message = "Assignment is not properly linked to a faculty member." });

            if (dto == null || dto.File == null)
            {
                return BadRequest("No file uploaded.");
            }

            // حفظ الملف
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assignments");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.File.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var fileUrl = $"/Submissions/{fileName}";

            // حفظ الـ Submission
            var submission = new AssignmentSubmission
            {
                AssignmentId = dto.AssignmentId,
                StudentId = dto.StudentId,
                FilePath = fileUrl,
                SubmittedAt = DateTime.Now,
                Description = dto.Description
            };

            _context.AssignmentSubmissions.Add(submission);

            // إنشاء Notification
            var notification = new Notification
            {
                FacultyId = assignment.Enrollment.CourseGroup.Faculty.Id,
                Title = "New Assignment Submission",
                Message = $"Student with ID {dto.StudentId} has submitted a solution for the assignment '{assignment.Title}'.",
                CreatedAt = DateTime.Now,
                IsRead = false,
                FileUrl = fileUrl,
                StudentId = dto.StudentId,          // اضيفهم هنا
                AssignmentId = dto.AssignmentId
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Assignment submitted successfully." });
        }



    }
}
