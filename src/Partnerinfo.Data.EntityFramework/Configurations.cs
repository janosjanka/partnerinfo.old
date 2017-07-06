// Copyright (c) János Janka. All rights reserved.

using System.Data.Entity.ModelConfiguration;

namespace Partnerinfo
{
    /// <summary>
    /// The configuration class for the <see cref="MailAddressItem" /> entity type.
    /// </summary>
    internal class MailAddressItemConfiguration : ComplexTypeConfiguration<MailAddressItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MailAddressItemConfiguration" /> class.
        /// </summary>
        public MailAddressItemConfiguration()
        {
            Property(p => p.Address).HasMaxLength(128);
            Property(p => p.Name).HasMaxLength(128);
        }
    }

    /// <summary>
    /// The configuration class for the <see cref="PhoneGroupItemConfiguration" /> entity type.
    /// </summary>
    internal class PhoneGroupItemConfiguration : ComplexTypeConfiguration<PhoneGroupItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneGroupItemConfiguration" /> class.
        /// </summary>
        public PhoneGroupItemConfiguration()
        {
            Property(p => p.Personal).IsUnicode(false).HasMaxLength(16);
            Property(p => p.Business).IsUnicode(false).HasMaxLength(16);
            Property(p => p.Mobile).IsUnicode(false).HasMaxLength(16);
            Property(p => p.Other).IsUnicode(false).HasMaxLength(16);
        }
    }
}
