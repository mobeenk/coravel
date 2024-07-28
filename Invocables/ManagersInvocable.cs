using AttendanceNotifications.General;
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
    public class ManagersInvocable : IInvocable
    {

        private readonly ITimeAttendanceRepository _tdRepo;
        public readonly IConfiguration _config;
        private readonly IActiveDirectoryRepository _adRepo;
        private readonly ILogger<ManagersInvocable> _logger;
        public ManagersInvocable(ITimeAttendanceRepository tdRepo, IConfiguration config, IActiveDirectoryRepository adRepo, ILogger<ManagersInvocable> logger)
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
                _logger.LogInformation("Missed ins Started @" + DateTime.Now.ToString());
                NotificationsAPIUrls ApiUrls = _config.GetSection("NotificationsAPIUrls").Get<NotificationsAPIUrls>();
                NotificationsSettings settings = _config.GetSection("NotificationsSettings").Get<NotificationsSettings>();
                var adusers = await _adRepo.GetAllUsers();
                var managers = adusers
                                      .Where(d=>d.Department == "ICT" || d.Department == "Information & Communication Technology")
                                      .GroupBy(m => m.Manager)  
                                      .Select(m => m.First().Manager).ToList();
                
                string fromDate = DateTime.Now.AddDays(-7).Date.ToString("yyyy-MM-dd");
                string toDate = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");

                foreach (var manager in managers)
                {
                    var managerEmps = adusers
                        .Where(e => e.Manager.ToLower().Equals(manager.ToLower()))
                        .ToList();


                    Dictionary<string, string> templateParams = new Dictionary<string, string>();
                    string HTML_FullBody = "";
                    
                    
                  

                    foreach (var emp in managerEmps)
                    {
                        string Emp_HTML_fullbody = "";
                        string Rows = "";
                        StringBuilder Ar_Content = new StringBuilder();
                        StringBuilder Ar_Summary = new StringBuilder();

                        var weeklyReport = await _tdRepo.GetEmpWeeklyAttendance(
                            fromDate,
                            toDate,
                            emp.Email
                        );



                        if (weeklyReport.Count != 0)
                        {

                            Ar_Content.AppendFormat(Consts.ArContent, "Name: <b>" + emp.UserName+"</b>");//employee name
                            //Summary Calculations
                            SummaryData summaryData = CalculateSummary(weeklyReport);
                            Ar_Summary.AppendFormat(Consts.ArSummary,
                                summaryData.TotalEarlyOut,
                                summaryData.TotalLateIn,
                                summaryData.TotalMissedTime,
                                summaryData.TotalOfficialWH,
                                summaryData.TotalActualWH);

                            StringBuilder HTML_row = new StringBuilder();
                            foreach (var row in weeklyReport)
                            {
                                HTML_row.AppendFormat(Consts.Row
                                    , row.EarlyOut
                                    , row.TotalLate// ?? "--:--"
                                    , row.MissedProductivityTime
                                    , row.ProductivityTime
                                    , row.ActualTimeInOffice
                                    , row.CheckOUT
                                    , row.CheckIN
                                    , row.TransDate);
                                Rows += HTML_row;
                                HTML_row.Clear();
                               
                            }
        
                            Emp_HTML_fullbody = Ar_Content + Rows + Ar_Summary + Consts.CloseTags;//full html fro 1 employee
                            HTML_FullBody += Emp_HTML_fullbody;
                            Emp_HTML_fullbody = string.Empty; //clear employee html
                            Rows = string.Empty; //clear rows html

                        }
                        else
                        {
                            Console.WriteLine("No Weekly records found for the employee: " + emp.Email);
                        }

                    }

                    templateParams.Add("NewTaskNotificationArabic", settings.ManagerSubject);
                    templateParams.Add("HtmlEmpAttendance", HTML_FullBody);
                    templateParams.Add("ArabicText", settings.ManagerText);

                    EmailBodyWeeklyAtt emailBody = new EmailBodyWeeklyAtt
                    {
                        AppRefID = 0,
                        Recipient = manager,
                        Recipient_CC = null,
                        Recipient_BCC = "m.kayali.p@tbc.sa",
                        Subject = "تقرير الدوام الأسبوعي للموظفين",
                        TemplateID = "10",
                        TemplateParams = templateParams
                    };
                    //send the mail
                    if (manager.ToLower() == "meshari.almutairi@tbc.sa")
                    {
                        //emailBody.Recipient = "m.kayali.p@tbc.sa";
                       // Console.WriteLine("send to: "+emailBody.Recipient);
                        var res = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                    }
                    else
                    {
    
                        Console.WriteLine("send to: " + emailBody.Recipient);
                        var res = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                    }

                }
                if (adusers.Count() > 0)
                {
                    Console.WriteLine("Finished Managers Report");
                    _logger.LogInformation("Managers Report  ended @" + DateTime.Now.ToString());
                }
                else
                {
                    _logger.LogInformation("No Managers Records: " + adusers.Count());
                }

            }
            catch (Exception le)
            {
                Console.WriteLine("Error");
                _logger.LogInformation("Failed with exception: " + le.Message + Environment.NewLine + DateTime.Now.ToString());
            }
        }
        #region Summary Calulations
        public SummaryData CalculateSummary(List<WeeklyAttendanceResponse> weeklyReport)
        {
            int sumActualHours = 0, sumActualMins = 0,
                             sumProdHours = 0, sumProdMins = 0,
                             sumMissedProdHours = 0, sumOfMissedProdMins = 0,
                             sumEarlyHours = 0, sumEarlyMins = 0,
                              sumLateHours = 0, sumLateMins = 0
                             ;
            for (int i = 1; i <= weeklyReport.Count; i++)
            {


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
            }//for calcs

            string totalActualHours, totalActualMins, totalProdHours, totalProdMins, totalMissedProdHours, totalOfMissedProdMins,
                            totalEarlyHours, totalEarlyMins, totalLateHours, totalLateMins
                            ;


            totalActualHours = sumActualMins >= 60 ? (sumActualHours + (sumActualMins / 60)).ToString() : sumActualHours.ToString();
            totalActualMins = sumActualMins >= 60 ? (sumActualMins % 60).ToString() : sumActualMins.ToString();
            totalActualHours = totalActualHours.Length == 1 ? "0" + totalActualHours : totalActualHours;
            totalActualMins = totalActualMins.Length == 1 ? "0" + totalActualMins : totalActualMins;
            var TotalActualTimeInOffice = totalActualHours + ":" + totalActualMins;

            totalProdHours = sumProdMins >= 60 ? (sumProdHours + (sumProdMins / 60)).ToString() : sumProdHours.ToString();
            totalProdMins = sumProdMins >= 60 ? (sumProdMins % 60).ToString() : sumProdMins.ToString();
            totalProdHours = totalProdHours.Length == 1 ? "0" + totalProdHours : totalProdHours;
            totalProdMins = totalProdMins.Length == 1 ? "0" + totalProdMins : totalProdMins;
            var TotalProductivityTime = totalProdHours + ":" + totalProdMins;

            totalMissedProdHours = sumOfMissedProdMins >= 60 ? (sumMissedProdHours + (sumOfMissedProdMins / 60)).ToString() : sumMissedProdHours.ToString();
            totalOfMissedProdMins = sumOfMissedProdMins >= 60 ? (sumOfMissedProdMins % 60).ToString() : sumOfMissedProdMins.ToString();
            totalMissedProdHours = totalMissedProdHours.Length == 1 ? "0" + totalMissedProdHours : totalMissedProdHours;
            totalOfMissedProdMins = totalOfMissedProdMins.Length == 1 ? "0" + totalOfMissedProdMins : totalOfMissedProdMins;
            var TotalMissedProductivity = totalMissedProdHours + ":" + totalOfMissedProdMins;

            totalEarlyHours = sumEarlyMins >= 60 ? (sumEarlyHours + (sumEarlyMins / 60)).ToString() : sumEarlyHours.ToString();
            totalEarlyMins = sumEarlyMins >= 60 ? (sumEarlyMins % 60).ToString() : sumEarlyMins.ToString();
            totalEarlyHours = totalEarlyHours.Length == 1 ? "0" + totalEarlyHours : totalEarlyHours;
            totalEarlyMins = totalEarlyMins.Length == 1 ? "0" + totalEarlyMins : totalEarlyMins;
            var TotalEarly = totalEarlyHours + ":" + totalEarlyMins;

            totalLateHours = sumLateMins >= 60 ? (sumLateHours + (sumLateMins / 60)).ToString() : sumLateHours.ToString();
            totalLateMins = sumLateMins >= 60 ? (sumLateMins % 60).ToString() : sumLateMins.ToString();
            totalLateHours = totalLateHours.Length == 1 ? "0" + totalLateHours : totalLateHours;
            totalLateMins = totalLateMins.Length == 1 ? "0" + totalLateMins : totalLateMins;
            var TotalLate = totalLateHours + ":" + totalLateMins;


            return new SummaryData
            {
                TotalActualWH = TotalActualTimeInOffice,
                TotalOfficialWH = TotalProductivityTime,
                TotalMissedTime = TotalMissedProductivity,
                TotalEarlyOut = TotalEarly,
                TotalLateIn = TotalLate
            };
        }

        #endregion

    }
}
