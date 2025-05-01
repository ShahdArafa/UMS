using Microsoft.EntityFrameworkCore;
using UMS.Core.Entities;

public class StoreContext : DbContext
{
    public StoreContext(DbContextOptions<StoreContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<ExamSchedule> ExamSchedules { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Assignments> Assignments { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<TimeTable> TimeTables { get; set; }
    public DbSet<Term> Terms { get; set; }
    public DbSet<TermResult> TermResults { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<CourseResult> CourseResults { get; set; }
    public DbSet<CourseGroup> CourseGroups { get; set; }

    public DbSet<EventPost>  eventPosts { get; set; }

    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }

    public DbSet<NotificationPreference> NotificationPreferences { get; set; }
    public DbSet<Registeration> Registerations { get; set; }
    public DbSet<RegisterationPeriod> RegisterationPeriods { get; set; }
    public DbSet<StudentTimeSlot> StudentTimeSlots { get; set; }

    public DbSet<StudentAccounts> studentAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // If you rely on [InverseProperty], 
        // you typically don't need further configuration.
        // But you can also do Fluent API if you like:
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Supervisor)
            .WithMany(f => f.SupervisedStudents)
            .HasForeignKey(s => s.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<TermResult>()
           .HasOne(tr => tr.Term)
           .WithMany()
           .HasForeignKey(tr => tr.TermId)
           .OnDelete(DeleteBehavior.Restrict);
           

        // العلاقة بين Student و TermResult
        modelBuilder.Entity<TermResult>()
            .HasOne(tr => tr.Student)
            .WithMany()
            .HasForeignKey(tr => tr.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
     .Property(c => c.Units)
     .HasDefaultValue(3);

        modelBuilder.Entity<Course>()
              .HasMany(c => c.Groups)  // Course يحتوي على العديد من CourseGroups
              .WithOne(cg => cg.Course)      // وكل CourseGroup يتبع Course واحد
              .HasForeignKey(cg => cg.CourseId);

        modelBuilder.Entity<CourseGroup>()
    .HasOne(cg => cg.Faculty)
    .WithMany(f => f.Groups)
    .HasForeignKey(cg => cg.FacultyId)
    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Assignments>()
    .HasOne(a => a.CourseGroup)
    .WithMany(cg => cg.Assignments)
    .HasForeignKey(a => a.CourseGroupId)
    .OnDelete(DeleteBehavior.Restrict); // أو .NoAction

        modelBuilder.Entity<Quiz>()
    .HasOne(q => q.CourseGroup)
    .WithMany()
    .HasForeignKey(q => q.CourseGroupId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
    .HasOne(e => e.CourseGroup)
    .WithMany()
    .HasForeignKey(e => e.CourseGroupId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Application>()
       .Property(a => a.Status)
       .HasDefaultValue("Pending");

        modelBuilder.Entity<Application>()
            .Property(a => a.OcrResult)
            .HasDefaultValue("NotProcessed");

        modelBuilder.Entity<Application>()
    .HasOne(a => a.Student)
    .WithOne(s => s.Application)
    .HasForeignKey<Student>(s => s.ApplicationId);

        base.OnModelCreating(modelBuilder);


    }
}
