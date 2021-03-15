// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Partnerinfo.Project.EntityFramework
{
    /// <summary>
    /// The configuration class for the <see cref="ProjectEntity" /> entity type.
    /// </summary>
    internal class ProjectEntityConfiguration : EntityTypeConfiguration<ProjectEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEntityConfiguration" /> class.
        /// </summary>
        public ProjectEntityConfiguration()
        {
            Property(p => p.Name).IsRequired().HasMaxLength(256);

            // Allow different nullability (and other facets) for properties of complex types in different usages
            // https://entityframework.codeplex.com/workitem/1247
            // If you want to be as fit as a fiddle, do not change nullability of a complex property.

            Property(p => p.Sender.Address)/*.IsRequired()*/.HasColumnName("SenderEmail");
            Property(p => p.Sender.Name).HasColumnName("SenderName");

            ToTable("Project", DbSchema.Project);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertProject", DbSchema.Project));
                m.Update(p => p.HasName("UpdateProject", DbSchema.Project));
                m.Delete(p => p.HasName("DeleteProject", DbSchema.Project));
            });
        }
    }

    /// <summary>
    /// The configuration class for the <see cref="ProjectAction" /> entity type.
    /// </summary>
    internal class ProjectActionConfiguration : EntityTypeConfiguration<ProjectAction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectActionConfiguration" /> class.
        /// </summary>
        public ProjectActionConfiguration()
        {
            //HasOptional(p => p.Parent)
            //    .WithMany(p => p.Children)
            //    .HasForeignKey(p => p.ParentId)
            //    .WillCascadeOnDelete(false);

            HasRequired(p => p.Project)
                .WithMany(p => p.Actions)
                .HasForeignKey(p => p.ProjectId)
                .WillCascadeOnDelete(true);

            HasMany(p => p.Children)
                .WithOptional()
                .HasForeignKey(p => p.ParentId)
                .WillCascadeOnDelete(false);

            Property(p => p.Type).HasColumnName("TypeId");
            Property(p => p.Name).HasMaxLength(256);
            Property(p => p.Options).HasColumnType("XML");

            ToTable("Action", DbSchema.Project);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertAction", DbSchema.Project));
                m.Update(p => p.HasName("UpdateAction", DbSchema.Project));
                m.Delete(p => p.HasName("DeleteAction", DbSchema.Project));
            });
        }
    }

    /// <summary>
    /// The configuration class for the <see cref="ProjectBusinessTag" /> entity type.
    /// </summary>
    internal class ProjectBusinessTagConfiguration : EntityTypeConfiguration<ProjectBusinessTag>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectContactConfiguration" /> class.
        /// </summary>
        public ProjectBusinessTagConfiguration()
        {
            HasRequired(p => p.Project)
                .WithMany(p => p.BusinessTags)
                .HasForeignKey(p => p.ProjectId)
                .WillCascadeOnDelete(false);

            Property(p => p.Name).IsRequired().HasMaxLength(64);
            Property(p => p.Color).IsUnicode(false).HasMaxLength(16);

            HasMany(p => p.Contacts)
                .WithRequired()
                .HasForeignKey(p => p.BusinessTagId);

            ToTable("BusinessTag", DbSchema.Project);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertBusinessTag", DbSchema.Project));
                m.Update(p => p.HasName("UpdateBusinessTag", DbSchema.Project));
                m.Delete(p => p.HasName("DeleteBusinessTag", DbSchema.Project));
            });
        }
    }

    /// <summary>
    /// The configuration class for the <see cref="ProjectContact" /> entity type.
    /// </summary>
    internal class ProjectContactConfiguration : EntityTypeConfiguration<ProjectContact>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectContactConfiguration" /> class.
        /// </summary>
        public ProjectContactConfiguration()
        {
            HasRequired(p => p.Project)
               .WithMany(p => p.Contacts)
               .HasForeignKey(p => p.ProjectId)
               .WillCascadeOnDelete(true);

            HasMany(p => p.BusinessTags)
                .WithRequired()
                .HasForeignKey(p => p.ContactId);

            HasMany(p => p.Playlists)
                .WithRequired()
                .HasForeignKey(p => p.ContactId)
                .WillCascadeOnDelete(true);

            Property(p => p.ProjectId).HasColumnAnnotation(
               IndexAnnotation.AnnotationName,
               new IndexAnnotation(new IndexAttribute("IX_ProjectId_Email")
               {
                   Order = 0,
                   IsUnique = false
               }));
            // Allow different nullability (and other facets) for properties of complex types in different usages
            // https://entityframework.codeplex.com/workitem/1247
            // If you want to be as fit as a fiddle, do not change nullability of a complex property.
            Property(p => p.Email.Address).HasColumnName("Email").HasColumnAnnotation(
                IndexAnnotation.AnnotationName,
                new IndexAnnotation(new IndexAttribute("IX_ProjectId_Email")
                {
                    Order = 1,
                    IsUnique = false
                }));
            Property(p => p.Email.Name).IsOptional().HasColumnName("Name");
            Property(p => p.FirstName).HasMaxLength(64);
            Property(p => p.LastName).HasMaxLength(64);
            Property(p => p.NickName).HasMaxLength(64);
            Property(p => p.Birthday).HasColumnType("date");
            Property(p => p.Phones.Personal).HasColumnName("PersonalPhone");
            Property(p => p.Phones.Business).HasColumnName("BusinessPhone");
            Property(p => p.Phones.Mobile).HasColumnName("MobilePhone");
            Property(p => p.Phones.Other).HasColumnName("OtherPhone");
            Property(p => p.Comment).HasMaxLength(512);

            ToTable("Contact", DbSchema.Project);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertContact", DbSchema.Project));
                m.Update(p => p.HasName("UpdateContact", DbSchema.Project));
                m.Delete(p => p.HasName("DeleteContact", DbSchema.Project));
            });
        }
    }

    /// <summary>
    /// Configures the <see cref="ProjectContactTag" /> type.
    /// </summary>
    internal class ProjectContactTagConfiguration : EntityTypeConfiguration<ProjectContactTag>
    {
        public ProjectContactTagConfiguration()
        {
            HasKey(p => new { p.BusinessTagId, p.ContactId });

            ToTable("ContactTag", DbSchema.Project);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertContactTag", DbSchema.Logging));
                m.Delete(p => p.HasName("DeleteContactTag", DbSchema.Logging));
            });
        }
    }

    /// <summary>
    /// The configuration class for the <see cref="ProjectMailMessage" /> entity type.
    /// </summary>
    internal class ProjectMailMessageConfiguration : EntityTypeConfiguration<ProjectMailMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectMailMessageConfiguration" /> class.
        /// </summary>
        public ProjectMailMessageConfiguration()
        {
            HasRequired(p => p.Project)
                .WithMany(p => p.MailMessages)
                .HasForeignKey(p => p.ProjectId)
                .WillCascadeOnDelete(true);

            Property(p => p.Subject).IsRequired().HasMaxLength(256);
            Property(p => p.Body).HasMaxLength(65536);

            ToTable("MailMessage", DbSchema.Project);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertMailMessage", DbSchema.Project));
                m.Update(p => p.HasName("UpdateMailMessage", DbSchema.Project));
                m.Delete(p => p.HasName("DeleteMailMessage", DbSchema.Project));
            });
        }
    }
}
