// Copyright (c) János Janka. All rights reserved.

using System.Data.Entity.ModelConfiguration;

namespace Partnerinfo.Portal.EntityFramework
{
    /// <summary>
    /// Configures the <see cref="PortalEntity" /> type.
    /// </summary>
    internal class PortalEntityConfiguration : EntityTypeConfiguration<PortalEntity>
    {
        public PortalEntityConfiguration()
        {
            Property(p => p.Uri).IsRequired().HasMaxLength(64).IsUnicode(false);
            Property(p => p.Domain).IsOptional().HasMaxLength(256).IsUnicode(false);
            Property(p => p.Name).IsRequired().HasMaxLength(64);
            Property(p => p.Description).IsOptional().HasMaxLength(256);
            Property(p => p.GATrackingId).IsOptional().HasMaxLength(128).IsUnicode(false);

            HasOptional(p => p.Project)
                .WithMany()
                .HasForeignKey(p => p.ProjectId)
                .WillCascadeOnDelete(true);

            HasMany(p => p.Media)
                .WithRequired()
                .HasForeignKey(p => p.PortalId)
                .WillCascadeOnDelete(true);

            HasMany(p => p.Pages)
                .WithRequired(p => p.Portal)
                .HasForeignKey(p => p.PortalId)
                .WillCascadeOnDelete(true);

            ToTable("Portal", DbSchema.Portal);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertPortal", DbSchema.Portal));
                m.Update(p => p.HasName("UpdatePortal", DbSchema.Portal));
                m.Delete(p => p.HasName("DeletePortal", DbSchema.Portal));
            });
        }
    }

    /// <summary>
    /// Configures the <see cref="PortalPage" /> type.
    /// </summary>
    internal class PortalPageConfiguration : EntityTypeConfiguration<PortalPage>
    {
        public PortalPageConfiguration()
        {
            Property(p => p.Uri).IsRequired().HasMaxLength(64).IsUnicode(false);
            Property(p => p.Name).IsRequired().HasMaxLength(64);
            Property(p => p.Description).IsOptional().HasMaxLength(256);
            Property(p => p.HtmlContent).IsOptional().HasMaxLength(131072);
            Property(p => p.StyleContent).IsOptional().HasMaxLength(131072);
            Property(p => p.ReferenceList).IsOptional().HasColumnType("xml");

            HasMany(p => p.Children)
                .WithOptional()
                .HasForeignKey(p => p.ParentId)
                .WillCascadeOnDelete(false);
            /*
            HasOptional(p => p.Parent)
               .WithMany(p => p.Children)
               .HasForeignKey(p => p.ParentId)
               .WillCascadeOnDelete(false);
            */
            HasOptional(p => p.Master)
               .WithMany(p => p.Contents)
               .HasForeignKey(p => p.MasterId)
               .WillCascadeOnDelete(false);

            ToTable("Page", DbSchema.Portal);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertPage", DbSchema.Portal));
                m.Update(p => p.HasName("UpdatePage", DbSchema.Portal));
                m.Delete(p => p.HasName("DeletePage", DbSchema.Portal));
            });
        }
    }

    /// <summary>
    /// Configures the <see cref="PortalMedia" /> type.
    /// </summary>
    internal class PortalMediaConfiguration : EntityTypeConfiguration<PortalMedia>
    {
        public PortalMediaConfiguration()
        {
            Property(p => p.Uri).IsRequired().HasMaxLength(64).IsUnicode(false);
            Property(p => p.Type).IsRequired().HasMaxLength(128).IsUnicode(false);
            Property(p => p.Name).IsRequired().HasMaxLength(256);

            HasMany(p => p.Children)
                .WithOptional()
                .HasForeignKey(p => p.ParentId)
                .WillCascadeOnDelete(false);

            ToTable("Media", DbSchema.Portal);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertMedia", DbSchema.Portal));
                m.Update(p => p.HasName("UpdateMedia", DbSchema.Portal));
                m.Delete(p => p.HasName("DeleteMedia", DbSchema.Portal));
            });
        }
    }
}
