using AttendanceNotification.Helper;
using AttendanceNotifications.Models;
using CR_API.DB;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceNotifications.Repositories
{
    public interface ITimeAttendanceRepository
    {
        Task<List<InEmps>> GetMissingSignInEmps();
        Task<List<OutEmps>> GetMissingSignOutEmps();
        Task<HttpResponseMessage> CallNotificationAPI(object body, string apiUrlName);
        double ScheduleAt(int hour, int minutes);
        Task<List<WeeklyAttendanceResponse>> GetEmpWeeklyAttendance(string fromDate, string toDate, string email);
    }
    public class TimeAttendanceRepository : ITimeAttendanceRepository
    {
        private SQLDBHelper _entities;
        private SQLDBHelper _timeAttDev;
        public readonly IConfiguration _configuration;

        //private readonly AppSettings _appSettings;
        public TimeAttendanceRepository(IConfiguration configuration)
        {
            
            _configuration = configuration;
            ConnectionStrings connections = configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
            _entities = new SQLDBHelper(connections.TimeAttendanceDB);
            _timeAttDev = new SQLDBHelper(connections.TimeAttendanceDev);


        }
        public async Task<List<InEmps>> GetMissingSignInEmps()
        {
            //try
            //{
                
                //var users = await _context.ActiveDirectoryUsers.ToListAsync();
                var users = await _entities.ExecuteToDataTable<InEmps>("TimeAtt_GetMissingSignInEmps").ConfigureAwait(false);
                return users;
            //}
            //catch (Exception ex)
            //{
            //    return new List<InEmps>() ;
            //}
          
        }
        public async Task<List<OutEmps>> GetMissingSignOutEmps()
        {
            //try
            //{
                var users = await _entities.ExecuteToDataTable<OutEmps>("TimeAtt_GetMissingSignOutEmps").ConfigureAwait(false);
                return users;
            //}
            //catch (Exception ex)
            //{

            //    return new List<OutEmps>();
            //}

        }
        public async Task<List<WeeklyAttendanceResponse>> GetEmpWeeklyAttendance(string fromDate, string toDate, string email)
        {
            //try
            //{
                var users = await _entities.ExecuteToDataTable<WeeklyAttendanceResponse>("TimeAtt_GetWeeklyAttendance",
                        new SqlParameter[]
                        {
                            new SqlParameter("@FromDate", fromDate),
                            new SqlParameter("@ToDate", toDate),
                            new SqlParameter("@Email", email),
                        }
                    ).ConfigureAwait(false);
                return users;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}

        }
        public async  Task<HttpResponseMessage> CallNotificationAPI(object body, string apiUrlName)
        {
            string jsonBody = JsonConvert.SerializeObject(body);
            string apiURL = apiUrlName;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, apiURL);
                var byteArray = Encoding.ASCII.GetBytes("tbcext:tbcext!43@1O");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                return await client.SendAsync(request);
            }
        }
        public double ScheduleAt(int hour, int minutes)
        {
            double timeNow = DateTime.Now
                //.AddHours(2)
                .Subtract(DateTime.MinValue).TotalSeconds;

            DateTime dt2 = DateTime.Now;
            TimeSpan ts = new TimeSpan(hour, minutes, 0);
            dt2 = dt2.Date + ts;
            double schedule = dt2.Subtract(DateTime.MinValue).TotalSeconds;

            double scheduleAfter = 0;
            if (timeNow < schedule)
            {
                scheduleAfter = schedule - timeNow;
            }
            else
            {
                DateTime dt = DateTime.Now;
                var dt_tomorow = dt.AddDays(1);
                TimeSpan ts_tomoworw = new TimeSpan(hour, minutes, 0);
                dt_tomorow = dt_tomorow.Date + ts_tomoworw;
                double scheduleTomorow = dt_tomorow.Subtract(DateTime.MinValue).TotalSeconds;
                scheduleAfter = scheduleTomorow - timeNow;

            }
            return scheduleAfter;
        }
    }
}
