using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceNotifications.Models
{
    public class InEmps
    {
        public int? ROWID { get; set; }
        public DateTime LOGINTIME { get; set; }
        public string EMPNO { get; set; }
        public string EMAIL { get; set; }
        public string MANAGER { get; set; }
        public string FULLNAME { get; set; }
        public string MOBILE { get; set; }

        
    }
    public class OutEmps
    {
        public int? ROWID { get; set; }
        public DateTime LOGOUTTIME { get; set; }
        public string EMPNO { get; set; }
        public string EMAIL { get; set; }
        public string MANAGER { get; set; }
        public string FULLNAME { get; set; }
        public string MOBILE { get; set; }


    }
}
