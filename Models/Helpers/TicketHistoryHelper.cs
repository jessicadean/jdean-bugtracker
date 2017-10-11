using jdean_bugtracker.Models.codeFirst;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jdean_bugtracker.Models.Helpers
{
    public class TicketHistoryHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void TktAssignUserHistory(Ticket ticket, string userId)
        {
            
            TicketHistory tickethistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            tickethistory.TicketId = ticket.Id;
            tickethistory.OldValue = oldTicket.AssignToUser.FullName;
            tickethistory.NewValue = db.Users.Find(ticket.AssignToUserId).FullName;
            tickethistory.Property = "Assign to User";
            tickethistory.Created = DateTimeOffset.UtcNow;
            tickethistory.AuthorId = userId;
            db.TicketHistories.Add(tickethistory);
              db.SaveChanges();
        }

        public void TktTitleHistory(Ticket ticket, string userId)
        {

            TicketHistory tickethistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            tickethistory.TicketId = ticket.Id;
            tickethistory.OldValue = oldTicket.Title;
            tickethistory.NewValue = ticket.Title;
            tickethistory.Property = "Title";
            tickethistory.Created = DateTimeOffset.UtcNow;
            tickethistory.AuthorId = userId;
            db.TicketHistories.Add(tickethistory);
            db.SaveChanges();
        }

        public void TktDescriptionHistory(Ticket ticket, string userId)
        {

            TicketHistory tickethistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            tickethistory.TicketId = ticket.Id;
            tickethistory.OldValue = oldTicket.Description;
            tickethistory.NewValue = ticket.Description;
            tickethistory.Property = "Description";
            tickethistory.Created = DateTimeOffset.UtcNow;
            tickethistory.AuthorId = userId;
            db.TicketHistories.Add(tickethistory);
            db.SaveChanges();
        }

        public void TktTypeIdHistory(Ticket ticket, string userId)
        {

            TicketHistory tickethistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            tickethistory.TicketId = ticket.Id;
            tickethistory.OldValue = oldTicket.TicketType.Name;
            tickethistory.NewValue = db.TicketTypes.Find(ticket.TicketTypeId).Name;
            tickethistory.Property = "Ticket Type";
            tickethistory.Created = DateTimeOffset.UtcNow;
            tickethistory.AuthorId = userId;
            db.TicketHistories.Add(tickethistory);
            db.SaveChanges();
        }

        public void TktStatusIdHistory(Ticket ticket, string userId)
        {

            TicketHistory tickethistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            tickethistory.TicketId = ticket.Id;
            tickethistory.OldValue = oldTicket.TicketStatus.Name;
            tickethistory.NewValue = db.TicketStatus.Find(ticket.TicketStatusId).Name;
            tickethistory.Property = "Ticket Status";
            tickethistory.Created = DateTimeOffset.UtcNow;
            tickethistory.AuthorId = userId;
            db.TicketHistories.Add(tickethistory);
            db.SaveChanges();
        }

        public void TktPriorityIdHistory(Ticket ticket, string userId)
        {

            TicketHistory tickethistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            tickethistory.TicketId = ticket.Id;
            tickethistory.OldValue = oldTicket.TicketPriorityId.ToString();
            tickethistory.NewValue = db.TicketPriorities.Find(ticket.TicketPriorityId).Name;
            tickethistory.Property = "Ticket Status";
            tickethistory.Created = DateTimeOffset.UtcNow;
            tickethistory.AuthorId = userId;
            db.TicketHistories.Add(tickethistory);
            db.SaveChanges();
        }
    }
}