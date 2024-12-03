using System;
using System.Diagnostics;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace WindowsService4
{
    public partial class ServiceGetCryptoInf : ServiceBase
    {
        public static void LogEvent(string msg, EventLogEntryType eventLogType)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "ServiceGetCryptoInf";
                eventLog.WriteEntry(msg, eventLogType, 101, 1);
            }
        }

        public ServiceGetCryptoInf()
        {
            InitializeComponent();
        }

        IHost hostBuilder;

        protected override void OnStart(string[] args)
        {
            // Налаштування хостингу
            var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Реєстрація залежностей
                services.AddHostedService<MyBackgroundService>();
                services.AddHttpClient<IMyService, MyService>();
                services.AddTransient<IMyService, MyService>();
                services.AddMemoryCache();
            })
            .Build();

            // Запуск хосту
            hostBuilder.Start();
        }

        protected override async void OnStop()
        {
            await hostBuilder.StopAsync();
        }
    }

    // Служба, що працює у фоновому режимі
    public class MyBackgroundService : BackgroundService
    {
        private readonly IMyService _myService;
        private readonly ILogger<MyBackgroundService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public MyBackgroundService(IMyService myService, ILogger<MyBackgroundService> logger, HttpClient httpClient, IMemoryCache memoryCache)
        {
            _myService = myService;
            _logger = logger;
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Служба запущена");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Виконання фонового завдання");
                _myService.PerformTask();
                await Task.Delay(10000, stoppingToken); // очікування 1 с
            }

            _logger.LogInformation("Служба зупинена");
        }
    }

    // Інтерфейс для сервісу
    public interface IMyService
    {
        void PerformTask();
    }

    // Реалізація сервісу
    public class MyService : IMyService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public MyService(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }
        public async Task<ExchangeInfo> GetExchangeInfo()
        {
            // Використання кешування
            return (await _memoryCache.GetOrCreateAsync(
            $"{GetType().Name}.GetExchangeInfo()",

            async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(600));
                return await GetData();
            }));

            async Task<ExchangeInfo> GetData()
            {
                Console.WriteLine("GetExchangeInfo via http request");

                var response = await _httpClient.GetAsync("https://api.binance.com/api/v3/exchangeInfo?symbol=ETHBTC");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ExchangeInfo>(content);

                return result;
            }
        }

        public async void PerformTask()
        {
            var exchangeInfo = await GetExchangeInfo();
            ServiceGetCryptoInf.LogEvent(exchangeInfo.GetExchangeInfoSummary(), EventLogEntryType.Information);
        }
    }
}