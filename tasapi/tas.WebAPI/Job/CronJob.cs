
using Microsoft.Extensions.DependencyInjection;
using tas.Application.Repositories;

namespace tas.WebAPI.Job
{
    public class CronJob : IHostedService
    {
        private Timer _timer;
        private DateTime _nextExecutionTime;
        private const int SCHEDULE_HOUR = 0;
        private const int SCHEDULE_MINUTE = 10;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CronJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Cron Job is starting.");
            CalculateNextExecutionTime();
            TimeSpan timeUntilNextExecution = _nextExecutionTime - DateTime.Now;
            _timer = new Timer(DoWork, null, timeUntilNextExecution, Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        private void CalculateNextExecutionTime()
        {
            DateTime now = DateTime.Now;
            DateTime fixedExecutionTime = new DateTime(now.Year, now.Month, now.Day, SCHEDULE_HOUR, SCHEDULE_MINUTE, 0);
            if (now > fixedExecutionTime)
            {
                _nextExecutionTime = fixedExecutionTime.AddDays(1);
            }
            else
            {
                _nextExecutionTime = fixedExecutionTime;
            }
        }

        private async void DoWork(object state)
        {
            await ExecuteProcess();
            Console.WriteLine("Cron Job is working at: " + DateTime.Now);
            CalculateNextExecutionTime();
            TimeSpan timeUntilNextExecution = _nextExecutionTime - DateTime.Now;
            _timer.Change(timeUntilNextExecution, Timeout.InfiniteTimeSpan);
        }

        private async Task ExecuteProcess()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedService = scope.ServiceProvider.GetRequiredService<IJoblogRepository>();

                await scopedService.ExeceuteEmployeeStatusJob(new CancellationToken());
                await scopedService.ExecuteRequestDelegationJob(new CancellationToken());

                await Task.CompletedTask;
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Cron Job is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
