using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMS.Core.Entities;
using UMS.Core.Entities.DTOs;
using static UMS.Service.RegisterationService;

namespace UMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseRegisterationController : ControllerBase
    {
        private readonly RegistrationService _registrationService;
        private readonly StoreContext _context;

        public CourseRegisterationController(StoreContext context)
        {
            _registrationService = new RegistrationService();
            _context = context;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("suggested-courses/{studentId}")]
        public IActionResult GetSuggestedCourses(int studentId)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            // Courses the student already registered
            var registeredCourseIds = _context.Registerations
                .Where(r => r.StudentId == studentId && r.Group != null)
                .Select(r => r.Group.CourseId)
                .Distinct()
                .ToList();

            // All available courses
            var suggestedCourses = _context.Courses
                .Where(c => !registeredCourseIds.Contains(c.Id))
                .Include(c => c.Groups)
                    .ThenInclude(g => g.Enrollments)
                .ToList();

            var result = new List<object>();

            foreach (var course in suggestedCourses)
            {
                foreach (var group in course.Groups)
                {
                    int availableSeats = group.MaxSeats - group.Enrollments.Count;
                    if (availableSeats > 0)
                    {
                        result.Add(new
                        {
                            CourseId = course.Id,
                            CourseName = course.Name,
                            Units = course.Units,
                            GroupId = group.Id,
                            GroupName = group.GroupName,
                            Day = group.DayOfWeek.ToString(),
                            StartTime = group.StartTime.ToString(@"hh\:mm"),
                            EndTime = group.EndTime.ToString(@"hh\:mm"),
                            AvailableSeats = availableSeats
                        });
                    }
                }
            }

            return Ok(result);
        }


        [Authorize(Roles = "Student")]
        [HttpGet("registered-courses/{studentId}")]
        public IActionResult GetRegisteredCourses(int studentId)
        {

            var regs = _context.Registerations
          .Where(r => r.StudentId == studentId)
          .Include(r => r.Group)
           .ThenInclude(g => g.Course)
          .Select(r => new
          {
              CourseId = r.Group.Course.Id,
              CourseName = r.Group.Course.Name,
              GroupName = r.Group.GroupName,
              Units = r.Group.Course.Units,
              DateRegistered = r.DateRegistered
          });
            return Ok(regs);
          }

        [Authorize(Roles = "Student")]
        [HttpPost("register")]
        public IActionResult RegisterCourse([FromBody] RegisterRequest request)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == request.StudentId);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            var group = _context.CourseGroups
                .Include(g => g.Course)
                .FirstOrDefault(g => g.Course.Id == request.CourseId && g.Id == request.GroupId);

            if (group == null)
                return NotFound(new { message = "Group Not Found For The Specific Course and Name" });

            var currentGroups = _context.Registerations
                .Where(r => r.StudentId == request.StudentId)
                .Select(r => r.Group)
                .ToList();

            if (group.AvailableSeats <= 0)
                return NotFound(new { message = "No Seats Available" });

            if (_registrationService.HasTimeConflict(group, currentGroups))
                return BadRequest("Time conflict with another group");

            int maxUnits = _registrationService.GetMaxUnits(student);
            int currentUnits = _context.Registerations
                .Where(r => r.StudentId == request.StudentId)
                .Sum(r => r.Group.Course.Units);

            if (currentUnits + group.Course.Units > maxUnits)
                return NotFound(new { message = "Exceeds Allowed Credit Hours" });

            group.AvailableSeats--;
            _context.Registerations.Add(new Registeration
            {
                StudentId = request.StudentId,
                GroupId = group.Id,
                Group = group,
                DateRegistered = DateTime.Now
            });

            _context.SaveChanges();
            return Ok(new { message = "Registered Successfully" });
        }

        [Authorize(Roles = "Student")]
        [HttpDelete("unregister-course")]
        public IActionResult UnregisterCourse(int studentId, int courseId)
        {
            var registration = _context.Registerations
                .FirstOrDefault(r => r.StudentId == studentId && r.Group.CourseId == courseId);

            if (registration == null)
                return NotFound(new { message = "Registration Not Found" });

            var group = _context.CourseGroups.FirstOrDefault(g => g.Id == registration.GroupId);
            if (group != null)
            {
                group.AvailableSeats++;
            }

            _context.Registerations.Remove(registration);
            _context.SaveChanges();

            return Ok(new { message = " Course UnRegistered Successfully" });
        }

    }
}