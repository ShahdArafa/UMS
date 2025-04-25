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
    public class TaskConfig :IEntityTypeConfiguration<Assignments>
    {
        public void Configure(EntityTypeBuilder<Assignments> builder)
        {
            //builder.HasKey(t => t.Id);

            //builder.Property(t => t.Title)
            //       .IsRequired()
            //       .HasMaxLength(255);

            //builder.Property(t => t.Description)
            //       .HasMaxLength(1000);

            //builder.HasOne(t => t.Course)
            //       .WithMany(c => c.Tasks)
            //       .HasForeignKey(t => t.CourseId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
