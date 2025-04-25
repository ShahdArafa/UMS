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
    public class QuizConfigcs :IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.HasKey(q => q.Id);

            // تحديد العلاقة بين الكويز والمادة
            //builder.HasOne(q => q.Course)
            //       .WithMany(c => c.Quizzes)
            //       .HasForeignKey(q => q.CourseId)
            //       .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
