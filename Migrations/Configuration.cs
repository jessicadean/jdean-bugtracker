namespace jdean_bugtracker.Migrations
{
    using jdean_bugtracker.Models;
    using jdean_bugtracker.Models.codeFirst;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<jdean_bugtracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(jdean_bugtracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            //Admin (me)
            if (!context.Users.Any(u => u.Email == "jessc.dean@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jessc.dean@gmail.com",
                    Email = "jessc.dean@gmail.com",
                    FirstName = "Jessica",
                    LastName = "Dean",
                }, "Applesauce3?");
            }
            var adminId = userManager.FindByEmail("jessc.dean@gmail.com").Id;
            userManager.AddToRole(adminId, "Admin");

            //Admin (demo)
            if (!context.Users.Any(u => u.Email == "admin@demo.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "admin@demo.com",
                    Email = "admin@demo.com",
                    FirstName = "Admin",
                    LastName = "Demo",
                }, "Demopass1!");
            }
            var adminDemoId = userManager.FindByEmail("admin@demo.com").Id;
            userManager.AddToRole(adminDemoId, "Admin");

            //Coder Foundry Admin 1
            if (!context.Users.Any(u => u.Email == "mjaang@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "mjaang@coderfoundry.com",
                    Email = "mjaang@coderfoundry.com",
                    FirstName = "Mark",
                    LastName = "Jaang",
                }, "Oranges5%");
            }
            var adminSub1Id = userManager.FindByEmail("mjaang@coderfoundry.com").Id;
            userManager.AddToRole(adminSub1Id, "Admin");

            //Coder Foundry Admin 2
            if (!context.Users.Any(u => u.Email == "rchapman@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "rchapman@coderfoundry.com",
                    Email = "rchapman@coderfoundry.com",
                    FirstName = "Ryan",
                    LastName = "Chapman",
                }, "Bananas4$");
            }
            var adminSub2Id = userManager.FindByEmail("rchapman@coderfoundry.com").Id;
            userManager.AddToRole(adminSub2Id, "Admin");

            //Project Manager
            if (!context.Users.Any(u => u.Email == "project@manager.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "project@manager.com",
                    Email = "project@manager.com",
                    FirstName = "Project",
                    LastName = "Manager",
                }, "Demopass1!");
            }
            var projectManagerId = userManager.FindByEmail("project@manager.com").Id;
            userManager.AddToRole(projectManagerId, "ProjectManager");

            //Developer
            if (!context.Users.Any(u => u.Email == "developer@demo.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "developer@demo.com",
                    Email = "developer@demo.com",
                    FirstName = "Developer",
                    LastName = "Developson",
                }, "Demopass1!");
            }
            var developerId = userManager.FindByEmail("developer@demo.com").Id;
            userManager.AddToRole(developerId, "Developer");

            //Submitter
            if (!context.Users.Any(u => u.Email == "submitter@demo.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "submitter@demo.com",
                    Email = "submitter@demo.com",
                    FirstName = "Submitter",
                    LastName = "McSubmits",
                }, "Demopass1!");
            }
            var submitterId = userManager.FindByEmail("submitter@demo.com").Id;
            userManager.AddToRole(submitterId, "Submitter");

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //Priorities
            if (!context.TicketPriorities.Any(p => p.Name == "Low"))
            {
                var priority = new TicketPriority();
                priority.Name = "Low";
                context.TicketPriorities.Add(priority);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "Medium"))
            {
                var priority = new TicketPriority();
                priority.Name = "Medium";
                context.TicketPriorities.Add(priority);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "High"))
            {
                var priority = new TicketPriority();
                priority.Name = "High";
                context.TicketPriorities.Add(priority);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "Urgent"))
            {
                var priority = new TicketPriority();
                priority.Name = "Urgent";
                context.TicketPriorities.Add(priority);
            }
            //Statuses
            if (!context.TicketStatus.Any(p => p.Name == "Unassigned"))
            {
                var status = new TicketStatus();
                status.Name = "Unassigned";
                context.TicketStatus.Add(status);
            }
            if (!context.TicketStatus.Any(p => p.Name == "Assigned"))
            {
                var status = new TicketStatus();
                status.Name = "Assigned";
                context.TicketStatus.Add(status);
            }
            if (!context.TicketStatus.Any(p => p.Name == "In Progress"))
            {
                var status = new TicketStatus();
                status.Name = "In Progress";
                context.TicketStatus.Add(status);
            }
            if (!context.TicketStatus.Any(p => p.Name == "Complete"))
            {
                var status = new TicketStatus();
                status.Name = "Complete";
                context.TicketStatus.Add(status);
            }
            //Types
            if (!context.TicketTypes.Any(p => p.Name == "Hardware"))
            {
                var type = new TicketType();
                type.Name = "Hardware";
                context.TicketTypes.Add(type);
            }
            if (!context.TicketTypes.Any(p => p.Name == "Software"))
            {
                var type = new TicketType();
                type.Name = "Software";
                context.TicketTypes.Add(type);
            }
        }
    }
}
