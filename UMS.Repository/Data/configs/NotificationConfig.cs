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
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            //// تحديد العلاقة بين الإشعار والطالب
            //builder.HasOne(n => n.Student)
            //       .WithMany(s => s.Notifications)
            //       .HasForeignKey(n => n.StudentId)
            //       .OnDelete(DeleteBehavior.Cascade);

            // تحديد العلاقة بين الإشعار والمادة
            builder.HasOne(n => n.Course)
                   .WithMany()
                   .HasForeignKey(n => n.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
