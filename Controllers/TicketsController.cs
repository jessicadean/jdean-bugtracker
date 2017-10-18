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
using System.Web.Security;
using CustomHelpers;
using System.IO;

namespace jdean_bugtracker.Controllers
{
    [Authorize]
    public class TicketsController : Universal
    {        
        // GET: Tickets
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var tickets = db.Tickets.Include(t => t.AssignToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
               
                return View(user.Projects.SelectMany(t => t.Tickets).ToList());
            }
            else if (User.IsInRole("Developer"))
            {
                return View(tickets.Where(t => t.AssignToUserId == user.Id).ToList());
            }

            else if (User.IsInRole("Submitter"))
            {
                return View(tickets.Where(t => t.OwnerUserId == user.Id).ToList());
            }
            return RedirectToAction("Index", "Projects");

        }

        // GET: TicketsAdmin
        [Authorize (Roles=("Admin"))]
        public ActionResult TicketsAdmin()
        {
            var tickets = db.Tickets.Include(t => t.AssignToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }


        // GET: Tickets/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ProjectAssignHelper helper = new ProjectAssignHelper();
            Project project = db.Projects.Find(ticket.ProjectId);
            var user = db.Users.Find(User.Identity.GetUserId());

            if (User.IsInRole("Admin"))
            {
                return View(ticket);
            }
            else if (User.IsInRole("ProjectManager"))
            {
               if (helper.IsUserOnProject(user.Id, project.Id) == true) //defaults to true, == true not necessary
                    {
                
                return View(ticket);
                }
            }
            else if (User.IsInRole("Developer") && ticket.AssignToUserId == user.Id)
            {
                return View(ticket);
            }
            else if (User.IsInRole("Submitter") && ticket.OwnerUserId == user.Id)
            {
                return View(ticket);
            }
            else
            {
                return RedirectToAction("Index", "Projects");
            }
            return RedirectToAction("Index","Tickets", null);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {

            ProjectAssignHelper helper = new ProjectAssignHelper();
            var user = db.Users.Find(User.Identity.GetUserId());


            ViewBag.ProjectId = new SelectList(helper.ListUserProjects(user.Id), "Id", "Title");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
           
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
           
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Submitter")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.TicketStatusId = 1;
                ticket.OwnerUserId = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
                ticket.Created = DateTimeOffset.UtcNow;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            Ticket ticket = db.Tickets.Find(id);
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            }
            else
            {
                ViewBag.TicketStatusId = new SelectList(new[] { ticket.TicketStatus }, "Id", "Name", ticket.TicketStatusId);
            }
            
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectAssignHelper helper = new ProjectAssignHelper();
            UserRoleHelper userhelper = new UserRoleHelper();
            if (ticket == null)
            {
                return HttpNotFound();
            }
            List<ApplicationUser> devlist = new List<ApplicationUser>();
            var projectdev = helper.ListUsersOnProject(ticket.ProjectId);
            foreach (var dev in projectdev)
            {
                if (userhelper.IsUserInRole(dev.Id, "Developer") == true)
                {
                    devlist.Add(dev);
                }
                
            }
            ViewBag.AssignToUserId = new SelectList(devlist, "Id", "FirstName", ticket.AssignToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(helper.ListUserProjects(user.Id), "Id", "Title");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            if (User.IsInRole("Admin"))
            {
                return View(ticket);
            }
            else if (User.IsInRole("ProjectManager"))
            {
                if (helper.IsUserOnProject(user.Id, ticket.ProjectId) == true) //defaults to true, == true not necessary
                {

                    return View(ticket);
                }
            }
            else if (User.IsInRole("Developer") && ticket.AssignToUserId == user.Id)
            {
                return View(ticket);
            }
            else if (User.IsInRole("Submitter") && ticket.OwnerUserId == user.Id)
            {
                return View(ticket);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Tickets", null);
        }
           
        

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                TicketHistoryHelper tickethelper = new TicketHistoryHelper();
                ticket.Updated = DateTimeOffset.UtcNow;
                db.Entry(ticket).State = EntityState.Modified;


                TicketHistory tickethistory = new TicketHistory();
                NotificationHelper notifyHelper = new NotificationHelper();
                var oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
                if (oldTicket.AssignToUserId != ticket.AssignToUserId)
                {
                    tickethelper.TktAssignUserHistory(ticket, user.Id);
                    notifyHelper.Notify(ticket.AssignToUserId, "BugTracker App Notification", "Your ticket needs attention. "
                            + ticket.Title, true);
                }
                if (oldTicket.Title != ticket.Title)
                {
                    tickethelper.TktTitleHistory(ticket, user.Id);
                    notifyHelper.Notify(ticket.AssignToUserId, "BugTracker App Notification", "Your ticket needs attention. "
                            + ticket.Title, true);
                }
                if (oldTicket.Description != ticket.Description)
                {
                    tickethelper.TktDescriptionHistory(ticket, user.Id);
                    notifyHelper.Notify(ticket.AssignToUserId, "BugTracker App Notification", "Your ticket needs attention. "
                            + ticket.Title, true);
                }
                if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    tickethelper.TktTypeIdHistory(ticket, user.Id);
                    notifyHelper.Notify(ticket.AssignToUserId, "BugTracker App Notification", "Your ticket needs attention. "
                            + ticket.Title, true);
                }
                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    tickethelper.TktStatusIdHistory(ticket, user.Id);
                    notifyHelper.Notify(ticket.AssignToUserId, "BugTracker App Notification", "Your ticket needs attention. "
                            + ticket.Title, true);
                }
                if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    tickethelper.TktPriorityIdHistory(ticket, user.Id);
                    notifyHelper.Notify(ticket.AssignToUserId, "BugTracker App Notification", "Your ticket needs attention. "
                            + ticket.Title, true);
                }

                db.SaveChanges();

                ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignToUserId);
                ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

                return View(ticket);
            }
            return RedirectToAction("Index");
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Tickets/CreateAttachment
        //public ActionResult CreateAttachment(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Ticket ticket = db.Tickets.Find(id);
        //    if (ticket == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticket);
        //}

        //POST: Tickets/CreateAttachment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttachment(
            [Bind(Include = "Id,Description,TicketId")] TicketAttachment ticketattachment, HttpPostedFileBase attachFile) //Bind Attribute tells it to add these properties when it sends to view
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (attachFile != null)
            {
            
                var ext = Path.GetExtension(attachFile.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp" && ext != ".pdf")

                    ModelState.AddModelError("attachFile", "Invalid Format.");
            }
            if (ModelState.IsValid) //makes sure all the properties are bound
            {
            if (attachFile != null)

                {
                    ticketattachment.Created = DateTimeOffset.Now;
                    ticketattachment.AuthorId = user.Id;
                    var filePath = "/Assets/UserUploads/";
                var absPath = Server.MapPath("~" + filePath);
                ticketattachment.FileUrl = filePath + attachFile.FileName;
                attachFile.SaveAs(Path.Combine(absPath, attachFile.FileName));
               
                db.TicketAttachments.Add(ticketattachment);
                
                }
                db.SaveChanges();
            }

            var ticket = db.Tickets.Find(ticketattachment.TicketId);
            if (ticket.AssignToUserId != null)
            {
                return RedirectToAction("AttachNotify", "Tickets", new { id = ticketattachment.Id });
            }
            else
            {
                return RedirectToAction("Details", "Tickets", new { id = ticketattachment.TicketId });
            }
            
            
           
        }

        public ActionResult AttachNotify(int? id)
        {
            var ticketAttachment = db.TicketAttachments.Find(id);
            NotificationHelper notificationHelper = new NotificationHelper();
            notificationHelper.Notify(ticketAttachment.TicketId, ticketAttachment.Ticket.AssignToUser.Id, "Ticket Update Alert", "An attachment has been added to " + ticketAttachment.Ticket.Title, true);
            return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
        }

        // POST: Tickets/DeleteAttachment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAttachment(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            TicketAttachment ticketattachment = db.TicketAttachments.Find(id);
            Ticket ticket = db.Tickets.Find(ticketattachment.TicketId);

            if ((User.IsInRole("Admin") || (User.IsInRole("ProjectManager") && ticket.OwnerUserId == user.Id) || ticketattachment.AuthorId == user.Id))
            {
                db.TicketAttachments.Remove(ticketattachment);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        //POST: Tickets/CreateComment
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment([Bind(Include = "Id,TicketId,Body")]TicketComment ticketComment)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
           
            if (ModelState.IsValid)
            {

                ticketComment.AuthorId = user.Id;
                ticketComment.Created = DateTimeOffset.UtcNow;
                db.TicketComments.Add(ticketComment);

                db.SaveChanges();
            }

            var ticket = db.Tickets.Find(ticketComment.TicketId);
            if (ticket.AssignToUserId != null)
            {
                return RedirectToAction("CommentNotify", "Tickets", new { id = ticketComment.Id });
            }
            else
            {
                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            }
        }

        public ActionResult CommentNotify(int? id)
        {
            var ticketComment = db.TicketComments.Find(id);
            NotificationHelper notificationHelper = new NotificationHelper();
            notificationHelper.Notify(ticketComment.TicketId, ticketComment.Ticket.AssignToUser.Id, "Ticket Update Alert", "A comment has been added to " + ticketComment.Ticket.Title, true);
            return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
        }

        // GET: Tickets/EditComment/5
        public ActionResult EditComment(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Ticket ticket = db.Tickets.Find(id);
            TicketComment ticketcomment = db.TicketComments.Find(id);


            if ((User.IsInRole("Admin") || (User.IsInRole("ProjectManager") && ticket.OwnerUserId == user.Id) || ticketcomment.AuthorId == user.Id))

            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (ticketcomment == null)
                {
                    return HttpNotFound();
                }
                

            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticketcomment.AuthorId);
            ViewBag.BlogPostId = new SelectList(db.TicketComments, "Id", "Title", ticketcomment.TicketId);
            return View(ticketcomment);
          
        }



        // POST: Tickets/EditComment/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment([Bind(Include = "Id,TicketId,AuthorId,Body,Created,Updated")] TicketComment ticketcomment)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Ticket ticket = db.Tickets.Find(ticketcomment.TicketId);
            //TicketComment ticketcomment = db.TicketComments.Find(id);
            if ((User.IsInRole("Admin") || (User.IsInRole("ProjectManager") || ticket.OwnerUserId == user.Id) || ticketcomment.AuthorId == user.Id))
            {
                if (ModelState.IsValid)
                {
                    //ticketcomment.Id = ticketcomment.TicketId;
                    //ticketcomment.AuthorId = User.Identity.GetUserId();
                    ticketcomment.Updated = DateTimeOffset.UtcNow;
                    db.Entry(ticketcomment).State = EntityState.Modified;
                    db.SaveChanges();
                    
                    return RedirectToAction("Details", "Tickets", new { id = ticketcomment.TicketId });

                }
                //ViewBag.AuthorId = new SelectList("Id", "FirstName", ticketcomment.AuthorId);
                //ViewBag.CommentId = new SelectList(db.TicketComments, "Id", "Title", ticketcomment.Id);
return View("Details");
                //return RedirectToAction("Details", "Tickets", new { id = ticketcomment.TicketId });
                
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


        }

        //// GET: Tickets/DeleteComment/5
        //public ActionResult DeleteComment(int? id)
        //{
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    Ticket ticket = db.Tickets.Find(id);
        //    TicketComment ticketcomment = db.TicketComments.Find(id);
        //    if ((User.IsInRole("Admin") || (User.IsInRole("ProjectManager") && ticket.OwnerUserId == user.Id) || ticketcomment.AuthorId == user.Id))
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }

        //        if (ticketcomment == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(ticketcomment);
        //    }
        //    else
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //}

        // POST: Tickets/DeleteComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            TicketComment ticketcomment = db.TicketComments.Find(id);
            Ticket ticket = db.Tickets.Find(ticketcomment.TicketId);

            if ((User.IsInRole("Admin") || (User.IsInRole("ProjectManager") && ticket.OwnerUserId == user.Id) || ticketcomment.AuthorId == user.Id))
            {
                db.TicketComments.Remove(ticketcomment);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

       

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
