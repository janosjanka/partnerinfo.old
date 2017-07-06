// Copyright (c) János Janka. All rights reserved.

using System.Data.Entity;
using Partnerinfo.Drive;
using Partnerinfo.Drive.EntityFramework;
using Partnerinfo.Identity.EntityFramework;
using Partnerinfo.Input.EntityFramework;
using Partnerinfo.Logging.EntityFramework;
using Partnerinfo.Media.EntityFramework;
using Partnerinfo.Portal.EntityFramework;
using Partnerinfo.Project.EntityFramework;
using Partnerinfo.Security.EntityFramework;

namespace Partnerinfo
{
    /// <summary>
    /// A DbContext instance represents a combination of the Unit Of Work and Repository
    /// patterns such that it can be used to query from a database and group together
    /// changes that will then be written back to the store as a unit.
    /// DbContext is conceptually similar to ObjectContext.
    /// </summary>
    public sealed class PartnerDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerDbContext" /> class.
        /// </summary>
        public PartnerDbContext()
            : base()
        {
#if DEBUG
            Database.Log = s => System.Diagnostics.Trace.WriteLine(s, "SQL");
#endif
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = true;
        }

        public DbSet<IdentityUser> Users { get; set; }

        public DbSet<LoggingCategory> LoggingCategories { get; set; }
        public DbSet<LoggingEvent> LoggingEvents { get; set; }
        public DbSet<LoggingRule> LoggingRules { get; set; }

        public DbSet<CommandEntity> InputCommands { get; set; }

        public DbSet<FileItem> DriveDocuments { get; set; }

        public DbSet<MediaPlaylist> MediaPlaylists { get; set; }
        public DbSet<MediaPlaylistItem> MediaPlaylistItems { get; set; }

        public DbSet<ProjectEntity> Projects { get; set; }
        public DbSet<ProjectAction> ProjectActions { get; set; }
        public DbSet<ProjectContact> ProjectContacts { get; set; }
        public DbSet<ProjectMailMessage> ProjectMailMessages { get; set; }
        public DbSet<ProjectBusinessTag> ProjectBusinessTags { get; set; }

        public DbSet<PortalEntity> Portals { get; set; }
        public DbSet<PortalMedia> PortalMedia { get; set; }
        public DbSet<PortalPage> PortalPages { get; set; }

        public DbSet<SecurityAccessRule> SecurityAccessRules { get; set; }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.
        /// </remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Database.SetInitializer<PartnerDbContext>(null);

            modelBuilder.Ignore<UniqueItem>();
            modelBuilder.Ignore<ResourceItem>();

            modelBuilder.Conventions.Add(new DateTime2Convention());
            modelBuilder.Conventions.Add(new PortalFunctionsConvention());

            modelBuilder.Configurations.Add(new MailAddressItemConfiguration());
            modelBuilder.Configurations.Add(new PhoneGroupItemConfiguration());

            modelBuilder.Configurations.Add(new IdentityUserConfig());

            modelBuilder.Configurations.Add(new LoggingCategoryConfiguration());
            modelBuilder.Configurations.Add(new LoggingEventConfiguration());
            modelBuilder.Configurations.Add(new LoggingEventSharingConfiguration());
            modelBuilder.Configurations.Add(new LoggingRuleConfiguration());

            modelBuilder.Configurations.Add(new CommandEntityConfiguration());
            modelBuilder.Configurations.Add(new DriveFileConfig());

            modelBuilder.Configurations.Add(new MediaPlaylistConfiguration());
            modelBuilder.Configurations.Add(new MediaPlaylistItemConfiguration());

            modelBuilder.Configurations.Add(new ProjectActionConfiguration());
            modelBuilder.Configurations.Add(new ProjectEntityConfiguration());
            modelBuilder.Configurations.Add(new ProjectContactConfiguration());
            modelBuilder.Configurations.Add(new ProjectContactTagConfiguration());
            modelBuilder.Configurations.Add(new ProjectMailMessageConfiguration());
            modelBuilder.Configurations.Add(new ProjectBusinessTagConfiguration());

            modelBuilder.Configurations.Add(new PortalEntityConfiguration());
            modelBuilder.Configurations.Add(new PortalMediaConfiguration());
            modelBuilder.Configurations.Add(new PortalPageConfiguration());

            modelBuilder.Configurations.Add(new SecurityAccessRuleConfig());
        }
    }
}
