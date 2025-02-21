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
            builder.Property(S => S.ImageUrl).IsRequired();
            builder.Property(S => S.StudentId).IsRequired();
            builder.Property(S => S.Supervisor).IsRequired();

        }
    }
}
