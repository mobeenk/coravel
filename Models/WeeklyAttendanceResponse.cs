using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceNotifications.Models
{
    public class WeeklyAttendanceResponse
    {
        public int EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string CheckIN { get; set; }
        public string CheckOUT { get; set; }
        public string Status { get; set; }
        public string ActualTime { get; set; } //official hours =  MissedProductivityTime + ProductivityTime
        public string WorkHours { get; set; } 
        //public string LateIN { get; set; }
        public string EarlyOut { get; set; }
        public string TotalLate { get; set; }
        public string OverTime { get; set; }
        public string DepartmentName { get; set; }
        public int ExcuseID { get; set; }
        public string Excuseinfo { get; set; }
        public string ExecuseTime { get; set; }
        public string Vacinfo { get; set; }
        public string TransDate { get; set; }
        public string ActualTimeInOffice { get; set; } //actual working hours 
        public string MissedProductivityTime { get; set; }//missed time in office
        public string ProductivityTime { get; set; } // time in office
    }

    public class SummaryData
    {
        public string TotalOfficialWH { get; set; }
        public string TotalActualWH { get; set; }
        public string TotalMissedTime { get; set; }
        public string TotalLateIn { get; set; }
        public string TotalEarlyOut { get; set; }
    }
}
