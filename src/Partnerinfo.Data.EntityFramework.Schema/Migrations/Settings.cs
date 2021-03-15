// Copyright (c) János Janka. All rights reserved.

using System.Data.Entity.Migrations;

namespace Partnerinfo.Migrations
{
    public class Settings : DbMigrationsConfiguration<PartnerDbContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            AutomaticMigrationsEnabled = true;
        }
    }
}
