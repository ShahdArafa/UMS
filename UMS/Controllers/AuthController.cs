using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UMS.Core.Entities.Identity;
using UMS.Repository.Services;
using UMS.Core.Entities.DTOs;


namespace UMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ITokenService tokenService;

        public AuthController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            this.tokenService = tokenService;
        }

        //    [HttpPost("choose-role")]
        //    public IActionResult ChooseRole([FromBody] ChooseRoleRequest request)
        //    {
        //        var validRoles = new Dictionary<string, List<string>>
        //{
        //    { "Student", new List<string> { "Name", "Email", "Password", "ConfirmPassword", "StudentId", "ProfileImage" } },
        //    { "Faculty", new List<string> { "Name", "Email", "Password", "ConfirmPassword", "Role" } },
        //    { "Admin", new List<string> { "Name", "Email", "Password", "ConfirmPassword" } }
        //};

        //        if (!validRoles.ContainsKey(request.Role))
        //        {
        //            return BadRequest(new { message = "Invalid role selected." });
        //        }

        //        return Ok(new
        //        {
        //            message = $"Role '{request.Role}' selected. Use the following fields for registration.",
        //            requiredFields = validRoles[request.Role]
        //        });
        //    }

        //1.Signup
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Core.Entities.DTOs.RegisterRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not null)
                return BadRequest(new { message = "Email is already registered." });

            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest(new { message = "Passwords do not match." });
            }

            //Role Valid Check
            var validRoles = new List<string> { "Student", "Faculty", "Admin" };
            if (!validRoles.Contains(request.Role))
            {
                return BadRequest(new { message = "Invalid role selected." });
            }

            var user = new AppUser
            {
                UserName = request.Email.Split('@')[0],
                Email = request.Email,
                Role = request.Role,
                PhoneNumber = request.PhoneNumber,
                Name = request.Name
            };

            //If Student Adding studid
            if (request.Role == "Student" && string.IsNullOrWhiteSpace(request.StudentId))
                return BadRequest(new { message = "Student ID is required for students." });

            user.StudentId = request.Role == "Student" ? request.StudentId : null;

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Name is required.");
            }

            //  creating the account
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "User creation failed.", errors = result.Errors });
            }

            // adding role to user
            await _userManager.AddToRoleAsync(user, request.Role);
            var token = await tokenService.CreateTokenAsync(user, _userManager);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role
                }
            });
        }

        //Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Core.Entities.DTOs.LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid email or password." });

            //var roles = await _userManager.GetRolesAsync(user);
            var token = await tokenService.CreateTokenAsync(user, _userManager);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role
                }
            });

        }
    }
}
