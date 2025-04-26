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

        [HttpGet("suggested-courses/{studentId}")]
        public IActionResult GetSuggestedCourses(int studentId)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null) return NotFound("Student not found");

            var allCourses = _context.Courses.ToList();
            var previousRegs = _context.Registerations.Where(r => r.StudentId == studentId).ToList();

            var suggested = _registrationService.GetSuggestedCourses(student, allCourses, previousRegs);
            return Ok(suggested);
        }

        [HttpGet("registered-courses/{studentId}")]
        public IActionResult GetRegisteredCourses(int studentId)
        {

            var regs = _context.Registerations
          .Where(r => r.StudentId == studentId)
          .Include(r => r.Group)
           .ThenInclude(g => g.Course)
          .Select(r => new
          {
              CourseName = r.Group.Course.Name,
              GroupName = r.Group.GroupName,
              Units = r.Group.Course.Units,
              DateRegistered = r.DateRegistered
          });
            return Ok(regs);
          }

        [HttpPost("register")]
        public IActionResult RegisterCourse([FromBody] RegisterRequest request)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == request.StudentId);
            if (student == null)
                return NotFound("Student not found");

            var group = _context.CourseGroups
                .Include(g => g.Course)
                .FirstOrDefault(g => g.Course.Id == request.CourseId && g.Id == request.GroupId);

            if (group == null)
                return NotFound("Group not found for the specified course and name");

            var currentGroups = _context.Registerations
                .Where(r => r.StudentId == request.StudentId)
                .Select(r => r.Group)
                .ToList();

            if (group.AvailableSeats <= 0)
                return BadRequest("No seats available");

            if (_registrationService.HasTimeConflict(group, currentGroups))
                return BadRequest("Time conflict with another group");

            int maxUnits = _registrationService.GetMaxUnits(student);
            int currentUnits = _context.Registerations
                .Where(r => r.StudentId == request.StudentId)
                .Sum(r => r.Group.Course.Units);

            if (currentUnits + group.Course.Units > maxUnits)
                return BadRequest("Exceeds allowed credit hours");

            group.AvailableSeats--;
            _context.Registerations.Add(new Registeration
            {
                StudentId = request.StudentId,
                GroupId = group.Id,
                Group = group,
                DateRegistered = DateTime.Now
            });

            _context.SaveChanges();
            return Ok("Registered successfully");
        }

    }
}