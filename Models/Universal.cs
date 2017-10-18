using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jdean_bugtracker.Models
{
    public class Universal : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (User.Identity.IsAuthenticated)
            {

                var userId = User.Identity.GetUserId();
                var user = db.Users.AsNoTracking().First(u => u.Id == userId);
                ViewBag.FirstName = user.FirstName;
                ViewBag.LastName = user.LastName;
                ViewBag.FullName = user.FullName;
                ViewBag.ProfilePic = user.ProfilePic;

                base.OnActionExecuting(filterContext);

            }
        }
    }
}