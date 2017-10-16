using jdean_bugtracker.Models;
using jdean_bugtracker.Models.codeFirst;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace jdean_bugtracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            List<Project> projects = new List<Project>();
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                projects = db.Projects.ToList();
            }
            else
            {
                projects = user.Projects.ToList();
            }
            return View(projects);
        }

        [Authorize(Roles = ("Admin, ProjectManager"))]
        public ActionResult AllProjects(string userId)
        {
            return View(db.Projects.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}