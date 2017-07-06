// Copyright (c) János Janka. All rights reserved.

using System.Data.Entity.ModelConfiguration;

namespace Partnerinfo.Input.EntityFramework
{
    /// <summary>
    /// Configures the <see cref="CommandEntityConfiguration" /> type.
    /// </summary>
    internal class CommandEntityConfiguration : EntityTypeConfiguration<CommandItem>
    {
        public CommandEntityConfiguration()
        {
            Property(p => p.Uri).IsUnicode(false).IsRequired().HasMaxLength(64);
            ToTable("Command", DbSchema.Input);
        }
    }
}
