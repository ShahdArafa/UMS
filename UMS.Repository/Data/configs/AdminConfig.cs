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
    class AdminConfig : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
        //    builder.Property(A => A.Name).IsRequired();
        //    builder.HasOne(a => a.Faculty)
        //.WithOne(f => f.Admin)
        //.HasForeignKey<Faculty>(f => f.Id);

            builder.HasBaseType<User>();
        }
    }
}
