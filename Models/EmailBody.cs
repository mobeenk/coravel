using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceNotifications.Models
{
    public class EmailBody
    {
        public int AppRefID { get; set; }
        public string Recipient { get; set; }
        public string Recipient_CC { get; set; }
        public string Recipient_BCC { get; set; }
        public string Subject { get; set; }
        public string TemplateID { get; set; }
        public TemplateParams TemplateParams { get; set; }
    }

    public class TemplateParams
    {
        public string NewTaskNotification { get; set; }
        public string NewTaskNotificationArabic { get; set; }
        public string ArabicContent { get; set; }
        public string EnglishContent { get; set; }
       // public string PortalURL { get; set; }
    }

    public class EmailBodyWeeklyAtt
    {
        public int AppRefID { get; set; }
        public string Recipient { get; set; }
        public string Recipient_CC { get; set; }
        public string Recipient_BCC { get; set; }
        public string Subject { get; set; }
        public string TemplateID { get; set; }
        public Dictionary<string,string> TemplateParams { get; set; }
    }

}
