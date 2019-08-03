using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using userprofile.entities.Models;

namespace userprofile.persistence.Configuration
{

        class AdminUserModelConfiguration : IEntityTypeConfiguration<AdminUserModel>
        {
            public void Configure(EntityTypeBuilder<AdminUserModel> builder)
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.EmailAddress).IsRequired();
                builder.Property(e => e.Firstname).IsRequired();
                builder.Property(e => e.Lastname).IsRequired();
                builder.Property(e => e.Password).IsRequired();
                builder.Property(e => e.ReferredBy).IsRequired();
                builder.Property(e => e.DateCreated).IsRequired();
                builder.Property(e => e.PhoneNo).HasMaxLength(20);


                var admins = new List<AdminUserModel>()
                   {
                       new AdminUserModel
                       {
                           Id = 1,
                           EmailAddress = "superadmin@wegopay.com",
                           Firstname = "Super",
                           Lastname = "Admin",
                           Password = "admin123@@@***",
                           IsActivated = true,
                           IsActive = true,
                           IsAdmin = true,
                           PhoneNo = "08022334455",
                           ReferredBy = "Site Owner",
                           DateCreated = DateTime.UtcNow
                       }
                   };

                builder.HasData(admins.ToArray());

            }
        }
}
