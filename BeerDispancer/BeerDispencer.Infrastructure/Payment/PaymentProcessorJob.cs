using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BeerDispencer.Infrastructure.Payment
{
    public class PaymentProcessorJob: BackgroundService
    {
       
        private readonly IServiceScopeFactory facrory;
        private readonly ILogger<PaymentProcessorJob> _logger;

        public PaymentProcessorJob(IServiceScopeFactory facrory , ILogger<PaymentProcessorJob> logger, IHostApplicationLifetime applicationLifetime)
		{
           
            this.facrory = facrory;
            _logger = logger;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {

                DoWork(stoppingToken);
                await Task.Delay(10_000);
            }
        }

        private async void DoWork(CancellationToken stoppingToken)
        {
          
            using var scope = facrory.CreateAsyncScope();
            using var dbcontext = scope.ServiceProvider.GetRequiredService<IBeerDispencerDbContext>();

            _logger.LogInformation("Start gatherinig outbox entities");


            var entitiesToProcess = dbcontext
                .Outbox
                .Where(
                x => x.MessageType.Equals(nameof(Payments)) &&
                x.Status != OperationStatus.Completed &&
                x.Status!=OperationStatus.InProgress)
                
                .ToArray();
            _logger.LogInformation($"Outbox entities to process {entitiesToProcess.Count()}");

            foreach (var e in entitiesToProcess)
            {
                _logger.LogInformation("Going to process entity: {@e}", e);
                e.Status = OperationStatus.InProgress;
                e.UpdatedAt = DateTime.UtcNow;
                var result =await  PerformFundsTransferAsync();

                if (result)
                {
                    e.Status = OperationStatus.Completed;
                    _logger.LogInformation("Successfully processed entity: {@e}", e);
                   
                }
                else
                {
                    e.Status = OperationStatus.Failed;
                    _logger.LogInformation("Failed to process entity: {@e}", e);
                    
                }
                e.UpdatedAt = DateTime.UtcNow;

                dbcontext.Outbox.Update(e);
                await dbcontext.SaveChangesAsync(stoppingToken);
            }
        }

        private  Task<bool> PerformFundsTransferAsync()
        {
            return Task.FromResult(true) ;
        }



    }
}

