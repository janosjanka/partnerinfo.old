// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Partnerinfo.Media.EntityFramework
{
    /// <summary>
    /// The configuration class for the <see cref="MediaPlaylist" /> entity type.
    /// </summary>
    internal sealed class MediaPlaylistConfiguration : EntityTypeConfiguration<MediaPlaylist>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlaylistConfiguration" /> class.
        /// </summary>
        public MediaPlaylistConfiguration()
        {
            Property(p => p.Name).IsRequired().HasMaxLength(256);
            Property(p => p.Uri).IsUnicode(false).IsRequired().HasMaxLength(64)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Playlist_Uri")
                    {
                        IsUnique = true
                    }));
            Property(p => p.EditMode).HasColumnName("EditModeId");

            HasMany(p => p.Items)
                .WithRequired()
                .HasForeignKey(p => p.PlaylistId)
                .WillCascadeOnDelete(true);

            ToTable("Playlist", DbSchema.Media);
        }
    }

    /// <summary>
    /// The configuration class for the <see cref="MediaPlaylistItem" /> entity type.
    /// </summary>
    internal sealed class MediaPlaylistItemConfiguration : EntityTypeConfiguration<MediaPlaylistItem>
    {
        public MediaPlaylistItemConfiguration()
        {
            Property(p => p.SortOrderId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            Property(p => p.Name).HasMaxLength(256);
            Property(p => p.MediaType).HasColumnName("MediaTypeId");
            Property(p => p.MediaId).IsUnicode(false).IsRequired().HasMaxLength(16);
            Property(p => p.PublishDate).HasColumnType("datetime2").HasPrecision(0); // 2014-09-30 12:30:30

            ToTable("PlaylistItem", DbSchema.Media);
        }
    }
}
