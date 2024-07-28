using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceNotifications.Models
{
    public class NotificationsSettings
    {
        public string EmailArabicContentIn { get; set; }
        public string EmailEnglishContentIn { get; set; }
        public string EmailTitleEnIn { get; set; }
        public string EmailTitleArIn { get; set; }
        public string EmailSubjectIn { get; set; }
        public string EmailArabicContentOut { get; set; }
        public string EmailEnglishContentOut { get; set; }
        public string EmailTitleEnOut { get; set; }
        public string EmailTitleArOut { get; set; }
        public string EmailSubjectOut { get; set; }
        public string SmsContentIn { get; set; }
        public string SmsContentOut { get; set; }
        //time
        public string MissedInHours { get; set; }
        public string MissedInMinutes { get; set; }
        public string MissedOutHours { get; set; }
        public string MissedOutMinutes { get; set; }
        public string WeeklyReportHours { get; set; }
        public string WeeklyReportMinutes { get; set; }

        public string WeeklyManagerHours { get; set; }
        public string WeeklyManagerMinutes { get; set; }

        public string DailyReportHours { get; set; }
        public string DailyReportMinutes { get; set; }

        //weekly report
        public string WeeklyReportAR { get; set; }
        public string WeeklyReportEN { get; set; }
        public string WeeklyReportTitleAR { get; set; }
        public string WeeklyReportTitleEN { get; set; }

        public string TestNotification { get; set; }

        public string InSubject { get; set; }
        public string OutSubject { get; set; }
        public string WeeklySubject { get; set; }

        public string DailySubject { get; set; }
        public string DailyArabicContent { get; set; }
        public string DailyEnglishContent { get; set; }

        public string TestCC { get; set; }
        //Manager Invocable
        public string ManagerSubject { get; set; }
        public string ManagerText { get; set; }

    }
}
