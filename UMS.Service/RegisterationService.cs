using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities;

namespace UMS.Service
{
    public class RegisterationService
    {
        public class RegistrationService
        {
            public List<Course> GetSuggestedCourses(Student student, List<Course> allCourses, List<Registeration> previousRegistrations)
            {
                previousRegistrations ??= new List<Registeration>();

                var registeredCourseIds = previousRegistrations
                    .Where(r => r.Group != null)               // تأكدي الأول إن Group مش null
                    .Select(r => r.Group.CourseId)              // بعدين خدي CourseId
                    .ToHashSet();

                return allCourses
                    .Where(c =>
                        !registeredCourseIds.Contains(c.Id) &&  // لو الطالب مش مسجل الكورس
                        (c.PrerequisiteCourseId == null || registeredCourseIds.Contains(c.PrerequisiteCourseId.Value)) // ولو الشرط المسبق محقق
                    )
                    .ToList();
            }

            public bool IsWithinRegistrationPeriod(DateTime now, RegisterationPeriod period, StudentTimeSlot slot)
            {
                return now.Date >= period.StartDate.Date &&
                       now.Date <= period.EndDate.Date &&
                       now.TimeOfDay >= slot.StartTime &&
                       now.TimeOfDay <= slot.EndTime;
            }

            public int GetMaxUnits(Student student)
            {
                return student.Semester == 1 ? 18 : (student.GPA < 2 ? 16 : 20);
            }

            public int GetAllowedUnits(Student student)
            {
                if (student.IsFirstSemester)
                    return 18;
                return student.GPA < 2.0 ? 16 : 20;
            }

            public bool HasTimeConflict(CourseGroup newGroup, List<CourseGroup> currentGroups)
            {
                return currentGroups.Any(g =>
                    g.DayOfWeek == newGroup.DayOfWeek &&
                    ((newGroup.StartTime >= g.StartTime && newGroup.StartTime < g.EndTime) ||
                     (newGroup.EndTime > g.StartTime && newGroup.EndTime <= g.EndTime))
                );
            }

            public bool RegisterCourse(Student student, CourseGroup group, List<CourseGroup> currentGroups)
            {
                if (group.AvailableSeats <= 0 || HasTimeConflict(group, currentGroups))
                    return false;

                int totalUnits = currentGroups.Sum(g => g.Course.Units);
                if (totalUnits + group.Course.Units > GetAllowedUnits(student))
                    return false;

                group.AvailableSeats--;
                // Save to database here...

                return true;
            }

            public void DropCourse(Student student, CourseGroup group)
            {
                group.AvailableSeats++;
                // Remove from database here...
            }
        }

    }
}
