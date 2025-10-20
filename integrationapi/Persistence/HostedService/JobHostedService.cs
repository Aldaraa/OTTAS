using Application.Features.HotDeskFeature.DepartmentSend;
using Application.Features.HotDeskFeature.EmployeeSend;
using Application.Features.HotDeskFeature.EmployeeStatusSend;
using Application.Features.OtinfoFeature.JobInfo;
using Application.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence.Repositories;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Logging;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.HostedService
{
    public class JobHostedService : IHostedService
    {
        #region Constructor
        private readonly JobScopedService _jobScopedService;
        private readonly IScheduler _scheduler;
        ILogger<JobHostedService> _logger;


        private const string JOBKEY_PREFIX = "TASINTEGRATION_JOB_";

        private const string JOBTRIGGER_PREFIX = "TASINTEGRATION_TRIGGER_";



        private readonly IServiceScopeFactory _serviceScopeFactory;

        public JobHostedService(JobScopedService jobScopedService, IServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider, ILogger<JobHostedService> logger)
        {
            _jobScopedService = jobScopedService;
            // Create a Quartz scheduler
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().Result;

            _serviceScopeFactory = serviceScopeFactory;
            _scheduler.JobFactory = serviceProvider.GetRequiredService<IJobFactory>();
            _logger = logger;



        }
        #endregion


        #region JobStart&&Stop

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await LoadSavedJobExecute();
            await _scheduler.Start();
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _scheduler.Shutdown().Wait();

            return Task.CompletedTask;
        }
        #endregion


        #region LoadJobStartService

        private async Task LoadSavedJobExecute()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedService = scope.ServiceProvider.GetRequiredService<ITransportInfoRepository>();

                await scopedService.LoadData();

                await Task.CompletedTask;
            }


        }
        #endregion

        #region OTINFO_JOB
        public async Task ScheduleEmployeeDataSyncJob(int minute)
        {
            JobKey jobKey = new JobKey(JOBKEY_PREFIX + "OTINFO_EMPLOYEEDATA");
            bool jobExists = await _scheduler.CheckExists(jobKey);

            if (!jobExists)
            {
                try
                {
                    IJobDetail newJob = JobBuilder.Create<MyJob>()
                        .WithIdentity(JOBKEY_PREFIX + "OTINFO_EMPLOYEEDATA")
                        .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("every65MinutesTrigger", "OTINFO_EMPLOYEEDATA")
                        .StartNow()
                        .WithSimpleSchedule(x => x
                            .WithIntervalInMinutes(minute)
                            .RepeatForever())
                        .Build();

                    await _scheduler.ScheduleJob(newJob, trigger);

                    Console.WriteLine("job added ------OTINFO ScheduleEmployeeDataSyncJob --------------------->");
                }
                catch (Exception ex)
                {
                  _logger.LogError(ex.Message, ex);
                }
            }
        }

        #endregion

        #region OTDESK_JOB

        public async Task ScheduleEmployeeSyncJob(string timequery)
        {
            JobKey jobKey = new JobKey(JOBKEY_PREFIX + "OTDESK_EMPLOYEE");
            bool jobExists = await _scheduler.CheckExists(jobKey);

            if (!jobExists)
            {
                try
                {
                    IJobDetail newJob = JobBuilder.Create<OTDeskEmployeeSyncJob>()
                        .WithIdentity(jobKey)
                        .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("dailyTrigger", "OTDESK_EMPLOYEE")
                        .StartNow()
                       // .WithCronSchedule("5 37 07 * * ?")
                        .WithCronSchedule(timequery)

                        .Build();

                    await _scheduler.ScheduleJob(newJob, trigger);

                    Console.WriteLine("job added ------OTDESK ScheduleEmployeeSyncJob --------------------->");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }
        }


        public async Task ScheduleEmployeeStatusSyncJob(string timequery)
        {
            
            JobKey jobKey = new JobKey(JOBKEY_PREFIX + "OTDESK_EMPLOYEESTATUS");
            bool jobExists = await _scheduler.CheckExists(jobKey);

            if (!jobExists)
            {
                try
                {
                    IJobDetail newJob = JobBuilder.Create<OTDeskEmployeeStatusSyncJob>()
                        .WithIdentity(jobKey)
                        .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("dailyTrigger", "OTDESK_EMPLOYEESTATUS")
                        .StartNow()
                        .WithCronSchedule(timequery)

                        .Build();

                    await _scheduler.ScheduleJob(newJob, trigger);

                    Console.WriteLine("job added ------OTDESK ScheduleEmployeeStatusSyncJob--------------------->");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }
        }


        public async Task ScheduleDepartmentSyncJob(string timequery)
        {
            JobKey jobKey = new JobKey(JOBKEY_PREFIX + "OTDESK_DEPARTMENT");
            bool jobExists = await _scheduler.CheckExists(jobKey);

            if (!jobExists)
            {
                try
                {
                    IJobDetail newJob = JobBuilder.Create<OTDeskDepartmentSyncJob>()
                        .WithIdentity(jobKey)
                        .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("dailyTrigger", "OTDESK_DEPARTMENT")
                        .StartNow()
                        .WithCronSchedule(timequery)

                        .Build();

                    await _scheduler.ScheduleJob(newJob, trigger);

                    Console.WriteLine("job added ------OTDESK ScheduleDepartmentSyncJob--------------------->");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }
        }

        #endregion


        #region GetRunningJob

        public async Task<List<JobInfoResponse>> GetRunningJobsInfo()
        {
            var triggerGroupNames = await _scheduler.GetTriggerGroupNames();


            var returnData = new List<JobInfoResponse>();
            foreach (var groupName in triggerGroupNames)
            {
                var triggerKeys = await _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(groupName));

                foreach (var triggerKey in triggerKeys)
                {
                    ITrigger trigger = await _scheduler.GetTrigger(triggerKey);

                    if (trigger != null)
                    {
                        var previousFireTimeUtc = trigger.GetPreviousFireTimeUtc();
                        var nextFireTimeUtc = trigger.GetNextFireTimeUtc();

                        DateTime? previousFireTimeLocal = previousFireTimeUtc?.ToLocalTime().DateTime;
                        DateTime? nextFireTimeLocal = nextFireTimeUtc?.ToLocalTime().DateTime;

                        int repeatCount = -1;
                        if (trigger is ISimpleTrigger simpleTrigger)
                        {
                            repeatCount = simpleTrigger.RepeatCount;
                        }


                        returnData.Add(new JobInfoResponse
                        {
                            JobKey = trigger.JobKey.Name,
                            JobRunTime = previousFireTimeLocal,
                            NextFireTime = nextFireTimeLocal,
                            RepeatCount = repeatCount
                        });
                    }
                }
            }

            return returnData;  
        }


        #endregion



        #region OtherClasses


        public class MyJob : IJob
        {
            private readonly IServiceScopeFactory _serviceScopeFactory;
            private readonly ILogger<MyJob> _logger;
            private readonly IConfiguration _configuration;
            public MyJob(IServiceScopeFactory serviceScopeFactory, ILogger<MyJob> logger, IConfiguration configuration)
            {
                _serviceScopeFactory = serviceScopeFactory;
                _logger = logger;
                _configuration = configuration;
            }


            public async Task Execute(IJobExecutionContext context)
            {

                try
                {

                    _logger.LogInformation("job started at {Time} | Trigger={TriggerKey}", DateTime.Now, context.Trigger.Key);

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var scopedService = scope.ServiceProvider.GetRequiredService<ITransportInfoRepository>();


                        bool productionMode = _configuration.GetSection("AppSettings:OTINFO:EnableProductionMode").Get<bool>();

                        if (productionMode)
                        {
                            await scopedService.SendTransportData(new CancellationToken());
                            _logger.LogInformation("SendTransportData executed at {Time}", DateTime.Now);
                        }
                        else
                        {
                            _logger.LogWarning("⚠️ ProductionMode=false → SendTransportData skipped at {Time}", DateTime.Now);
                        }
                    }
                    _logger.LogInformation("job finished at {Time} | Trigger={TriggerKey}", DateTime.Now, context.Trigger.Key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception thrown from job execution: {ex.Message}");
                    throw;
                }

            }
        }


        public class OTDeskEmployeeSyncJob : IJob
        {
            private readonly IServiceScopeFactory _serviceScopeFactory;
            private readonly ILogger<OTDeskEmployeeSyncJob> _logger;
            public OTDeskEmployeeSyncJob(IServiceScopeFactory serviceScopeFactory, ILogger<OTDeskEmployeeSyncJob> logger)
            {
                _serviceScopeFactory = serviceScopeFactory;
                _logger = logger;
            }


            public async Task Execute(IJobExecutionContext context)
            {

                try
                {

                    _logger.LogInformation("job started at {Time} | JobKey={JobKey} | Trigger={TriggerKey}", DateTime.Now, context.JobDetail.Key, context.Trigger.Key);
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var scopedService = scope.ServiceProvider.GetRequiredService<IHotDeskRepository>();
                        await scopedService.EmployeeSendData(new EmployeeSendRequest(), new CancellationToken());
                    }

                    _logger.LogInformation(
                        "Job finished at {Time} | JobKey={JobKey} | TriggerKey={TriggerKey}",
                        DateTime.Now,
                        context.JobDetail.Key,
                        context.Trigger.Key
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Exception thrown from HotDesk job execution at {Time}", DateTime.Now);
                    throw;
                }

            }
        }


        public class OTDeskEmployeeStatusSyncJob : IJob
        {
            private readonly IServiceScopeFactory _serviceScopeFactory;
            private readonly ILogger<OTDeskEmployeeStatusSyncJob> _logger;
            public OTDeskEmployeeStatusSyncJob(IServiceScopeFactory serviceScopeFactory, ILogger<OTDeskEmployeeStatusSyncJob> logger)
            {
                _serviceScopeFactory = serviceScopeFactory;
                _logger = logger;
            }


            public async Task Execute(IJobExecutionContext context)
            {

                try
                {

                    _logger.LogInformation("job started at {Time} | JobKey={JobKey} | Trigger={TriggerKey}", DateTime.Now, context.JobDetail.Key, context.Trigger.Key);

                    Console.WriteLine("CALLED FUNCTION" + DateTime.Now + "-----------------------------" + context.Trigger.JobKey);

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var scopedService = scope.ServiceProvider.GetRequiredService<IHotDeskRepository>();
                        await scopedService.EmployeeStatusSendData(new EmployeeStatusSendRequest(), new CancellationToken());
                    }

                    _logger.LogInformation(
                        "Job finished at {Time} | JobKey={JobKey} | TriggerKey={TriggerKey}",
                        DateTime.Now,
                        context.JobDetail.Key,
                        context.Trigger.Key
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception thrown from job execution: {ex.Message}");
                    _logger.LogError(ex, "❌ Exception thrown from HotDesk job execution at {Time}", DateTime.Now);
                    throw;
                }

            }


        }

        public class OTDeskDepartmentSyncJob : IJob
        {
            private readonly IServiceScopeFactory _serviceScopeFactory;
            private readonly ILogger<OTDeskDepartmentSyncJob> _logger;
            public OTDeskDepartmentSyncJob(IServiceScopeFactory serviceScopeFactory, ILogger<OTDeskDepartmentSyncJob> logger)
            {
                _serviceScopeFactory = serviceScopeFactory;
                _logger = logger;
            }


            public async Task Execute(IJobExecutionContext context)
            {

                try
                {

                    _logger.LogInformation("job started at {Time} | JobKey={JobKey} | Trigger={TriggerKey}", DateTime.Now, context.JobDetail.Key, context.Trigger.Key);

                    Console.WriteLine("CALLED FUNCTION" + DateTime.Now + "-----------------------------" + context.Trigger.JobKey);

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var scopedService = scope.ServiceProvider.GetRequiredService<IHotDeskRepository>();
                        await scopedService.DepartmentSendData(new DepartmentSendRequest(), new CancellationToken());
                    }

                    _logger.LogInformation(
                        "Job finished at {Time} | JobKey={JobKey} | TriggerKey={TriggerKey}",
                        DateTime.Now,
                        context.JobDetail.Key,
                        context.Trigger.Key
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception thrown from job execution: {ex.Message}");
                    _logger.LogError(ex, "❌ Exception thrown from HotDesk job execution at {Time}", DateTime.Now);
                    throw;
                }

            }


        }



        public class CustomQuartzJobFactory : IJobFactory
        {
            private readonly IServiceProvider _serviceProvider;

            public CustomQuartzJobFactory(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
            {
                var jobType = bundle.JobDetail.JobType;
                return (IJob)_serviceProvider.GetRequiredService(jobType);
            }

            public void ReturnJob(IJob job)
            {
                if (job is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public class CronSchedule
        {
            public string? Name { get; set; }

            public string CronExpression { get; set; }
        }

        #endregion



    }
}
