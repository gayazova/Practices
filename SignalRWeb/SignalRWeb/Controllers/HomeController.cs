using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRWeb.Hubs;
using System;
using System.Threading.Tasks;

namespace SignalRWeb.Controllers
{
    public class HomeController : Controller
    {
        public IHubContext<ChatHubVer2> hubContext;

        public HomeController(IHubContext<ChatHubVer2> hubContext)
        {
            this.hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(string product)
        {
            await hubContext.Clients.All.SendAsync("Notify", $"Добавлено: {product} - {DateTime.Now.ToShortTimeString()}");
            return RedirectToAction("Index");
        }
    }
}
