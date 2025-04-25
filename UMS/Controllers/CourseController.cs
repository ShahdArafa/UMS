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
        [HttpGet("{studentId}/assignments")]
        public async Task<IActionResult> GetAssignments(int studentId)
        {
            var assignments = await _context.Assignments
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.CourseGroup)
                        .ThenInclude(g => g.Course)
                .Where(a => a.Enrollment.StudentId == studentId)
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




        // endpoint return quizzes of each course
        [HttpGet("student/{Id}/course/{courseId}/quizzes")]
        public async Task<IActionResult> GetQuizzesForCourse(int Id, int courseId)
        {
            // تحقق إن الطالب فعلاً مسجل في الكورس ده
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == Id && e.CourseId == courseId);

            if (!isEnrolled)
                return Unauthorized("الطالب غير مسجل في هذه المادة.");

            // هات الكويزات الخاصة بالمادة
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
 
       
        [HttpGet("{studentId}/courses/{courseId}/attendance")]
        public async Task<IActionResult> GetAttendanceForCourse(int studentId, int courseId)
        {
            var attendanceRecords = await _context.Attendances
                .Include(a => a.Enrollment)
                .Where(a =>
                    a.Enrollment.StudentId == studentId &&
                    a.Enrollment.CourseId == courseId)
                .OrderBy(a => a.Date) // يمكنك ترتيب البيانات حسب التاريخ أو إزالة الترتيب إن لم يكن ضروريًا
                .ToListAsync();

            var attendanceDto = attendanceRecords.Select(a => new
            {
                Week = a.WeekNumber,  // سيتم جلب رقم الأسبوع فقط
                Lecture = a.LectureAttendance ? "Present" : "Absent",
                Section = a.SectionAttendance ? "Present" : "Absent"
            }).ToList();

            return Ok(attendanceDto);
        }

        

        [HttpGet("suggested-courses/{studentId}")]
        public async Task<IActionResult> GetSuggestedCourses(int studentId)
        {
            var courses = await _courseService.GetSuggestedCoursesAsync(studentId);
            return Ok(courses);
        }


        [HttpPost("register-course")]
        public async Task<IActionResult> RegisterCourse( RegisteredCourse dto)
        {
            var result = await _courseService.RegisterCoursesAsync(dto.StudentId, dto.selectedGroupIds);
            return Ok(new { message = result });
        }
        [HttpPost("submit-assignment")]
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

            // حفظ الملف
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Submissions");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
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
                Title = "Upload New Task",
                Message = $"Student with ID {dto.StudentId} has submitted a solution for the assignment '{assignment.Title}'.",
                CreatedAt = DateTime.Now,
                IsRead = false,
                FileUrl = fileUrl
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Assignment submitted successfully." });
        }



    }
}
