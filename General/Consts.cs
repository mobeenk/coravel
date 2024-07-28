using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceNotifications.General
{
    public static class Consts
    {
        #region English

        public static string EnContent =
            "<tr><td colspan=\"2\">" +
            "<p style=\"margin-top:4px;margin-bottom:4px;font-size:13px;font-weight:normal;font-family:\'Tajawal\',sans-serif;padding:20px10px20px0px;text-align:justify\">" +
                "{0}</p>" +
            "</td>" +
            "</tr>" +
            "<tr>" +
            "<td colspan=\"2\"><table style=\"border-collapse:collapse;border:1pxsolid#ccc;width:100%;font-family:\'Tajawal\',Helvetica,Arial,sans-serif;\"cellspacing=\"0\"cellpadding=\"0\"class=\"table-bordered\">" +
            "<thead>" +
                "<tr>" +
                    "<td>Date</td>" +
                    "<td title=\"First registered Check In time\">CheckIn</td>" +
                    "<td title=\"Last registered Check Out time\">CheckOut</td>" +
                    "<td title=\"Calculated as sum of all periods in office, excluding the time was spent out of the office\">Attendance Time<br/><span class=\"small\">(Excludes time out of office)</span></td>" +
                    "<td title=\"Calculated as sum of all periods in office within 7:00 to 16:30. Maximum: 8 Hours\">Business Time<br/><span class=\"small\">(7:00 to 16:30 Max:8hrs)</span></td>" +
                    "<td title=\"The time the employee miss from office hours(from 7:00 to 16:30)\">Time out<br/><span class=\"small\">(Missed time from7:00 to 16:30)</span></td>" +
                    "<td>Late In</td>" +
                    "<td>Early Out</td>" +
                "</tr>" +
            "</thead>" +
        "<tbody>"
     ;

        public static string EnSummary =
        "<tr style=\"font-weight:bold\">" +
            "<td colspan=\"3\">Total Summary</td>" +
            "<td>{0}</td>" +
            "<td>{1}</td>" +
            "<td>{2}</td>" +
            "<td>{3}</td>" +
            "<td>{4}</td>" +
        "</tr>";


        #endregion

        #region Arabic
        public static string ArContent =
              "<tr><td colspan=\"2\"><p style=\"margin-top:4px;margin-bottom:4px;font-size:13px;font-weight:normal;font-family:\'Tajawal\',sans-serif;padding:20px10px20px0px;text-align:justify\">" +
                  "{0}</p>" +
              "</td>" +
              "</tr>" +
              "<tr>" +
              "<td colspan=\"2\"><table style=\"border-collapse:collapse;border:1pxsolid#ccc;width:100%;font-family:\'Tajawal\',Helvetica,Arial,sans-serif;\"cellspacing=\"0\"cellpadding=\"0\"class=\"table-bordered\">" +
                      "<thead>" +
                          "<tr>" +
                              "<td title=\"الوقت الذي غادر فيه الموظف مبكرًا من المكتب قبل إكمال 8 ساعات\">خروج مبكر</td>" +
                              "<td title=\"الوقت الذي تأخر فيه الموظف عن الحضور إلى المكتب بعد الساعة 8:30\">التاأخير</td>" +
                              "<td title=\"الوقت الذي يغيبه الموظف من الدوام الرسمي (7:00 حتي 16:30)\">الوقت خارج المكتب<br/><span class=\"small\">( الوقت خارج المكتب من 7:00 إلى 16:30)</span></td>" +
                              "<td title=\"مجموع  فترات الدوام في غضون 7:00 إلى 16:30. الحد الأقصى: 8 ساعات\">ساعات العمل الرسمية<br/><span class=\"small\">(من 7:00 إلى 16:30 الحد الأقصى 8 ساعات)</span></td>" +
                              "<td title=\"مجموع  فترات الدوام، باستثناء الوقت الذي تم قضاؤه خارج المكتب\">ساعات العمل الكلبة<br/><span class=\"small\">(باستثناء الوقت خارج المكتب)</span></td>" +
                              "<td title=\"وقت تسجيل الخروج الأخير\">الخروج<br/></td>" +
                              "<td title=\"وقت تسجيل الدخول الأول\">الدخول</td>" +
                              "<td>التاريخ</td>" +
                          "</tr>" +
                      "</thead>" +
                  "<tbody>"
            ;
        public static string ArSummary =
        "<tr style=\"font-weight:bold\">" +
            "<td>{0}</td>" +
            "<td>{1}</td>" +
            "<td>{2}</td>" +
            "<td>{3}</td>" +
            "<td>{4}</td>" +
            "<td colspan=\"3\">ملخص الدوام</td>" +
        "</tr>";

        #endregion


        public static string Row =
        "<tr>" +
            "<td>{0}</td>" +
            "<td>{1}</td>" +
            "<td>{2}</td>" +
            "<td>{3}</td>" +
            "<td>{4}</td>" +
            "<td>{5}</td>" +
            "<td>{6}</td>" +
            "<td>{7}</td>" +
        "</tr>";
        public static string CloseTags =
                    "</tbody>" +
                "</table>" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td style=\"height:30px;\"colspan=\"2\"/>" +
        "</tr>";


    }
}
