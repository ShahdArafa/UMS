using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UMS.Core;
using UMS.Core.Entities.DTOs;
using UMS.Core.Entities.Identity;
using UMS.Repository.Data;
using UMS.Repository.Data.Identity;
using UMS.Repository.Services;
using UMS.Service;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "UMS API", Version = "v1" });

    // إضافة إعدادات الـ JWT Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token.\n\nExample: Bearer abcdef12345"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//allow DInj to DbContextOptions
builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
} );
builder.Services.AddDbContext<AppIdentityDbcontext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});

//Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
 .AddEntityFrameworkStores<AppIdentityDbcontext>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOCRService, OCRService>();
builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your_Secret_Key_Here")),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });


builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins("https://localhost:44394")  // التأكد من إضافة البروتوكول الصحيح (https)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

   builder.Services.AddScoped<OCRService>();
builder.Services.Configure<EmailSetting>(
    builder.Configuration.GetSection("Email"));
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
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(Directory.GetCurrentDirectory(), "UploadedExamSchedules")),
//    RequestPath = "/UploadedExamSchedules"
//});

var uploadedFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

// مهم جدًا: إنشاء الفولدر لو مش موجود
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleInitializer.InitializeRoles(services);

   
}

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
    DataSeeding.SeedData(context);
}

app.Run();
