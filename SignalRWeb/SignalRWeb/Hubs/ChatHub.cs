using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRWeb.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            await Clients.All.SendAsync("Send", message);
        }
    }
}
