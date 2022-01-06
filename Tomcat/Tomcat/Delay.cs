using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tomcat
{
    class Delay
    {
        public static void delay(uint ms)
        {
            int dwStart = System.Environment.TickCount;
            while (System.Environment.TickCount - dwStart < ms)
            {
                Application.DoEvents();
            }
        }
    }
}
