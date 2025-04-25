using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using UMS.Core.Entities;
using UMS.Core.Entities.DTOs;
using UMS.Repository.Data;
using UMS.Service;

namespace UMS.Controllers
{
    [ApiController]
    [Route("api/applications")]
    public class ApplicationController : ControllerBase
    {
        private readonly StoreContext _context;
        private readonly IOCRService _OCR;

        public ApplicationController(StoreContext context, IOCRService OCR)
        {
            _context = context;
            _OCR = OCR;
        }

        [HttpPost("analyze-image")]
        public async Task<IActionResult> AnalyzeImage([FromForm] ImageUploadRequest request)
        {
            if (request.ImageFile == null || request.ImageFile.Length == 0)
                return BadRequest("لم يتم رفع صورة.");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, request.ImageFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.ImageFile.CopyToAsync(stream);
            }

            var resultMessage = await _OCR.AnalyzeStudentImageAsync(filePath);

            return Ok(new { result = resultMessage });
        }

        [HttpPost("submit-application")]
        public async Task<IActionResult> SubmitApplicationAsync([FromForm] ApplicationDto dto)
        {
            if (dto.Image == null || dto.AdmissionCard == null || dto.BirthCertificate == null || dto.HighSchoolCertificate == null)
            {
                return BadRequest("All files must be uploaded (image, required documents, high school certificate, and birth certificate).");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            async Task<string> SaveFileAsync(IFormFile file)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                return filePath;
            }

            // Save files
            var imagePath = await SaveFileAsync(dto.Image);
            var admissionCardPath = await SaveFileAsync(dto.AdmissionCard);
            var highSchoolCertificatePath = await SaveFileAsync(dto.HighSchoolCertificate);
            var birthCertificatePath = await SaveFileAsync(dto.BirthCertificate);

            // Create a new application (it doesn't mean the student is created yet)
            var application = new Application
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                DesiredDepartment = dto.DesiredDepartment,
                SubmittedAt = DateTime.Now,
                ImagePath = imagePath,
                AdissmioncardPath = admissionCardPath,
                HighSchoolCertificatePath = highSchoolCertificatePath,
                BirthCertificatePath = birthCertificatePath,
                StudentId = null // Initially null as the student will be added after approval
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            // Notify the admin about the new application
            var adminUserId = _context.Users.FirstOrDefault(u => u.Role == "Admin")?.Id;
            if (adminUserId != null)
            {
                var notification = new Notification
                {
                    Type="Application",
                    Title ="New Student Application",
                    UserId = adminUserId.Value, // Assumed the first Admin user here
                    Message = "A new student application has been submitted. Please review it.",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "Application submitted successfully. The admin will review your application.",
                imageUrl = $"{Request.Scheme}://{Request.Host}/UploadedFiles/{Path.GetFileName(imagePath)}",
                admissionCardUrl = $"{Request.Scheme}://{Request.Host}/UploadedFiles/{Path.GetFileName(admissionCardPath)}",
                highSchoolCertificateUrl = $"{Request.Scheme}://{Request.Host}/UploadedFiles/{Path.GetFileName(highSchoolCertificatePath)}",
                birthCertificateUrl = $"{Request.Scheme}://{Request.Host}/UploadedFiles/{Path.GetFileName(birthCertificatePath)}"
            });
        }



        //private string SaveFiles(IFormFile file)
        //    {
        //        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        //        if (!Directory.Exists(uploadsFolder))
        //        {
        //            Directory.CreateDirectory(uploadsFolder);
        //        }

        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        //        var filePath = Path.Combine(uploadsFolder, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            file.CopyTo(stream);
        //        }

        //        return filePath;
        //    }

    }

    }
