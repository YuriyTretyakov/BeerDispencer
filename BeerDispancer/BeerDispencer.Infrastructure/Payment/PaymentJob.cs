using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using BeerDispencer.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BeerDispencer.Infrastructure.Payment
{
    public class PaymentJob: IHostedService
    {
       
        private readonly IServiceScopeFactory facrory;
        private readonly ILogger<PaymentJob> _logger;
        Timer _timer;
        CancellationToken _cancellationToken;

        public PaymentJob(IServiceScopeFactory facrory , ILogger<PaymentJob> logger)
		{
           
            this.facrory = facrory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
           

            _cancellationToken = cancellationToken;
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }


        private async void DoWork(object? state)
        {
            using var scope = facrory.CreateAsyncScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<IBeerDispencerDbContext>();

            _logger.LogInformation("Start gatherinig outbox entities");

            var entitiesToProcess = dbcontext.Outbox.Where(x => x.Status != OperationStatus.Completed || x.Status != OperationStatus.InProgress);
            _logger.LogInformation($"Outbox entities to process {entitiesToProcess.Count()}");

            foreach (var e in entitiesToProcess)
            {
                _logger.LogInformation("Going to process entity: {@e}", e);
                e.Status = OperationStatus.InProgress;
                e.UpdatedAt = DateTime.UtcNow;
                var result =await  PerformFundsTransferAsync();

                if (result)
                {
                    _logger.LogInformation("Successfully processed entity: {@e}", e);
                    e.Status = OperationStatus.Completed;
                }
                else
                {
                    _logger.LogInformation("Failed to process entity: {@e}", e);
                    e.Status = OperationStatus.Failed;
                }
                e.UpdatedAt = DateTime.UtcNow;
                await dbcontext.SaveChangesAsync(_cancellationToken);
            }
        }

        private  Task<bool> PerformFundsTransferAsync()
        {
            return Task.FromResult(true) ;
        }



    }
}

