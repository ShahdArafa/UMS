using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities;

namespace UMS.Repository.Data.configs
{
    internal class ApplicationConfig
    {
        public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
        {
            public void Configure(EntityTypeBuilder<Application> builder)
            {
                builder.ToTable("Applications");
                builder.HasKey(a => a.Id);



                //// تحديد المفتاح الخارجي للعلاقة بين Application و Student
                //builder.HasOne(a => a.Student) // ارتباط Application بـ Student
                //    .WithOne(s => s.Application) // ارتباط Student بـ Application
                //    .HasForeignKey<Application>(a => a.StudentId) // المفتاح الخارجي في Application
                //    .OnDelete(DeleteBehavior.Cascade); 

                // إضافة قيد على أن لكل طالب طلب واحد فقط
                builder.HasIndex(a => a.StudentId).IsUnique();


                builder.Property(a => a.FullName)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.Property(a=> a.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                builder.Property(a=> a.Phone)
                    .IsRequired();

               

            }
        }
    }
}