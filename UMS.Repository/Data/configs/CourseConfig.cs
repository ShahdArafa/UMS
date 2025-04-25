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
    public class CourseConfig //: IEntityTypeConfiguration<Course>
    {
        //public void Configure(EntityTypeBuilder<Course> builder)
        //{
        //    // تحديد الـ Primary Key
        //    builder.HasKey(c => c.Id);

        //    // تحديد العلاقة بين المادة والتسجيل في المواد (Many-to-Many)
        //    builder.HasMany(c => c.Enrollments)
        //           .WithOne(e => e.Course)
        //           .HasForeignKey(e => e.CourseId)
        //           .OnDelete(DeleteBehavior.Cascade);

        //    // تحديد العلاقة بين المادة والمهام (One-to-Many)
        //    builder.HasMany(c => c.Tasks)
        //           .WithOne(t => t.Course)
        //           .HasForeignKey(t => t.CourseId)
        //           .OnDelete(DeleteBehavior.Cascade);

        //    // تحديد العلاقة بين المادة والكويزات (One-to-Many)
        //    builder.HasMany(c => c.Quizzes)
        //           .WithOne(q => q.Course)
        //           .HasForeignKey(q => q.CourseId)
        //           .OnDelete(DeleteBehavior.Cascade);
        //}
    }

}
