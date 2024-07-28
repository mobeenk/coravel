using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceNotifications.Models
{
    public class SMSBody
    {
        public int AppRefID { get; set; }
        public int ID { get; set; }
        public string MobileNo { get; set; }
        public string Body { get; set; }
    }
}
