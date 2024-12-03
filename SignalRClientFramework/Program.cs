using System;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRClientFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            // URL сервера SignalR
            string serverUrl = "http://localhost:5000";

            // Створюємо підключення до сервера
            var connection = new HubConnection(serverUrl);

            // Створюємо проксі для хаба "ChatHub"
            var chatHub = connection.CreateHubProxy("ChatHub");

            // Налаштовуємо обробник для отримання повідомлень
            chatHub.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"{user}: {message}");
            });

            try
            {
                // Встановлюємо підключення
                connection.Start().Wait();
                Console.WriteLine("Connected to the SignalR server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
                return;
            }

            // Тримаємо програму запущеною, щоб отримувати повідомлення
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            // Закриваємо підключення
            connection.Stop();
            Console.WriteLine("Disconnected from the server.");
        }
    }
}
