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

            // ������ ���������� ��������� SignalR:
            builder.Services.AddSignalR(act =>
            {
                act.ClientTimeoutInterval = new System.TimeSpan(0, 0, 30); // �����, � ������� �������� ������ ������ ��������� ������� ���������.  �� ��������� 30 ���
                act.HandshakeTimeout = new System.TimeSpan(0, 0, 15); //���������� ����� ��������, ������� ����� ������ �� ��������� �� ������� ������� ��������� �� ��������� ����������
                act.KeepAliveInterval = new System.TimeSpan(0, 0, 15); //���� � ������� ����� ������� ������ �� �������� ������� ���������, �� ������������� ������������ ping-��������� ��� ����������� ����������� ��������.
                //act.SupportedProtocols = ; - ���������� �������������� ���������. �� ��������� �������������� ��� ���������.
                act.EnableDetailedErrors = true; //��� �������� true ���������� ������� ��������� �������� ��������� ������
            })
                .AddHubOptions<ChatHub>(opt => // ������ ��������� ������ ��� ���� ChatHub
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
                    opt.ApplicationMaxBufferSize = 64; // ������������ ������ ������ � ������, � ������� ������ �������� ���������� �� ������� ������.
                    // opt.AuthorizationData; ������������ ������ (������ IList) �������� IAuthorizeData, ������� ����������, ����������� �� ������ ��� ����������� � ����.
                    opt.TransportMaxBufferSize = 64; // ������������ ������ ������ � ������, � ������� ������ �������� ������ ��� �������� �������.
                    opt.Transports = HttpTransportType.LongPolling | HttpTransportType.ServerSentEvents | HttpTransportType.WebSockets; //������������ ������� ����� �� �������� ������������ HttpTransportType, ������� ������������� ���������� ���� ����������.
                    opt.LongPolling.PollTimeout = new System.TimeSpan(0, 1, 0); //����������� ��������� LongPolling.
                    //opt.WebSockets.CloseTimeout ����������� ��������� WebSocket.
                });
                endpoints.MapHub<ChatHubVer2>("/chatVer2");
                endpoints.MapDefaultControllerRoute();
            });

            app.Run();
        }
    }
}
