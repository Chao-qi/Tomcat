using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomcat
{
    class global
    {
        public static ModbusTCP tcpPLC;
        public static TimeSpan startTime, endTime, cycleTime;//开始时间，结束时间
        public static string alarmString = "0000000000000000";
        public static string CountString = "0";
        public static string StateString = "0000000000000000";
        public static string CylinderString = "0000000000000000";
        public static bool adm;
    }
}
