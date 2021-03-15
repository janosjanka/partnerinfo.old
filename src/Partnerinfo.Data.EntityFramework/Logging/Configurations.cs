// Copyright (c) János Janka. All rights reserved.

using System.Data.Entity.ModelConfiguration;

namespace Partnerinfo.Logging.EntityFramework
{
    /// <summary>
    /// Provides the base class for configuring <see cref="LoggingCategory" /> entity type.
    /// </summary>
    internal class LoggingCategoryConfiguration : EntityTypeConfiguration<LoggingCategory>
    {
        public LoggingCategoryConfiguration()
        {
            Property(p => p.Name).IsRequired().HasMaxLength(64);

            HasOptional(p => p.Project)
                .WithMany()
                .HasForeignKey(p => p.ProjectId)
                .WillCascadeOnDelete(false);

            HasMany(p => p.Events).WithOptional().HasForeignKey(p => p.CategoryId);

            ToTable("Category", DbSchema.Logging);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertCategory", DbSchema.Logging));
                m.Update(p => p.HasName("UpdateCategory", DbSchema.Logging));
                m.Delete(p => p.HasName("DeleteCategory", DbSchema.Logging));
            });
        }
    }

    /// <summary>
    /// Provides the base class for configuring <see cref="LoggingEvent" /> entity type.
    /// </summary>
    internal class LoggingEventConfiguration : EntityTypeConfiguration<LoggingEvent>
    {
        public LoggingEventConfiguration()
        {
            Property(p => p.ObjectType).HasColumnName("ObjectTypeId");
            Property(p => p.ContactState).HasColumnName("ContactStateId");
            Property(p => p.StartDate).HasPrecision(0);
            Property(p => p.FinishDate).HasPrecision(0);
            Property(p => p.BrowserBrand).HasColumnName("BrowserBrandId");
            Property(p => p.MobileDevice).HasColumnName("MobileDeviceId");
            Property(p => p.ClientId).IsUnicode(false).HasMaxLength(64);
            Property(p => p.CustomUri).IsUnicode(false).HasMaxLength(64);
            Property(p => p.ReferrerUrl).IsUnicode(false).HasMaxLength(256);

            HasMany(p => p.Users).WithRequired().HasForeignKey(p => p.EventId);

            ToTable("Event", DbSchema.Logging);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertEvent", DbSchema.Logging));
                m.Update(p => p.HasName("UpdateEvent", DbSchema.Logging));
                m.Delete(p => p.HasName("DeleteEvent", DbSchema.Logging));
            });
        }
    }

    /// <summary>
    /// Provides the base class for configuring <see cref="LoggingEventSharing" /> entity type.
    /// </summary>
    internal class LoggingEventSharingConfiguration : EntityTypeConfiguration<LoggingEventSharing>
    {
        public LoggingEventSharingConfiguration()
        {
            HasKey(p => new { p.UserId, p.EventId });

            ToTable("EventSharing", DbSchema.Logging);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertEventSharing", DbSchema.Logging));
                m.Delete(p => p.HasName("DeleteEventSharing", DbSchema.Logging));
            });
        }
    }

    /// <summary>
    /// Provides the base class for configuring <see cref="LoggingRule" /> entity type.
    /// </summary>
    internal class LoggingRuleConfiguration : EntityTypeConfiguration<LoggingRule>
    {
        public LoggingRuleConfiguration()
        {
            ToTable("Rule", DbSchema.Logging);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertRule", DbSchema.Logging));
                m.Update(p => p.HasName("UpdateRule", DbSchema.Logging));
                m.Delete(p => p.HasName("DeleteRule", DbSchema.Logging));
            });
        }
    }
}
