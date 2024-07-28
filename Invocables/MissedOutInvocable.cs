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
    public  class MissedOutInvocable : IInvocable
    {
        private readonly ITimeAttendanceRepository _tdRepo;
        public  readonly IConfiguration _config;
        private readonly IActiveDirectoryRepository _adRepo;
        private readonly ILogger<MissedOutInvocable> _logger;
        public MissedOutInvocable(ITimeAttendanceRepository tdRepo, IConfiguration config, IActiveDirectoryRepository adRepo, ILogger<MissedOutInvocable> logger)
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
                _logger.LogInformation("Missed Outs Started @" + DateTime.Now.ToString());
                NotificationsAPIUrls ApiUrls = _config.GetSection("NotificationsAPIUrls").Get<NotificationsAPIUrls>();
                NotificationsSettings settings = _config.GetSection("NotificationsSettings").Get<NotificationsSettings>();
                var outEmps = await _tdRepo.GetMissingSignOutEmps();
                if (outEmps.Count() > 0)
                {
                    _logger.LogInformation("fetched: " + outEmps.Count() + " Missed out records.");
                    foreach (var outEmp in outEmps)
                    {


                        TemplateParams templateParams = new TemplateParams()
                        {
                            ArabicContent = string.Format(settings.EmailArabicContentOut, outEmp.FULLNAME),
                            EnglishContent = string.Format(settings.EmailEnglishContentOut, outEmp.FULLNAME),
                            NewTaskNotification = settings.EmailTitleEnOut,
                            NewTaskNotificationArabic = settings.EmailTitleArOut   //إاشعار نظام الحضور
                        }
                        ;
                        EmailBody emailBody = new EmailBody
                        {
                            AppRefID = 0,
                            Recipient = outEmp.EMAIL,
                            Recipient_CC = outEmp.MANAGER,
                            Subject = settings.OutSubject,
                            TemplateID = "8",
                            TemplateParams = templateParams
                        };
                        SMSBody smsBody = new SMSBody
                        {
                            AppRefID = 0,
                            Body = string.Format(settings.SmsContentOut, outEmp.FULLNAME),
                            ID = 0,
                            MobileNo = outEmp.MOBILE
                        };
                        if (outEmp.EMAIL == "m.kayali.p@tbc.sa" && settings.TestNotification.ToLower() == "test")
                        {

                            outEmp.MANAGER = "m.muqeet.p@tbc.sa";
                            emailBody.Recipient_CC = outEmp.MANAGER + ";m.kayali.p@tbc.sa";
                            var Email_Response = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                            var SMS_Response = await _tdRepo.CallNotificationAPI(smsBody, ApiUrls.SmsAPI);
                        }
                        else if (settings.TestNotification.ToLower() != "test")
                        {
                            //send notifications
                            if (outEmp.EMAIL == "m.kayali.p@tbc.sa")
                            {
                                emailBody.Recipient_CC = "m.kayali.p@tbc.sa";
                                var SMS_Response = await _tdRepo.CallNotificationAPI(smsBody, ApiUrls.SmsAPI);
                            }
                            var Email_Response = await _tdRepo.CallNotificationAPI(emailBody, ApiUrls.EmailAPI);
                            if (outEmp.EMAIL.Contains(".p@"))
                            {
                                Console.WriteLine("");
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
                    Console.WriteLine("Finished Missed Outs ");
                    _logger.LogInformation("Missed Outs ended @" + DateTime.Now.ToString());
                }
                else
                {
                    Console.WriteLine("No Out Records. ");
                    _logger.LogInformation("No out Records Found. @" + DateTime.Now.ToString());
                }

            }
            catch (Exception le)
            {
                Console.WriteLine("Error");
                _logger.LogInformation(le.Message + Environment.NewLine + DateTime.Now.ToString());
            }
        }
    }
}
