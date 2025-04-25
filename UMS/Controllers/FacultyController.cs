using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using UMS.Core.Entities;
using UMS.Core.Entities.DTOs;

namespace UMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultyController : ControllerBase
    {
        private readonly StoreContext _context;

        public FacultyController( StoreContext context)
        {
            _context = context;
        }

        [HttpGet("{facultyId}/groups")]
        public async Task<IActionResult> GetFacultyGroups(int facultyId)
        {
            var groups = await _context.CourseGroups
       .Where(g => g.FacultyId == facultyId)
       .Include(g => g.Course)
       .Include(g => g.Faculty)
       .Select(g => new FacultyGroupDto
       {
           CourseName = g.Course.Name,
           GroupName = g.GroupName,
           FacultyName = g.Faculty.Name
       })
       .ToListAsync();

            return Ok(groups);
        }

        [HttpGet("courses/{courseGroupId}/attendance/weeks")]
        public IActionResult GetAttendanceWeeks(int courseGroupId)
        {
            int totalWeeks = 14;

            var arabicWeekNames = new[]
            {
        "الأسبوع الأول", "الأسبوع الثاني", "الأسبوع الثالث", "الأسبوع الرابع",
        "الأسبوع الخامس", "الأسبوع السادس", "الأسبوع السابع", "الأسبوع الثامن",
        "الأسبوع التاسع", "الأسبوع العاشر", "الأسبوع الحادي عشر", "الأسبوع الثاني عشر",
        "الأسبوع الثالث عشر", "الأسبوع الرابع عشر"
    };

            var weeks = arabicWeekNames
                .Take(totalWeeks)
                .Select((week, index) => new
                {
                    WeekNumber = index + 1,
                    WeekName = week
                }).ToList();

            return Ok(weeks);
        }

        [HttpGet("course-group/{courseGroupId}/week/{weekNumber}")]
        public async Task<IActionResult> GetStudentsAttendanceForGroupAndWeek(int courseGroupId, string weekNumber)
        {
            var enrollmentsInGroup = await _context.Enrollments
                .Where(e => e.Course.Groups.Any(g => g.Id == courseGroupId))
                .Select(e => new
                {
                    StudentName = e.Student.Name,
                    StudentUniversityId = e.Student.StudentIdentifier,
                    IsPresent = e.Attendances
                        .Where(a => a.WeekNumber == weekNumber)
                        .Select(a => a.LectureAttendance)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(enrollmentsInGroup);
        }

        [HttpPost("assignments")]
        public async Task<IActionResult> PostAssignment([FromForm] AssignmentRegisterDto dto)
        {
            
            var courseGroupExists = await _context.CourseGroups.AnyAsync(g => g.Id == dto.CourseGroupId);
            if (!courseGroupExists)
            {
                return NotFound($"Course group with ID {dto.CourseGroupId} does not exist.");
            }

            string uploadedFilePath = null;

            if (dto.File != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(dto.File.FileName);
                var extension = Path.GetExtension(dto.File.FileName);
                var uniqueName = $"{fileName}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine("wwwroot/assignments", uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                uploadedFilePath = filePath;
            }
            var assignment = new Assignments
            {
                CourseGroupId = dto.CourseGroupId,
                Title = dto.Title,
                Description = dto.Description,
                FilePath = uploadedFilePath,
                WeekNumber = dto.WeekNumber
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            // send Notifications to Students

            // هات الطلاب المسجلين في نفس الكورس
            var courseId = await _context.CourseGroups
                .Where(g => g.Id == assignment.CourseGroupId)
                .Select(g => g.CourseId)
                .FirstOrDefaultAsync();

            var studentIds = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToListAsync();

            foreach (var studentId in studentIds)
            {
                var notification = new Notification
                {
                    Title = "New Assignment",
                    StudentId = studentId,
                    Message = $"New Assignment Is Added: {assignment.Title}",
                    Type = "Assignment",
                    RelatedItemId = assignment.Id
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();




            return Ok("Assignment Posted");
        }
        [HttpPost("PostQuiz")]
        public async Task<IActionResult> PostQuiz([FromBody] QuizRegisterDto dto)
        {

            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            // تحقق من أن الجروب موجود
            var courseGroup = await _context.CourseGroups
                .FirstOrDefaultAsync(cg => cg.Id == dto.CourseGroupId);

            if (courseGroup == null)
            {
                return BadRequest("The specified group does not exist.");
            }

            // تحقق من وجود CourseId
            if (courseGroup.CourseId == 0)
            {
                return BadRequest("Course ID is invalid.");
            }


            // إنشاء الكويز
            var quiz = new Quiz
                {
                    CourseGroupId = dto.CourseGroupId,
                    Title = dto.Title,
                    Description = dto.Description,
                    QuizDate = dto.QuizDate,
                    WeekNumber = dto.WeekNumber,
                    CourseId = courseGroup.CourseId  // تأكد من الوصول إلى CourseId من خلال الكائن courseGroup
                };

                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();

            var courseId = await _context.CourseGroups
                .Where(g => g.Id == quiz.CourseGroupId)
                .Select(g => g.CourseId)
                .FirstOrDefaultAsync();

            var studentIds = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToListAsync();

            foreach (var studentId in studentIds)
            {
                var notification = new Notification
                {
                   Title = quiz.Title,
                    StudentId = studentId,
                    Message = $"A new quiz has been posted for: {quiz.Title}",
                    Type = "Quiz",
                    RelatedItemId = quiz.Id
                };
                _context.Notifications.Add(notification);
            }
              await _context.SaveChangesAsync();

            return Ok(new { message = "Quiz uploaded successfully." });
        }
        [HttpGet("{facultyId}/student-notifications")]
        public async Task<IActionResult> GetStudentNotifications(int facultyId)
        {
            var allNotifications = await _context.Notifications
                .Where(n => n.FacultyId == facultyId && n.Message.Contains("has submitted a solution"))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            var latestNotifications = allNotifications
                .GroupBy(n => new { n.AssignmentId, n.StudentId })
                .Select(g => g.First())  // أول عنصر بعد الترتيب
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return Ok(latestNotifications);
        }


    }


}




