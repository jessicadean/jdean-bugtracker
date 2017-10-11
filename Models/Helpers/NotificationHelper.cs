using jdean_bugtracker.Models.codeFirst;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace jdean_bugtracker.Models.Helpers
{
    //public class NotificationHelper
    //{

    //    public async Task<ActionResult> NotifyUser(Ticket ticket, string userId)
    //    {

    //        var user = db.Tickets.Find(ticket.AssignToUserId).Email;
          
            
    //        var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
    //        await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
    //        return RedirectToAction("ForgotPasswordConfirmation", "Account");
    //    }

    //}
    //return View();
}    