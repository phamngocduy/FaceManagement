using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace FaceManagement.Controllers
{
    public class CheckHub : Hub
    {
        public void Send(string id, string code)
        {
            Clients.All.addNewCheckToPage(id, code);
        }
    }
}