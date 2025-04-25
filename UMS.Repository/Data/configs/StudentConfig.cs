using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities;

namespace UMS.Repository.Data.configs
{
    class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(S => S.Name).IsRequired();
            builder.Property(S => S.Image).IsRequired(false);
            builder.Property(S => S.Id).IsRequired();
            builder.Property(S => S.Supervisor).IsRequired();

           

            // التأكد من أن البريد الإلكتروني فريد
            //builder.HasIndex(s => s.Email).IsUnique();

            // تحديد العلاقة بين الطالب والتسجيل في المواد (Many-to-Many)
            builder.HasMany(s => s.Enrollments)
                   .WithOne(e => e.Student)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // تحديد العلاقة بين الطالب والإشعارات (One-to-Many)
            //builder.HasMany(s => s.Notifications)
            //       .WithOne(n => n.Student)
            //       .HasForeignKey(n => n.StudentId)
            //       .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}
