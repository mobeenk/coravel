
using AttendanceNotifications.Invocables;
using AttendanceNotifications.Models;
using AttendanceNotifications.Repositories;
using AttendanceNotificationsService;
using Coravel;
using CR_API.DB;
using CR_API.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace activedirectoryService
{
    public class Program
    {
        public static void Main(string[] args)
        {


            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"C:\temp\attendance_log\Attendance_LogFile.txt")
                .CreateLogger();

            try
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true).Build();

                NotificationsSettings time = config.GetSection("NotificationsSettings").Get<NotificationsSettings>();
                var InStart = int.Parse(time.MissedInHours);
                var InEnd = int.Parse(time.MissedInMinutes);
                var OutStart = int.Parse(time.MissedOutHours);
                var OutEnd = int.Parse(time.MissedOutMinutes);
                var WeeklyStart = int.Parse(time.WeeklyReportHours);
                var WeeklyEnd = int.Parse(time.WeeklyReportMinutes);
                var DailyStart = int.Parse(time.DailyReportHours); 
                var DailyEnd = int.Parse(time.DailyReportMinutes);

                var WeeklyManagerHours = int.Parse(time.WeeklyManagerHours);
                var WeeklyManagerMins = int.Parse(time.WeeklyManagerMinutes);

                Log.Information("Starting up the service");
                Log.Information("In Start Time: " + InStart.ToString()+":"+ InEnd.ToString());
                Log.Information("Out Start Time: " + OutStart.ToString()+ ":" + OutEnd.ToString());
                Log.Information("Weekly Time: " + WeeklyStart.ToString()+ ":" + WeeklyEnd.ToString());
                Log.Information("Daily Time: " + DailyStart.ToString() + ":" + DailyEnd.ToString());
                Log.Information("Weekly Managers Time: " + WeeklyManagerHours.ToString() + ":" + WeeklyManagerMins.ToString());


                IHost host = CreateHostBuilder(args).Build();
                host.Services.UseScheduler(scheduler =>
                {
                    if (time.TestNotification == "test")
                    {
                        //scheduler.Schedule<MissedInInvocable>().EveryFiveSeconds().PreventOverlapping("MissedInInvocable");
                        //scheduler.Schedule<MissedOutInvocable>().EveryFiveSeconds().PreventOverlapping("MissedOutInvocable");
                        scheduler.Schedule<WeeklyAttendanceInvocable>().EveryFiveSeconds().PreventOverlapping("WeeklyAttendanceInvocable");
                        //scheduler.Schedule<DailyAttendanceInvocable>().EveryFiveSeconds().PreventOverlapping("DailyAttendanceInvocable");
                        //scheduler.Schedule<ManagersInvocable>().EveryFiveSeconds().PreventOverlapping("ManagersInvocable");


                        //scheduler.OnWorker("ManagersInvocable");
                        //scheduler.Schedule<ManagersInvocable>().DailyAt(WeeklyManagerHours, WeeklyManagerMins).Sunday()
                        //.PreventOverlapping("ManagersInvocable");
                    }
                    else
                    {
                        //missed in
                        scheduler.OnWorker("MissedInInvocable");
                        scheduler.Schedule<MissedInInvocable>().DailyAt(InStart, InEnd)
                         .Sunday().Monday().Tuesday().Wednesday().Thursday()   
                              .PreventOverlapping("MissedInInvocable");
                        //missed out
                        scheduler.OnWorker("MissedOutInvocable");
                        scheduler.Schedule<MissedOutInvocable>().DailyAt(OutStart, OutEnd)
                         .Sunday().Monday().Tuesday().Wednesday().Thursday() 
                            .PreventOverlapping("MissedOutInvocable");

                        scheduler.OnWorker("WeeklyAttendanceInvocable");
                        scheduler.Schedule<WeeklyAttendanceInvocable>().DailyAt(WeeklyStart, WeeklyEnd).Sunday()
                        .PreventOverlapping("WeeklyAttendanceInvocable");

                        scheduler.OnWorker("DailyAttendanceInvocable");
                        scheduler.Schedule<DailyAttendanceInvocable>().DailyAt(DailyStart, DailyEnd)
                            .Sunday().Monday().Tuesday().Wednesday().Thursday()
                        .PreventOverlapping("DailyAttendanceInvocable");
                        //Manager report
                        scheduler.OnWorker("ManagersInvocable");
                        scheduler.Schedule<ManagersInvocable>().DailyAt(WeeklyManagerHours, WeeklyManagerMins).Sunday()
                        .PreventOverlapping("ManagersInvocable");
                    }


                });
                host.Run();
                //return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the serivce");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
               // .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddTransient<ITimeAttendanceRepository, TimeAttendanceRepository>();
                    services.AddTransient<IActiveDirectoryRepository, ActiveDirectoryRepository>();
                    services.AddScheduler();
                    services.AddTransient<MissedInInvocable>();
                    services.AddTransient<MissedOutInvocable>();
                    services.AddTransient<WeeklyAttendanceInvocable>();
                    services.AddTransient<DailyAttendanceInvocable>();
                    services.AddTransient<ManagersInvocable>();
                    ;
                })
            .UseSerilog();
    }
}
