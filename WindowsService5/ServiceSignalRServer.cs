using System;
using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace WindowsService5
{
    public partial class ServiceSignalRServer : ServiceBase
    {
        // URL для сервера
        string url = "http://localhost:5000";

        public static void LogEvent(string msg, EventLogEntryType eventLogType) {
            using (EventLog eventLog = new EventLog("Application")) {
                eventLog.Source = "ServiceSignalRServer";
                eventLog.WriteEntry(msg, eventLogType, 101, 1);
            }
        }

        public ServiceSignalRServer() {
            InitializeComponent();
        }

        private IDisposable serverHandle;

        protected override void OnStart(string[] args)
        {
            serverHandle = WebApp.Start<Startup>(url);
            LogEvent($"Server SignalR has started on {url}", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            serverHandle?.Dispose();
            LogEvent($"Server SignalR has stopped on {url}", EventLogEntryType.Information);
        }
    }

    //Хаб SignalR
    public class ChatHub : Microsoft.AspNet.SignalR.Hub
    {
        public void BroadcastMessage(string message) {
            string systemUser = "System";
            Clients.All.ReceiveMessage(systemUser, message);
        }

        public void SendMessage(string user, string message) {
            // Відправляємо повідомлення всім клієнтам
            Clients.All.ReceiveMessage(user, message);
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app) {
            // Включаємо CORS
            app.UseCors(CorsOptions.AllowAll);
            // Налаштовуємо SignalR
            app.MapSignalR();
            // Створюємо таймер для розсилки повідомлень кожні 10 секунд
            var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            var timer = new System.Timers.Timer(10000); // 10 секунд
            timer.Elapsed += (sender, e) =>
            {
                ServiceSignalRServer.LogEvent($"Server SignalR: sent msg", EventLogEntryType.Information);
                hubContext.Clients.All.ReceiveMessage("System", $"Broadcast message at {DateTime.Now}");
            };
            timer.Start();
        }
    }
}