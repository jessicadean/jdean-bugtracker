using jdean_bugtracker.Models.codeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jdean_bugtracker.Models.Helpers
{
    public class TicketViewModel
    {
        public ApplicationUser User { get; set; }
        public Ticket Ticket { get; set; }
        public Project Project { get; set; }
        public SelectList Projects { get; set; }
        public int TicketPriority { get; set; }
        public SelectList TicketPriorities { get; set; }
        public int TicketType { get; set; }
        public SelectList TicketTypes { get; set; }
    }
}