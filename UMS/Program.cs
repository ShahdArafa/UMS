using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Security.Claims;
using System.Text;
using UMS.Core;
using UMS.Core.Entities.DTOs;
using UMS.Core.Entities.Identity;
using UMS.Repository.Data;
using UMS.Repository.Data.Identity;
using UMS.Repository.Services;
using UMS.Service;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddLogging(logging => {
    logging.AddConsole();
    logging.AddDebug();
});
builder.Services.AddScoped<IOCRService, OCRService>();
builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<StoreContext>()
    .AddDefaultTokenProviders();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://localhost:7044", "http://localhost:5282")  // التأكد من إضافة البروتوكول الصحيح (https)
               .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<EmailService>();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();

// إضافة خدمات Authorization للسياسات
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Student").RequireClaim(ClaimTypes.NameIdentifier));
    options.AddPolicy("FacultyPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Faculty").RequireClaim(ClaimTypes.NameIdentifier));
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin").RequireClaim(ClaimTypes.NameIdentifier));//RequireRole("Admin"));
});

//allow DInj to DbContextOptions
builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<AppIdentityDbcontext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});




#region Authorization
// Define Swagger generation options and add Bearer token authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWT Auth Sample",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer jhfdkj.jkdsakjdsa.jkdsajk\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT Key is missing or empty in appsettings.json. Please check the configuration.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            // RoleClaimType = ClaimTypes.Role // ✅ ضروري
        };
    });

#endregion


// 1. Build
var app = builder.Build();

// 2. Environment-specific
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. HTTPS
app.UseHttpsRedirection();

// 4. CORS
app.UseCors("AllowAll");

// 5. Static Files
app.UseStaticFiles(); // wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles")),
    RequestPath = "/UploadedFiles"
});

// Ensure uploads folder exists
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
if (!Directory.Exists(uploadsPath))
    Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// 6. Authentication / Authorization
app.UseAuthentication();
app.UseAuthorization();

// 7. Map Controllers
app.MapControllers();

// 8. Seed Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
    DataSeeding.SeedData(context);
}

// 9. Run
app.Run();
