using Microsoft.Azure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rebus.Activation;
using Rebus.Config;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GloboTickets.Services.Payment
{
    public class NewOrderWorkerService : BackgroundService
    {
        private readonly IConfiguration _config;
        public NewOrderWorkerService(IConfiguration configuration)
        {
            _config = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Payment service has started.");

            var storageAccount = CloudStorageAccount.Parse(_config["AzureQueues:ConnectionString"]);

            using var activator = new BuiltinHandlerActivator();
            activator.Register(() => new NewOrderHandler());
            
            Configure.With(activator)
                .Transport(t => t.UseAzureStorageQueues(storageAccount, _config["AzureQueues:QueueName"]))
                .Start();

            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
    }
}
