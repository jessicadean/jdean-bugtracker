using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jdean_bugtracker.Models;
using jdean_bugtracker.Models.codeFirst;
using jdean_bugtracker.Models.Helpers;
using Microsoft.AspNet.Identity;

namespace jdean_bugtracker.Controllers
{
    [Authorize]
    public class ProjectsController : Universal
    {
        private ProjectAssignHelper helper = new ProjectAssignHelper();

        // GET: Projects
        [Authorize]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                ViewBag.UserTimeZone = db.Users.Find(User.Identity.GetUserId()).TimeZone;
                var userId = User.Identity.GetUserId();
                var userProjects = helper.ListUserProjects(userId);
                return View(userProjects);
            }
            else
            {
                return View();               
            }

        }
        // GET: ProjectsAdmin
        [Authorize]
        public ActionResult ProjectAdmin()
        {
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                return View(db.Projects.ToList());
            }
            else
            {

                return RedirectToAction("Index");

            }

        }
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ProjectAssignHelper helper = new ProjectAssignHelper();
            var user = db.Users.Find(User.Identity.GetUserId());
            
            if (helper.IsUserOnProject(user.Id, project.Id) == true || User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            { 
                return View(project);
            }
            else
            {
                return RedirectToAction("Index");
        }
    }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Description,AuthorId")] Project project)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                db.Projects.Add(project);
                project.Created = DateTime.Now;
                project.AuthorId = user.FullName;
                db.SaveChanges();
                return RedirectToAction("ProjectAdmin");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Description,AuthorId")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                project.Updated = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("ProjectAdmin");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin, ProjectManager")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("ProjectAdmin");
        }

        //GET: EditProjectUsers
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult ProjectUser(int id)
        {

            var project = db.Projects.Find(id);
            ProjectUserViewModel projectuserVM = new ProjectUserViewModel();
            projectuserVM.AssignProject = project;
            projectuserVM.AssignProjectId = id;
            projectuserVM.SelectedUsers = project.Users.Select(u => u.Id).ToArray();
            projectuserVM.Users = new MultiSelectList(db.Users.ToList(), "Id", "FullName", projectuserVM.SelectedUsers); //can call FullName if prefer

            return View(projectuserVM);

        }

        //POST: EditProjectUsers
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult ProjectUser(ProjectUserViewModel model)
        {

            ProjectAssignHelper helper = new ProjectAssignHelper();
            foreach (var userId in db.Users.Select(r => r.Id).ToList())
            {
                helper.RemoveUserFromProject(userId, model.AssignProjectId);

            }

            foreach (var userId in model.SelectedUsers)
            {
                helper.AddUserToProject(userId, model.AssignProjectId);

            }
            return RedirectToAction("ProjectAdmin");

        }
        //{
        //    var project = db.Projects.Find(model.Project.Id);
        //    var helper = new ProjectAssignHelper();

        //    if(model.SelectedUsers != null)
        //    { 

        //    foreach (var user in model.SelectedUsers)
        //    {
        //        helper.RemoveUserFromProject(id, projectId);

        //    }

        //        return View(model);
        //    }
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
