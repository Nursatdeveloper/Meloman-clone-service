using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Meloman_clone_service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            client.Dispose();
            _logger.LogInformation("The service has been stopped...");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await client.GetAsync("http://meloman-clone-db.herokuapp.com");
                if(result.IsSuccessStatusCode)
                {
                    _logger.LogInformation("The website is up. Status code: {StatusCode}, {time}", result.StatusCode, DateTimeOffset.Now);
                }
                else
                {
                    _logger.LogError("The website is down. Status code: {StatusCode}, {time}", result.StatusCode, DateTimeOffset.Now);

                    // Live toast to notify that meloman-clone is down
                    new ToastContentBuilder()
                        .AddArgument("action", "viewConversation")
                        .AddArgument("conversationId", 9813)
                        .AddText("Meloman-clone Service")
                        .AddText($"The website is down! Status Code: {result.StatusCode}")
                        .Show();
                }
                
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
