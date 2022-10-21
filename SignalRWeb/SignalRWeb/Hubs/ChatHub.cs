using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SignalRWeb.Hubs
{
    public class ChatHub : Hub
    {

        public async Task Send(User user)
        {
            user.Age += 5;
            await Clients.Caller.SendAsync("Receive", user);
        }

        public class User
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }

        public async Task SendOld(string product)
        {
            var connetionId = Context.ConnectionId; // уникальный идентификатор подключения в виде строки
            var connectionAborted = Context.ConnectionAborted; // возвращает объект CancellationToken, который извещает о закрытии подключения
            var user = Context.User; //возвращает объект ClaimsPrincipal, ассоциированный с текущим пользователем. По сути это аналог свойства HttpContext.User, которое доступно в констроллерах
            var userIdentifier = Context.UserIdentifier; // возвращает идентификатор пользователя.
            var items = Context.Items; // возвращает словарь значений, ассоциированных с текущим подключением. Данный словарь позволяет хранить данные для определенного клиента между его запросами
            var features = Context.Features; //возвращает коллекцию возможностей HTTP, ассоциированных с текущим подключением

            //Context.Abort() - принудительно завершает текущее подключение
            //Context.GetHttpContext() - возвращает объект HttpContext для текущего подключения или null, если подключение не ассоциировано с запросом HTTP.

            //Client
            await Clients.Caller.SendAsync("Notify", "Ваш товар добавлен");
            await Clients.Others.SendAsync("Receive", $"Добавлено: {product} в {DateTime.Now.ToShortTimeString()}");

            //То же самое, что и выше
            await Clients.Client(Context.ConnectionId).SendAsync("Notify", "Ваш товар добавлен");
            await Clients.AllExcept(new List<string> { Context.ConnectionId }).SendAsync("Receive", $"Добавлено: {product} в {DateTime.Now.ToShortTimeString()}");

            //await Clients.All.SendAsync("Receive", message, Context.ConnectionId);
        }

        // срабатывает при подключении нового клиента
        public override async Task OnConnectedAsync()
        {
            var context = this.Context.GetHttpContext();
            // получаем кук name
            if (context.Request.Cookies.ContainsKey("name"))
            {
                string userName;
                if (context.Request.Cookies.TryGetValue("name", out userName))
                {
                    Debug.WriteLine($"name = {userName}");
                }
            }
            // получаем юзер-агент
            Debug.WriteLine($"UserAgent = {context.Request.Headers["User-Agent"]}");
            // получаем ip
            Debug.WriteLine($"RemoteIpAddress = {context.Connection.RemoteIpAddress.ToString()}");

            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} вошел в чат");
            await base.OnConnectedAsync();
        }

        // срабатывает при отключении клиента, в качестве параметра передается сообщение об ошибке, которая описывает, почему произошло отключение.
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} покинул в чат");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
