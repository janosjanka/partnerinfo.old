// Copyright (c) János Janka. All rights reserved.

using System;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Partnerinfo
{
    internal sealed class DateTime2Convention : Convention
    {
        public DateTime2Convention()
        {
            Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
        }
    }
}
