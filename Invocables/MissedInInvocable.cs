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
    public class MissedInInvocable : IInvocable
    {

        private readonly ITimeAttendanceRepository _tdRepo;
        public  readonly IConfiguration _config;
        private readonly IActiveDirectoryRepository _adRepo;
        private readonly ILogger<MissedInInvocable> _logger;
        public MissedInInvocable(ITimeAttendanceRepository tdRepo, IConfiguration config, IActiveDirectoryRepository adRepo, ILogger<MissedInInvocable> logger )
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
                var inEmps = await _tdRepo.GetMissingSignInEmps();
                if (inEmps.Count() > 0)
                {
                    _logger.LogInformation("fetched: "+ inEmps.Count() + " Missed ins records.");
                    foreach (var inEmp in inEmps)
                    {


                        TemplateParams templateParams = new TemplateParams()
                        {
                            ArabicContent = string.Format(settings.EmailArabicContentIn, inEmp.FULLNAME),
                            EnglishContent = string.Format(settings.EmailEnglishContentIn, inEmp.FULLNAME),
                            NewTaskNotification = settings.EmailTitleEnIn,
                            NewTaskNotificationArabic = settings.EmailTitleArIn   //إاشعار نظام الحضور
                        }
                        ;
                        EmailBody emailBody = new EmailBody
                        {
                            AppRefID = 0,
                            Recipient = inEmp.EMAIL,
                            Recipient_CC = inEmp.MANAGER,
                            Subject = settings.InSubject,
                            TemplateID = "8",
                            TemplateParams = templateParams
                        };
                        SMSBody smsBody = new SMSBody
                        {
                            AppRefID = 0,
                            Body = string.Format(settings.SmsContentIn, inEmp.FULLNAME),
                            ID = 0,
                            MobileNo = inEmp.MOBILE
                        };
                        if (inEmp.EMAIL == "m.kayali.p@tbc.sa" && settings.TestNotification.ToLower() == "test")
                        {
                            _logger.LogInformation("Test started: " + inEmp.EMAIL);
                            inEmp.MANAGER = "m.muqeet.p@tbc.sa";
                            emailBody.Recipient_CC = inEmp.MANAGER + ";m.kayali.p@tbc.sa";
                            var Email_Response = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                            if (inEmp.EMAIL.Contains(".p@"))
                            {
                                var SMS_Response = await _tdRepo.CallNotificationAPI(smsBody, ApiUrls.SmsAPI);
                            }
                            else
                            {

                            }
                        }
                        else if ( settings.TestNotification.ToLower() != "test")
                        {
                            //send notifications
                            if (inEmp.EMAIL == "m.kayali.p@tbc.sa")
                            {
                                emailBody.Recipient_CC = "m.kayali.p@tbc.sa";
                                var SMS_Response = await _tdRepo.CallNotificationAPI(smsBody, ApiUrls.SmsAPI);
                            }

                            var Email_Response = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                            if (inEmp.EMAIL.Contains(".p@"))
                            {
                                //Dont sent SMS to 3rd party
                            }
                            else
                            {
                                var SMS_Response = await _tdRepo.CallNotificationAPI(smsBody, ApiUrls.SmsAPI);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    Console.WriteLine("Finished Missed Sign Ins");
                    _logger.LogInformation("Missed ins ended @" + DateTime.Now.ToString());
                }
                else
                {
                    _logger.LogInformation("No Missed ins records: " + inEmps.Count());
                }
   
            }
            catch (Exception le)
            {
                Console.WriteLine("Error");
                _logger.LogInformation("Failed with exception: "+le.Message+Environment.NewLine + DateTime.Now.ToString());
            }
        }


        
    }
}
