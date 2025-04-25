using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities;
using System.Reflection.Emit;

namespace UMS.Repository.Data.configs
{
    public class UserConfig :  IEntityTypeConfiguration<User>
    {
       
        
            public void Configure(EntityTypeBuilder<User> builder)
            {
                 builder.HasKey(u => u.Id);

            builder
           .HasDiscriminator<string>("UserType")
           .HasValue<Admin>("Admin")
           .HasValue<Faculty>("Faculty")
           .HasValue<Student>("Student");


            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            //builder.Property(u => u.PasswordHash)
            //    .IsRequired();

            builder.Property(u => u.Role)
                .IsRequired();


            }
        
    }
}
