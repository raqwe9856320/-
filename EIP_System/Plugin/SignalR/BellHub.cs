using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace EIP_System.Plugin.SignalR
{
    public class BellHub : Hub
    {
        public void Bell()
        {
            Clients.All.bell();
        }
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}