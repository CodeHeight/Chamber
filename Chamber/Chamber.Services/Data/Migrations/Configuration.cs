using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Chamber.Domain.DomainModel;
using Chamber.Services.Data.Context;
using Chamber.Utilities;

namespace Chamber.Services.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ChamberContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ChamberContext context)
        {
            // if the settings already exist then do nothing
            // If not then add default settings
            var currentSettings = context.Setting.FirstOrDefault();
            if (currentSettings == null)
            {
                // create the settings
                var settings = new Settings
                {
                    SiteName = "Chamber of Commerce",
                    SiteUrl = "chamber.azurewebsites.net",
                    Theme = "Default"
                };

                context.Setting.Add(settings);
                context.SaveChanges();
            }
        }
    }
}