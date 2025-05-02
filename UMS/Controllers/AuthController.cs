//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity.Data;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using UMS.Core.Entities.Identity;
//using UMS.Repository.Services;
//using UMS.Core.Entities.DTOs;


//namespace UMS.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly UserManager<AppUser> _userManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly IConfiguration _config;
//        private readonly ITokenService tokenService;

//        public AuthController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, ITokenService tokenService)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _config = config;
//            this.tokenService = tokenService;
//        }

//        //    [HttpPost("choose-role")]
//        //    public IActionResult ChooseRole([FromBody] ChooseRoleRequest request)
//        //    {
//        //        var validRoles = new Dictionary<string, List<string>>
//        //{
//        //    { "Student", new List<string> { "Name", "Email", "Password", "ConfirmPassword", "StudentId", "ProfileImage" } },
//        //    { "Faculty", new List<string> { "Name", "Email", "Password", "ConfirmPassword", "Role" } },
//        //    { "Admin", new List<string> { "Name", "Email", "Password", "ConfirmPassword" } }
//        //};

//        //        if (!validRoles.ContainsKey(request.Role))
//        //        {
//        //            return BadRequest(new { message = "Invalid role selected." });
//        //        }

//        //        return Ok(new
//        //        {
//        //            message = $"Role '{request.Role}' selected. Use the following fields for registration.",
//        //            requiredFields = validRoles[request.Role]
//        //        });
//        //    }

//        //1.Signup
//        [HttpPost("register")]
//        public async Task<IActionResult> Register([FromBody] Core.Entities.DTOs.RegisterRequest request)
//        {
//            if (await _userManager.FindByEmailAsync(request.Email) is not null)
//                return BadRequest(new { message = "Email is already registered." });

//            if (request.Password != request.ConfirmPassword)
//            {
//                return BadRequest(new { message = "Passwords do not match." });
//            }

//            //Role Valid Check
//            var validRoles = new List<string> { "Student", "Faculty", "Admin" };
//            if (!validRoles.Contains(request.Role))
//            {
//                return BadRequest(new { message = "Invalid role selected." });
//            }

//            var user = new AppUser
//            {
//                UserName = request.Email.Split('@')[0],
//                Email = request.Email,
//                Role = request.Role,
//                PhoneNumber = request.PhoneNumber,
//                Name = request.Name
//            };

//            //If Student Adding studid
//            if (request.Role == "Student" && string.IsNullOrWhiteSpace(request.StudentId))
//                return BadRequest(new { message = "Student ID is required for students." });

//            user.StudentId = request.Role == "Student" ? request.StudentId : null;

//            if (string.IsNullOrWhiteSpace(request.Name))
//            {
//                return BadRequest("Name is required.");
//            }

//            //  creating the account
//            var result = await _userManager.CreateAsync(user, request.Password);
//            if (!result.Succeeded)
//            {
//                return BadRequest(new { message = "User creation failed.", errors = result.Errors });
//            }

//            // adding role to user
//            await _userManager.AddToRoleAsync(user, request.Role);
//            var token = await tokenService.CreateTokenAsync(user, _userManager);

//            return Ok(new
//            {
//                token,
//                user = new
//                {
//                    user.Id,
//                    user.Name,
//                    user.Email,
//                    user.Role
//                }
//            });
//        }

//        //Login
//       [ApiController]
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using UMS.Core.Entities;
using UMS.Core.Entities.DTOs;
using UMS.Core.Entities.Identity;
using UMS.Service;

[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly StoreContext _context;
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;

    public AuthController(StoreContext context, IConfiguration configuration, UserManager<AppUser> userManager)
    {
        _context = context;
        _configuration = configuration;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Login(Microsoft.AspNetCore.Identity.Data.LoginRequest loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Password == loginDto.Password);

        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });

        var token = GenerateJwtToken(user);

        var response = new AuthResponse
        {
            Token = token,
            FullName = user.FullName,
            Role = user.Role,
            UserId = user.Id,
            UserEmail = user.Email
        };

        if (user.Role == "Student")
        {
            var student = await _context.Students
                .Where(s => s.UserId == user.Id)
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync();

            if (student != null)
            {

                response.StudentInfo = new StudentHomeDto
                {
                    StudentIdentifier = student.StudentIdentifier,
                    Name = student.Name,
                    GPA = student.GPA,
                    TotalUnits = student.TotalUnits.GetValueOrDefault(),

                    Courses = student.Enrollments.Select(e => new CourseDto
                    {
                        Id = e.Course.Id,
                        Name = e.Course.Name,
                        Department = e.Course.Department
                    }).ToList()
                };
            }
        }

        return Ok(response);
    }


    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
 {
     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
     new Claim(ClaimTypes.Email, user.Email),
     new Claim(ClaimTypes.Role, user.Role)
 };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
        {
            return NotFound(new { message = "User with this email does not exist." });
        }

        // Generate 5-digit OTP
        var otp = new Random().Next(10000, 99999).ToString();

        // Store OTP & Expiry
        user.ResetOtp = otp;
        user.ResetOtpExpiry = DateTime.UtcNow.AddMinutes(10);
        await _context.SaveChangesAsync();

        // Send Email
        var emailService = new EmailService();
        var subject = "Your OTP Code";
        var body = $"Your OTP code is: {otp}. It will expire in 10 minutes.";
        await emailService.SendEmailAsync(user.Email, subject, body);

        return Ok(new { message = "OTP has been sent to your email." });
    }

    public class EmailService
    {
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int smtpPort = 587;
        private readonly string senderEmail = "shahd.arafa.03@gmail.com";
        private readonly string senderPassword = "onjpklachtnmaqhv"; // لازم يكون App Password لو Gmail

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                var mailMessage = new MailMessage(senderEmail, toEmail, subject, body)
                {
                    IsBodyHtml = true // لو بترسل HTML
                };

                await client.SendMailAsync(mailMessage);
            }
        }
    }

    //[AllowAnonymous]
    //[HttpPost("verify-otp")]
    //public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto model)
    //{
    //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
    //    if (user == null)
    //    {
    //        return NotFound(new { message = "User with this email does not exist." });
    //    }

    //    if (user.ResetOtp != model.Otp || user.ResetOtpExpiry < DateTime.UtcNow)
    //    {
    //        return BadRequest(new { message = "Invalid or expired OTP." });
    //    }

    //    // OTP verified — Clear it
    //    user.ResetOtp = null;
    //    user.ResetOtpExpiry = null;
    //    await _context.SaveChangesAsync();

    //    return Ok(new { message = "OTP verified successfully. You can now reset your password." });
    //}

    public class VerifyOtpDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
