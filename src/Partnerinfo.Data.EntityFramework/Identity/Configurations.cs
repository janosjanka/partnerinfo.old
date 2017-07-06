// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Partnerinfo.Identity.EntityFramework
{
    /// <summary>
    /// The configuration class for the <see cref="IdentityUser" /> entity type.
    /// </summary>
    internal class IdentityUserConfig : EntityTypeConfiguration<IdentityUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserConfig" /> class.
        /// </summary>
        public IdentityUserConfig()
        {
            // Allow different nullability (and other facets) for properties of complex types in different usages
            // https://entityframework.codeplex.com/workitem/1247
            // If you want to be as fit as a fiddle, do not change nullability of a complex property.
            Property(p => p.Email.Address).HasColumnName("Email")
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Email") { IsUnique = true }));
            Property(p => p.Email.Name).IsOptional().HasColumnName("Name");
            Property(p => p.FirstName).HasMaxLength(64);
            Property(p => p.LastName).HasMaxLength(64);
            Property(p => p.NickName).HasMaxLength(64);
            Property(p => p.Birthday).HasColumnType("date");
            Property(p => p.LastEventDate).HasPrecision(0);
            Property(p => p.LastIPAddress).IsUnicode(false).HasMaxLength(64);
            Property(p => p.LastLoginDate).HasPrecision(0);

            HasMany(p => p.Events).WithRequired().HasForeignKey(p => p.UserId);

            ToTable("User");
        }
    }
}
