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
    public class EnrollmentConfig : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // تحديد الـ Primary Key
            builder.HasKey(e => e.Id);

            // التأكد من أن كل طالب يمكن أن يسجل في مادة واحدة فقط لمرة واحدة
            builder.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
        }
    }
}
