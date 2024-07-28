using AttendanceNotifications.Models;
using AttendanceNotifications.Repositories;
using Coravel.Invocable;
using CR_API.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceNotifications.Invocables
{
    public class DailyAttendanceInvocable : IInvocable
    {
        private readonly ITimeAttendanceRepository _tdRepo;
        public  readonly IConfiguration _config;
        private readonly IActiveDirectoryRepository _adRepo;
        private readonly ILogger<DailyAttendanceInvocable> _logger;

        public DailyAttendanceInvocable(ITimeAttendanceRepository tdRepo, IConfiguration config,
            IActiveDirectoryRepository adRepo, ILogger<DailyAttendanceInvocable> logger)
        {
            _tdRepo = tdRepo;
            _config = config;
            _adRepo = adRepo;
            _logger = logger;
        }
        public async Task Invoke()
        {
            try
            {
                _logger.LogInformation("Daily Attendance report Started @" + DateTime.Now.ToString());
                NotificationsSettings settings = _config.GetSection("NotificationsSettings").Get<NotificationsSettings>();
                var today = DateTime.Now.DayOfWeek;
                int dayIndex = today.ToString().Equals("Sunday")  ? - 3 : -1;
                var prevWorkDay = DateTime.Now.AddDays(dayIndex).Date.ToString("yyyy-MM-dd");
                var emailLabel = DateTime.Now.AddDays(dayIndex).Date.ToString("dd/MM/yyyy");
                var adusers = await _adRepo.GetAllUsers();
                if (adusers.Count > 0)
                {
                    foreach (var user in adusers)
                    {
                        var weeklyReport = await _tdRepo.GetEmpWeeklyAttendance(prevWorkDay, prevWorkDay, user.Email );
                        if (weeklyReport.Count != 0)
                        {
                            Dictionary<string, string> templateParams = new Dictionary<string, string>();
                            templateParams.Add("NewTaskNotification", string.Format("Daily Attendance Report"));
                            templateParams.Add("NewTaskNotificationArabic", string.Format("تقرير الحضور اليومي"));
                            templateParams.Add("ArabicText", string.Format(settings.DailyArabicContent, user.UserName, emailLabel));
                            templateParams.Add("EnglishText", string.Format(settings.DailyEnglishContent, user.UserName, emailLabel));

                            for (int i = 1; i <= weeklyReport.Count; i++)
                            {
                                templateParams.Add("Date" + i, weeklyReport[i - 1].TransDate);
                                templateParams.Add("CheckIn" + i, weeklyReport[i - 1].CheckIN);
                                templateParams.Add("CheckOut" + i, weeklyReport[i - 1].CheckOUT);
                                templateParams.Add("ActualTimeInOffice" + i, weeklyReport[i - 1].ActualTimeInOffice);
                                if (weeklyReport[i - 1].ExecuseTime != "--:--")
                                {
                                    weeklyReport[i - 1].ProductivityTime += " (<b>Time Off: </b>" + weeklyReport[i - 1].ExecuseTime + ")";
                                }
                                templateParams.Add("ProductivityTime" + i, weeklyReport[i - 1].ProductivityTime); //office

                                templateParams.Add("Late" + i, weeklyReport[i - 1].TotalLate);
                                templateParams.Add("Early" + i, weeklyReport[i - 1].EarlyOut);
                                templateParams.Add("MissedProductivity" + i, weeklyReport[i - 1].MissedProductivityTime);
                                //to hide last record
                            }
                            templateParams.Add("HideSummary", "hide");
                            if (weeklyReport.Count < 7)
                            {
                                for (int i = 7; i > weeklyReport.Count; i--)
                                {
                                    templateParams.Add("Date" + i + "Row", "hide");
                                }
                            }

                            EmailBodyWeeklyAtt emailBody = new EmailBodyWeeklyAtt
                            {
                                AppRefID = 0,
                                Recipient = user.Email,
                                Recipient_CC = "",
                                Recipient_BCC = "m.kayali.p@tbc.sa",
                                Subject =   settings.DailySubject,
                                TemplateID = "9",//"7",
                                TemplateParams = templateParams
                            };
                            NotificationsAPIUrls ApiUrls = _config.GetSection("NotificationsAPIUrls").Get<NotificationsAPIUrls>();
                            if (settings.TestNotification.ToLower() == "test") //m.muqeet.p@tbc.sa  && user.Email.ToLower() == "m.kayali.p@tbc.sa"
                            {
                                //emailBody.Recipient_CC = "m.kayali.p@tbc.sa";
                                //emailBody.Recipient_BCC = "m.kayali.p@tbc.sa";
                                var res = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                            }
                            else if ( settings.TestNotification.ToLower() != "test") //sent to all TBC employees
                            {
                                //test case production
                                if (user.Email.ToLower() == "m.kayali.p@tbc.sa")
                                {
                                    //emailBody.Recipient_CC = "m.kayali.p@tbc.sa";
                                    var res = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                                }
                                else //send to all
                                {
                                    var res = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("No Daily records found for the employee: " + user.Email);
                            // _logger.LogInformation("No Weekly records found for the employee @" + user.Email + DateTime.Now.ToString());
                        }
                    }

                    //}
                    Console.WriteLine("Finished Daily report");
                    _logger.LogInformation("Daily ended @" + DateTime.Now.ToString());
                }
                else
                {
                    _logger.LogInformation("No Active Directory Users were found @" + DateTime.Now.ToString());
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error");
                _logger.LogInformation(ex.Message + Environment.NewLine + DateTime.Now.ToString());
            }
        }
    }

}
