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
    class FacultyConfig : IEntityTypeConfiguration<Faculty>
    {
        public void Configure(EntityTypeBuilder<Faculty> builder)
        {
            builder.Property(F => F.Name).IsRequired();

           

            builder.Property(f => f.Id)
                   .IsRequired(); // أو خليها .IsRequired(false) لو مش دايمًا فيه Admin

            //builder
            //    .HasOne(f => f.Admin)
            //    .WithOne(a => a.Faculty)
            //    .HasForeignKey<Faculty>(f => f.Id);
            builder.HasBaseType<User>(); 

        }
    }
}
