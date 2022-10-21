using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using SignalRWeb.Hubs;

namespace SignalRWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ѕример глобальной настройки SignalR:
            builder.Services.AddSignalR(act =>
            {
                act.ClientTimeoutInterval = new System.TimeSpan(0, 0, 30); // врем€, в течение которого клиент должен отправить серверу сообщение.  по умолчанию 30 сек
                act.HandshakeTimeout = new System.TimeSpan(0, 0, 15); //допустимое врем€ таймаута, которое может пройти до получени€ от клиента первого сообщени€ об установки соединени€
                act.KeepAliveInterval = new System.TimeSpan(0, 0, 15); //если в течение этого периода сервер не отправит никаких сообшений, то автоматически отправл€етс€ ping-сообщение дл€ поддержани€ подключени€ открытым.
                //act.SupportedProtocols = ; - определ€ет поддерживаемые протоколы. ѕо умолчанию поддерживаютс€ все протоколы.
                act.EnableDetailedErrors = true; //при значении true возвращает клиенту детальное описание возникшей ошибки
            })
                .AddHubOptions<ChatHub>(opt => // пример настройки только дл€ хаба ChatHub
                {
                    opt.EnableDetailedErrors = true;
                });
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            app.MapGet("/", () => "Hello World!");
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat", opt =>
                {
                    opt.ApplicationMaxBufferSize = 64; // максимальный размер буфера в байтах, в который сервер помещает получаемые от клиента данные.
                    // opt.AuthorizationData; представл€ет список (объект IList) объектов IAuthorizeData, которые определ€ют, авторизован ли клиент дл€ подключени€ к хабу.
                    opt.TransportMaxBufferSize = 64; // максимальный размер буфера в байтах, в который сервер помещает данные дл€ отправки клиенту.
                    opt.Transports = HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents | HttpTransportType.WebSockets; //представл€ет битовую маску из значений перечислени€ HttpTransportType, котора€ устанавливает допустимые типы транспорта.
                    opt.LongPolling.PollTimeout = new System.TimeSpan(0, 1, 0); //настраивает транспорт LongPolling.
                    //opt.WebSockets.CloseTimeout настраивает транспорт WebSocket.
                });
                endpoints.MapHub<ChatHubVer2>("/chatVer2");
                endpoints.MapDefaultControllerRoute();
            });

            app.Run();
        }
    }
}
