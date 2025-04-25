using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities;
using UMS.Core.Entities.DTOs;
using UMS.Repository.Services;

namespace UMS.Service
{
    public class CourseService : ICourseService
    {
        private readonly StoreContext _context;
        private readonly ILogger<CourseService> _logger;

        public CourseService(StoreContext context, ILogger<CourseService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public int GetMaxAllowedHours(double gpa, int semester)
        {

            if (semester == 1)
                return 18;
            else if (gpa >= 2.0)
                return 20;
            else if (gpa >= 1.5)
                return 16;
            else
                return 14;
        }
        public async Task<List<SuggestedCourseDto>> GetSuggestedCoursesAsync(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                _logger.LogWarning($"Student with ID {studentId} not found.");
                return new List<SuggestedCourseDto>();
            }

            // سجل تفاصيل الطالب
            _logger.LogInformation($"Student found: {student.Name}, Department: {student.DepartmentName}");

            // جلب الدورات المسجلة للطالب
            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.CourseId)
                .ToListAsync();

            // سجل الدورات المسجلة
            _logger.LogInformation($"Enrolled Course IDs: {string.Join(", ", enrolledCourseIds)}");

            // جلب الدورات المتاحة بناءً على القسم والدورات المسجلة
            var availableCourses = await _context.Courses
                .Include(c => c.Groups)
                .Where(c =>
                    c.Department == student.DepartmentName &&
                    !enrolledCourseIds.Contains(c.Id) &&
                    (c.PrerequisiteCourseId == null || enrolledCourseIds.Contains(c.PrerequisiteCourseId.Value)))
                .ToListAsync();

            // سجل عدد الدورات المتاحة
            _logger.LogInformation($"Available Courses: {availableCourses.Count}");

            // تحويل الدورات المتاحة إلى نموذج SuggestedCourseDto
            var suggestedCourses = availableCourses
                .SelectMany(c => c.Groups.Select(g => new SuggestedCourseDto
                {
                    CourseId = c.Id,
                    CourseName = c.Name,
                    Units = c.Units,
                    InstructorName = g.InstructorName,
                    GroupId = g.Id,
                    Day = g.Day,
                    Time = g.Time,
                    Location = g.Location
                }))
                .ToList();

            // سجل عدد الدورات المقترحة
            _logger.LogInformation($"Suggested Courses: {suggestedCourses.Count}");

            return suggestedCourses;
        }

        public async Task<object> RegisterCoursesAsync(int studentId, List<int> selectedGroupIds)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return new
                {
                    Message = "الطالب غير موجود.",
                    RegisteredCourses = new List<object>(),
                    TotalUnits = 0
                };
            }

            var gpa = student.GPA;
            var semester = student.Semester;
            var maxAllowedHours = GetMaxAllowedHours(gpa, semester);

            // جلب الجروبات اللي الطالب اختارها
            var selectedGroups = await _context.CourseGroups
                .Include(g => g.Course)
                .Where(g => selectedGroupIds.Contains(g.Id) && g.Course.Department == student.DepartmentName)
                .ToListAsync();

            var totalUnits = selectedGroups.Sum(g => g.Course.Units);

            if (totalUnits > maxAllowedHours)
            {
                return new
                {
                    Message = $"لا يمكنك تسجيل أكثر من {maxAllowedHours} ساعة.",
                    RegisteredCourses = new List<object>(),
                    TotalUnits = 0
                };
            }

            var registeredCourses = new List<object>();
            var registeredTimes = new List<(string Day, TimeSpan Time)>(); // لتخزين مواعيد الجروبات
            var conflictedCourses = new List<string>();

            foreach (var group in selectedGroups)
            {
                // التحقق من وجود تعارض في المواعيد
                if (registeredTimes.Any(t => t.Day == group.Day && t.Time == TimeSpan.Parse(group.Time)))
                {
                    conflictedCourses.Add(group.Course.Name);
                    continue;
                }

                registeredTimes.Add((group.Day, TimeSpan.Parse(group.Time)));

                _context.Enrollments.Add(new Enrollment
                {
                    StudentId = studentId,
                    CourseId = group.CourseId
                });

                registeredCourses.Add(new
                {
                    CourseName = group.Course.Name,
                    Units = group.Course.Units,
                    InstructorName = group.InstructorName,
                    Day = group.Day,
                    Time = group.Time,
                    Location = group.Location
                });
            }

            await _context.SaveChangesAsync();

            var resultMessage = conflictedCourses.Any()
                ? $"تم تسجيل المواد بنجاح. لم يتم تسجيل المواد التالية بسبب تعارض في المواعيد: {string.Join(", ", conflictedCourses)}"
                : "تم تسجيل المواد بنجاح.";

            return new
            {
                Message = resultMessage,
                RegisteredCourses = registeredCourses,
                TotalUnits = registeredCourses.Sum(c => (int)c.GetType().GetProperty("Units")?.GetValue(c)!)
            };
        }
    }
}