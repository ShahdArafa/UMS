using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using UMS.Core.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using UMS.Core.Entities;
using UMS.Service;
using UMS.Repository.Services;

namespace UMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly StoreContext context;
        private readonly OCRService _ocrService;
        private readonly EmailService _emailService;

        public AdminController(StoreContext context , OCRService ocrService , EmailService emailService)
        {
            this.context = context;
            _ocrService = ocrService;
            _emailService = emailService;
        }




        [HttpGet("all")]
        public IActionResult GetAllApplications()
        {
            var applicationsWithIsRead = context.Applications
                .OrderByDescending(a => a.SubmittedAt)
                .Select(a => new
                {
                    a.Id,
                    a.FullName,
                    a.Email,
                    a.Phone,
                    a.DesiredDepartment,
                    a.SubmittedAt,
                    a.ImagePath,
                    a.AdissmioncardPath,
                    a.HighSchoolCertificatePath,
                    a.BirthCertificatePath,

                    // نفترض إن كل إشعار مربوط بـ ApplicationId
                    IsRead = context.Notifications
                        .Where(n => n.ApplicationId == a.Id)
                        .Select(n => n.IsRead)
                        .FirstOrDefault()
                })
                .ToList();

            return Ok(applicationsWithIsRead);
        }


       // [Authorize(Roles = "Admin")]
        [HttpPost("upload-course-file")]
        public async Task<IActionResult> UploadCourseFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("يرجى رفع ملف صالح.");
            }

            try
            {
                // توليد اسم فريد للملف
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

                // إنشاء مجلد لو مش موجود
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                // حفظ الملف على السيرفر
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // توليد اللينك (لو API شغال على localhost:5000 مثلاً)
                var fileUrl = $"{Request.Scheme}://{Request.Host}/UploadedFiles/{fileName}";

                // إرسال إشعار لكل طالب
                var students = await context.Students.ToListAsync();
                var notifications = students.Select(s => new Notification
                {
                    Title = " CourseExam File Uploaded",
                    Message = " course schedule file has been uploaded. You can download it from the link below.",
                    FileUrl = fileUrl,
                    StudentId = s.Id,
                    Type = "File",
                    RelatedItemId = 0, // Optional
                    CreatedAt = DateTime.Now
                }).ToList();

                 context.Notifications.AddRange(notifications);
                await context.SaveChangesAsync();

                return Ok(new { message = " File uploaded and notifications sent successfully!", fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"An error occurred while uploading the file: {ex.Message}" });
            }
        }

       
        [HttpPost("upload-course-schedule")]
        public async Task<IActionResult> UploadCourseSchedule(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", fileName);

            try
            {
                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create a notification for the students
                var notifications = new List<Notification>();
                var studentIds = await context.Students.Select(s => s.Id).ToListAsync();

                foreach (var studentId in studentIds)
                {
                    notifications.Add(new Notification
                    {
                        Title = "New Course File Uploaded",
                        StudentId = studentId,
                        Message = "A new course schedule has been added. You can download it from the link below.",
                        FileUrl = $"{Request.Scheme}://{Request.Host}/UploadedFiles/{fileName}",
                        CreatedAt = DateTime.UtcNow,
                        Type = "File",
                        RelatedItemId = 0, // Optional
                       

                    });
                }

                // Add notifications to the database
                context.Notifications.AddRange(notifications);
                await context.SaveChangesAsync();

                return Ok(new { message = "Course schedule uploaded and notifications sent to all students..", fileUrl = $"{Request.Scheme}://{Request.Host}/UploadedFiles/{fileName}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("notifications")]
        public async Task<IActionResult> GetAdminNotifications()
        {
            var adminUserId = context.Users
                .Where(u => u.Role == "Admin")
                .Select(u => u.Id)
                .FirstOrDefault(); // لو فيه أكتر من Admin، يفضل تاخدي ID من الـ token

            var notifications = await context.Notifications
                .Where(n => n.UserId == adminUserId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }
        [HttpGet("notifications/unread-count")]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            var adminUserId = context.Users
                .Where(u => u.Role == "Admin")
                .Select(u => u.Id)
                .FirstOrDefault();

            var count = await context.Notifications
                .CountAsync(n => n.UserId == adminUserId && n.IsRead == false);

            return Ok(new { unreadCount = count });
        }

        [HttpPut("notifications/mark-as-read/{id}")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var notification = await context.Notifications.FindAsync(id);

            if (notification == null)
                return NotFound(new { message = "Notification not found" });

            notification.IsRead = true;
            context.Notifications.Update(notification);
            await context.SaveChangesAsync();

            return Ok(new { message = "Notification marked as read." });
        }
        [HttpGet("students/count-per-semester")]
        public async Task<IActionResult> GetStudentCountPerSemester()
        {
            var result = await context.Students
                .GroupBy(s => s.Semester)
                .Select(g => new
                {
                    Semester = g.Key,
                    StudentCount = g.Count()
                })
                .OrderBy(g => g.Semester)
                .ToListAsync();

            return Ok(result);
        }
        [HttpGet("faculty/count")]
        public async Task<IActionResult> GetFacultyCount()
        {
            var count = await context.Faculties.CountAsync();
            return Ok(new { facultyCount = count });
        }

        [HttpPost("events/create")]
        public async Task<IActionResult> CreateEventPost([FromForm] CreateEventPostDto dto)
        {
            string? imagePath = null;

            if (dto.Image != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EventImages");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                imagePath = $"/EventImages/{fileName}";
            }

            var eventPost = new EventPost
            {
                Title = dto.Title,
                Description = dto.Description,
                ImagePath = imagePath
                // Removed CreatedByUserId
            };

            context.eventPosts.Add(eventPost);
            await context.SaveChangesAsync();

            return Ok(new { message = "Event post created successfully" });
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetAllEventPosts()
        {
            var posts = await context.eventPosts
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    ImageUrl = $"{Request.Scheme}://{Request.Host}{e.ImagePath}",
                    e.CreatedAt,
                })
                .ToListAsync();

            return Ok(posts);
        }

        [HttpPost("application/process-ocr/{id}")]
        public async Task<IActionResult> ProcessApplicationOCR(int id)
        {
            var application = await context.Applications.FindAsync(id);
            if (application == null)
                return NotFound(new { message = "Application not found" });

            if (string.IsNullOrEmpty(application.AdissmioncardPath))
                return BadRequest(new { message = "No document image found for this application." });

            try
            {
                // استخدم OCR لتحليل الصورة
                var ocrText = await _ocrService.AnalyzeStudentImageAsync(application.AdissmioncardPath);

                if (string.IsNullOrEmpty(ocrText))
                {
                    return BadRequest(new { message = "OCR failed to extract text from the Document." });
                }

                // احفظ النص في الـ Application entity
                application.OcrResult = ocrText;

                // تحليل النتيجة لتحديد الحالة
                if (ocrText.Contains("مقبول"))
                    application.Status = "Accepted";
                else if (ocrText.Contains("عذرا"))
                    application.Status = "Rejected";
                else
                    application.Status = "Pending";

                // حفظ التحديثات في قاعدة البيانات
                await context.SaveChangesAsync();

                return Ok(new
                {
                    message = "OCR processing completed",
                    status = application.Status,
                    ocrText = application.OcrResult
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing OCR.", error = ex.Message });
            }
        }
        [HttpPost("application/generate-credentials/{applicationId}")]
        public async Task<IActionResult> GenerateCredentials(int applicationId)
        {
            var application = await context.Applications.FindAsync(applicationId);
            if (application == null || application.Status != "Accepted")
                return BadRequest(new { message = "Application not found or not accepted." });

            // Check if student already created
            var existingStudent = await context.Students
                .FirstOrDefaultAsync(s => s.ApplicationId == applicationId);
            if (existingStudent != null)
                return BadRequest(new { message = "Student already exists for this application." });

            // Step 1: Generate Student Identifier
            var year = DateTime.Now.Year.ToString(); // سنة الالتحاق
            var count = await context.Students
                .CountAsync(s => s.StudentIdentifier.StartsWith("4" + year));
            var serial = (count + 1).ToString("D3");
            var studentIdentifier = $"4{year}{serial}";

            // Step 2: Generate Email and Password
            var universityEmail = $"student{studentIdentifier}@university.edu.eg";
            var plainPassword = Guid.NewGuid().ToString().Substring(0, 8) + "Aa!"; // عشوائي
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Step 3: Create user account
            var user = new User
            {
                Email = universityEmail,
                Password = hashedPassword,
                Role = "Student"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Step 4: Create student
            var student = new Student
            {
                Name = application.FullName,
                GPA = 0,
                DepartmentName = application.DesiredDepartment,
                Semester = 1,
                StudentIdentifier = studentIdentifier,
                ApplicationId = application.Id,
                UserId = user.Id
            };
            context.Students.Add(student);
            await context.SaveChangesAsync();

            // Step 5: Send email to student
            await _emailService.SendAsync(new EmailMessage
            {
                To = application.Email,
                Subject = "Welcome to University - Your Credentials",
                Body = $"مرحبًا {application.FullName},\n\n" +
                       $"لقد تم قبولك في الجامعة بنجاح.\n\n" +
                       $"بيانات الدخول:\n" +
                       $"📧 الإيميل الجامعي: {universityEmail}\n" +
                       $"🔑 كلمة المرور: {plainPassword}\n" +
                       $"🆔 الرقم التعريفي: {studentIdentifier}\n\n" +
                       $"يرجى تغيير كلمة المرور بعد أول تسجيل دخول.\n\n" +
                       $"مع تمنياتنا بالتوفيق،\nإدارة الجامعة"
            });

            return Ok(new
            {
                message = "Student account created and credentials sent via email.",
                studentIdentifier,
                universityEmail
            });
        }
        [HttpGet("accepted-applications")]
        public async Task<IActionResult> GetAcceptedApplications()
        {
            var acceptedApplications = await context.Applications
                .Where(a => a.Status == "Accepted")
                .ToListAsync();

            return Ok(acceptedApplications);
        }

        [HttpGet("event-image/{fileName}")]
        public IActionResult GetEventImage(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EventImages", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("الصورة مش موجودة");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "image/" + Path.GetExtension(filePath).TrimStart('.');

            return File(fileBytes, contentType);
        }



    }
}
