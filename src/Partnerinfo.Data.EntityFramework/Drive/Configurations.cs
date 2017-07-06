// Copyright (c) János Janka. All rights reserved.

using System.Data.Entity.ModelConfiguration;

namespace Partnerinfo.Drive.EntityFramework
{
    /// <summary>
    /// The configuration class for the <see cref="FileItem" /> entity type.
    /// </summary>
    internal class DriveFileConfig : EntityTypeConfiguration<FileItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriveFileConfig" /> class.
        /// </summary>
        public DriveFileConfig()
        {
            Property(p => p.Type).HasColumnName("TypeId");
            Property(p => p.Name).IsRequired().HasMaxLength(256);
            Property(p => p.Slug).IsUnicode(false).IsRequired().HasMaxLength(64);
            Property(p => p.PhysicalPath).IsUnicode(false).HasMaxLength(256);

            HasMany(p => p.Children)
                .WithOptional()
                .HasForeignKey(p => p.ParentId)
                .WillCascadeOnDelete(false);

            ToTable("File", DbSchema.Drive);
        }
    }
}
