using AttendanceNotifications.Models;
using AttendanceNotifications.Repositories;
using Coravel.Invocable;
using CR_API.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AttendanceNotifications.Invocables
{
    public class WeeklyAttendanceInvocable : IInvocable
    {
        private readonly ITimeAttendanceRepository _tdRepo;
        public readonly IConfiguration _config;
        private readonly IActiveDirectoryRepository _adRepo;
        private readonly ILogger<WeeklyAttendanceInvocable> _logger;

        public WeeklyAttendanceInvocable(ITimeAttendanceRepository tdRepo, IConfiguration config, IActiveDirectoryRepository adRepo, ILogger<WeeklyAttendanceInvocable> logger)
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
                _logger.LogInformation("WeeklyAttendance report Started @" + DateTime.Now.ToString());
                NotificationsSettings settings = _config.GetSection("NotificationsSettings").Get<NotificationsSettings>();
                var fromDate = "";
                var toDate = "";
                var today = DateTime.Now.DayOfWeek;

                //if (today.ToString().Equals("Sunday"))
                //{
                fromDate = DateTime.Now.AddDays(-7).Date.ToString("yyyy-MM-dd");
                toDate = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");

                var adusers = await _adRepo.GetAllUsers();
                if (adusers.Count > 0)
                {
                    foreach (var user in adusers)
                    {
                        if (user.Email == "l.alnowaiser.t@tbc.sa")
                        {
                            var x = 0;
                        }

                        var weeklyReport = await _tdRepo.GetEmpWeeklyAttendance(
                            fromDate,
                            toDate,
                            user.Email
                        );
                        if (weeklyReport.Count != 0)
                        {
                            Dictionary<string, string> templateParams = new Dictionary<string, string>();
                            templateParams.Add("NewTaskNotification", string.Format("Weekly Attendance Report"));
                            templateParams.Add("NewTaskNotificationArabic", string.Format("تقرير الحضور الأسبوعي"));
                            templateParams.Add("ArabicText", string.Format(settings.WeeklyReportAR, user.UserName));
                            templateParams.Add("EnglishText", string.Format(settings.WeeklyReportEN, user.UserName));

                             int sumActualHours = 0, sumActualMins = 0 ,
                                sumProdHours = 0 , sumProdMins = 0 ,
                                sumMissedProdHours = 0, sumOfMissedProdMins = 0 ,
                                sumEarlyHours = 0, sumEarlyMins =0,
                                 sumLateHours = 0, sumLateMins = 0
                                ;
                            //hoursMins(sumActualHours, sumActualMins, weeklyReport[i - 1].ActualTimeInOffice);
                            for (int i = 1; i <= weeklyReport.Count; i++)
                            {
                                templateParams.Add("Date" + i, weeklyReport[i - 1].TransDate);
                                templateParams.Add("CheckIn" + i, weeklyReport[i - 1].CheckIN);
                                templateParams.Add("CheckOut" + i, weeklyReport[i - 1].CheckOUT);
                                templateParams.Add("ActualTimeInOffice" + i, weeklyReport[i - 1].ActualTimeInOffice);
                                
                                if (weeklyReport[i - 1].ProductivityTime.Contains(":") && weeklyReport[i - 1].ProductivityTime != "--:--")
                                {
                                    //fix time off
                                    string[] hoursAndMins = weeklyReport[i - 1].ProductivityTime.Split(":");
                                    sumProdHours += int.Parse(hoursAndMins[0]);
                                    sumProdMins += int.Parse(hoursAndMins[1]);
                                    if (weeklyReport[i - 1].ExecuseTime != "--:--")
                                    {
                                        weeklyReport[i - 1].ProductivityTime += " (<b>Time Off: </b>" + weeklyReport[i - 1].ExecuseTime + ")";
                                        string[] hoursAndMinsTimeOff = weeklyReport[i - 1].ExecuseTime.Split(":");
                                        sumProdHours += int.Parse(hoursAndMinsTimeOff[0]);
                                        sumProdMins += int.Parse(hoursAndMinsTimeOff[1]);
                                    }
                                }
                                templateParams.Add("ProductivityTime" + i, weeklyReport[i - 1].ProductivityTime); //Official Working Hours 
                                templateParams.Add("Late" + i, weeklyReport[i - 1].TotalLate);
                                templateParams.Add("Early" + i, weeklyReport[i - 1].EarlyOut);
                                templateParams.Add("MissedProductivity" + i, weeklyReport[i - 1].MissedProductivityTime);
                                //templateParams.Add("ShowDate" + i, "show");
                                if (weeklyReport[i - 1].ActualTimeInOffice.Contains(":") && weeklyReport[i - 1].ActualTimeInOffice != "--:--")
                                {
                                    string[] hoursAndMins = weeklyReport[i - 1].ActualTimeInOffice.Split(":");
                                    sumActualHours += int.Parse(hoursAndMins[0]);
                                    sumActualMins += int.Parse(hoursAndMins[1]);
                                }

                               
                                if (weeklyReport[i - 1].MissedProductivityTime.Contains(":") && weeklyReport[i - 1].MissedProductivityTime != "--:--")
                                {
                                    string[] hoursAndMins = weeklyReport[i - 1].MissedProductivityTime.Split(":");
                                    sumMissedProdHours += int.Parse(hoursAndMins[0]);
                                    sumOfMissedProdMins += int.Parse(hoursAndMins[1]);
                                }
                                if (weeklyReport[i - 1].EarlyOut.Contains(":") && weeklyReport[i - 1].EarlyOut != "--:--")
                                {
                                    string[] hoursAndMins = weeklyReport[i - 1].EarlyOut.Split(":");
                                    sumEarlyHours += int.Parse(hoursAndMins[0]);
                                    sumEarlyMins += int.Parse(hoursAndMins[1]);
                                }
                                if (weeklyReport[i - 1].TotalLate.Contains(":") && weeklyReport[i - 1].TotalLate != "--:--")
                                {
                                    string[] hoursAndMins = weeklyReport[i - 1].TotalLate.Split(":");
                                    sumLateHours += int.Parse(hoursAndMins[0]);
                                    sumLateMins += int.Parse(hoursAndMins[1]);
                                }


                            }
                            string totalActualHours = "", totalActualMins = "",
                            totalProdHours = "", totalProdMins = "",
                            totalMissedProdHours = "", totalOfMissedProdMins = "",
                            totalEarlyHours = "", totalEarlyMins = "",
                             totalLateHours = "", totalLateMins = ""
                            ;

                            totalActualHours = sumActualMins >= 60 ? (sumActualHours + (sumActualMins / 60)).ToString() : sumActualHours.ToString();
                            totalActualMins = sumActualMins >= 60 ?  (sumActualMins %  60).ToString() : sumActualMins.ToString();
                            totalActualHours = totalActualHours.Length == 1 ? "0" + totalActualHours : totalActualHours;
                            totalActualMins = totalActualMins.Length == 1 ? "0" + totalActualMins : totalActualMins;
                            templateParams.Add("TotalActualTimeInOffice", totalActualHours+":"+ totalActualMins);

                            totalProdHours = sumProdMins >= 60 ? (sumProdHours + (sumProdMins / 60)).ToString() : sumProdHours.ToString();
                            totalProdMins = sumProdMins >= 60 ? (sumProdMins % 60).ToString() : sumProdMins.ToString();
                            totalProdHours = totalProdHours.Length == 1 ? "0" + totalProdHours : totalProdHours;
                            totalProdMins = totalProdMins.Length == 1 ? "0" + totalProdMins : totalProdMins;
                            templateParams.Add("TotalProductivityTime", totalProdHours + ":" + totalProdMins);

                            totalMissedProdHours = sumOfMissedProdMins >= 60 ? (sumMissedProdHours + (sumOfMissedProdMins / 60)).ToString() : sumMissedProdHours.ToString();
                            totalOfMissedProdMins = sumOfMissedProdMins >= 60 ? (sumOfMissedProdMins % 60).ToString() : sumOfMissedProdMins.ToString();
                            totalMissedProdHours = totalMissedProdHours.Length == 1 ? "0" + totalMissedProdHours : totalMissedProdHours;
                            totalOfMissedProdMins = totalOfMissedProdMins.Length == 1 ? "0" + totalOfMissedProdMins : totalOfMissedProdMins;
                            templateParams.Add("TotalMissedProductivity", totalMissedProdHours + ":" + totalOfMissedProdMins);

                            totalEarlyHours = sumEarlyMins >= 60 ? (sumEarlyHours + (sumEarlyMins / 60)).ToString() : sumEarlyHours.ToString();
                            totalEarlyMins = sumEarlyMins >= 60 ? (sumEarlyMins % 60).ToString() : sumEarlyMins.ToString();
                            totalEarlyHours = totalEarlyHours.Length == 1 ? "0" + totalEarlyHours : totalEarlyHours;
                            totalEarlyMins = totalEarlyMins.Length == 1 ? "0" + totalEarlyMins : totalEarlyMins;
                            templateParams.Add("TotalEarly", totalEarlyHours + ":" + totalEarlyMins);

                            totalLateHours = sumLateMins >= 60 ? (sumLateHours + (sumLateMins / 60)).ToString() : sumLateHours.ToString();
                            totalLateMins = sumLateMins >= 60 ? (sumLateMins % 60).ToString() : sumLateMins.ToString();
                            totalLateHours = totalLateHours.Length == 1 ? "0" + totalLateHours : totalLateHours;
                            totalLateMins = totalLateMins.Length == 1 ? "0" + totalLateMins : totalLateMins;
                            templateParams.Add("TotalLate", totalLateHours + ":" + totalLateMins);

                            Console.WriteLine("{0}: Actual: {1}, Productivty: {2}, MissedProd: {3}, Early: {4}, Late: {5}",
                                user.Email, totalActualHours + ":" + totalActualMins, totalProdHours + ":" + totalProdMins,
                                totalMissedProdHours + ":" + totalOfMissedProdMins, totalEarlyHours + ":" + totalEarlyMins, totalLateHours + ":" + totalLateMins

                                );

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
                                Recipient_CC = user.Manager,
                                Subject = settings.WeeklySubject,
                                TemplateID = "7",
                                TemplateParams = templateParams
                            };
                            NotificationsAPIUrls ApiUrls = _config.GetSection("NotificationsAPIUrls").Get<NotificationsAPIUrls>();
                            if (
                                  // user.Email.ToLower() == "hajar.alsuwayed@tbc.sa" &&

                                  settings.TestNotification.ToLower() == "test")
                            {
                                //emailBody.Recipient_CC = "m.kayali.p@tbc.sa; meshari.almutairi@tbc.sa";
                                emailBody.Recipient_BCC = "m.kayali.p@tbc.sa";
                                var res = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                            }
                            else if ( settings.TestNotification.ToLower() != "test") //sent to all TBC employees
                            {
                                emailBody.Recipient_CC = null ;
                                emailBody.Recipient_BCC = "m.kayali.p@tbc.sa";
                                //test case
                                if ( user.Email.ToLower() == "m.kayali.p@tbc.sa" || user.Email.ToLower() == "m.muqeet.p@tbc.sa")
                                {
                                   // emailBody.Recipient_CC = "m.kayali.p@tbc.sa";
                                    var res = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                                }
                                else
                                {
                                    Console.WriteLine("Send Mail to: " + emailBody.Recipient);
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
                            Console.WriteLine("No Weekly records found for the employee: " + user.Email);
                            // _logger.LogInformation("No Weekly records found for the employee @" + user.Email + DateTime.Now.ToString());
                        }
                    }
                    //}
                    Console.WriteLine("Finished Weekly report");
                    _logger.LogInformation("Weekly ended @" + DateTime.Now.ToString());
                }
                else
                {
                    _logger.LogInformation("No Active Directory Users were found @" + DateTime.Now.ToString());
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error"+ ex.Message);
                _logger.LogInformation(ex.Message + Environment.NewLine + DateTime.Now.ToString());
            }

        }
    }
}
