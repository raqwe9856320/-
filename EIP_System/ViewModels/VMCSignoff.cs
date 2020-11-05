using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendSystem.ViewModels
{
    public class VMCSignoff
    {
        public int id { get; set; }
        public int? leaveId { get; set; }
        public int? applypunchId { get; set; }
        public int? overtimeId { get; set; }
        public string supervisorName { get; set; }
        public string agentName { get; set; }
        public int? isAgree { get; set; }
        public string applyClass { get; set; }
        public DateTime startdate { get; set; }
        public DateTime? passdate { get; set; }
        public DateTime enddate { get; set; }
    }
}