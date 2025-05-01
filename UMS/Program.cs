using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using UMS.Core;
using UMS.Core.Entities.DTOs;
using UMS.Core.Entities.Identity;
using UMS.Repository.Data;
using UMS.Repository.Data.Identity;
using UMS.Repository.Services;
using UMS.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo { Title = "UMS API", Version = "v1" });
//});

//// إضافة خدمات Authorization للسياسات
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("StudentPolicy", policy => policy.RequireRole("Student"));
//    options.AddPolicy("FacultyPolicy", policy => policy.RequireRole("Faculty"));
//    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
//});

//allow DInj to DbContextOptions
builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<AppIdentityDbcontext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});

// Identity
//builder.Services.AddIdentity<AppUser, IdentityRole>()
//    .AddEntityFrameworkStores<AppIdentityDbcontext>();

////builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOCRService, OCRService>();
builder.Services.AddScoped<ICourseService, CourseService>();

//builder.Services.AddScoped<ICourseService, CourseService>();

// Authentication and Authorization
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your_Secret_Key_Here")),
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.Zero
//        };
//    });

// Add config for JWT bearer token
//builder.Services.AddAuthentication(opt =>
//{
//opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(opt =>
//{
//opt.RequireHttpsMetadata = false; // for development only
//opt.SaveToken = true;
//opt.TokenValidationParameters = new TokenValidationParameters
//{
//ValidateIssuerSigningKey = true,
//IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:SecretKey"])),
//ValidateIssuer = true,
//ValidIssuer = builder.Configuration["JWT:Issuer"],
//ValidateAudience = true,
//ValidAudience = builder.Configuration["JWT:Audience"],
//};
//});

builder.Services.AddEndpointsApiExplorer();

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
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();


//builder.Services.AddIdentity<AppUser, IdentityRole>()
//    .AddEntityFrameworkStores<StoreContext>()
//    .AddDefaultTokenProviders();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://localhost:44394")  // التأكد من إضافة البروتوكول الصحيح (https)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//builder.Services.AddScoped<OCRService>();
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<EmailService>();



var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    if (context.Request.Scheme == "https")
    {
        context.Response.Redirect("http://" + context.Request.Host + context.Request.Path + context.Request.QueryString);
        return;
    }
    await next();
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles")),
    RequestPath = "/UploadedFiles"
});

// مهم جدًا: إنشاء الفولدر لو مش موجود
var uploadedFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
if (!Directory.Exists(uploadedFilesPath))
{
    Directory.CreateDirectory(uploadedFilesPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadedFilesPath),
    RequestPath = "/uploads"
});

app.UseAuthentication();
app.UseAuthorization();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await RoleInitializer.InitializeRoles(services);
//}

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
    DataSeeding.SeedData(context);
}

app.Run();