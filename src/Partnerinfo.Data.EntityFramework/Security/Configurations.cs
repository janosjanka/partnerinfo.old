// Copyright (c) János Janka. All rights reserved.

using System.Data.Entity.ModelConfiguration;

namespace Partnerinfo.Security.EntityFramework
{
    /// <summary>
    /// The configuration class for the <see cref="SecurityAccessRule" /> entity type.
    /// </summary>
    internal class SecurityAccessRuleConfig : EntityTypeConfiguration<SecurityAccessRule>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityAccessRuleConfig" /> class.
        /// </summary>
        public SecurityAccessRuleConfig()
        {
            Property(p => p.ObjectType).HasColumnName("ObjectTypeId");
            Property(p => p.Permission).HasColumnName("PermissionId");
            Property(p => p.Visibility).HasColumnName("VisibilityId");

            ToTable("AccessRule", DbSchema.Security);

            MapToStoredProcedures(m =>
            {
                m.Insert(p => p.HasName("InsertAccessRule", DbSchema.Security));
                m.Update(p => p.HasName("UpdateAccessRule", DbSchema.Security));
                m.Delete(p => p.HasName("DeleteAccessRule", DbSchema.Security));
            });
        }
    }
}
