using jdean_bugtracker.Models.codeFirst;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace jdean_bugtracker.Models.Helpers
{
    public class ProjectAssignHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));


        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var userBool = project.Users.Any(u => u.Id == userId);
            return userBool;
        }
        public void RemoveUserFromProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            project.Users.Remove(user);
            db.SaveChanges();
        }
        public void AddUserToProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            project.Users.Add(user);
            db.SaveChanges();
        }

        public List<Project> ListUserProjects(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Projects.ToList();
        }

        public List<ApplicationUser> ListUsersOnProject(int projectId)
        {
            var project = db.Projects.Find(projectId);
            return project.Users.ToList();
        }

        public List<ApplicationUser> ListUsersNotOnProjects(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }
        //public ICollection<string> ListUserProjects(string userId)
        //{
        //    var user = db.Users.Find(userId);
        //    var project = db.Projects.Id;


        //    user.Projects.ToList();
        //    //return userManager.GetRoles(userId);
        //}

    }
}