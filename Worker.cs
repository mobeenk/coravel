
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using AttendanceNotification.Helper;
using CR_API.DB;
using AttendanceNotifications.Repositories;
using AttendanceNotifications.Models;
using Microsoft.Extensions.Configuration;
using CR_API.Repositories;

namespace AttendanceNotificationsService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public  readonly IConfiguration _config;


        public Worker(ILogger<Worker> logger) 
        {
            _logger = logger;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("The service has been stopped...");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Run();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("ERROR at: {time}", DateTimeOffset.Now);
                }
               // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);



                await Task.Delay(10000 , stoppingToken);

              
            }
        }

        protected async Task Run()
        {

            //Console.WriteLine("Done!");
            //Console.WriteLine();
        }
      
    }
}
