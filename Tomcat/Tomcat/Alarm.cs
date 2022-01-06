using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tomcat
{
    public partial class Alarm : Form
    {
        string[] oldState = new string[16];
        public Alarm()
        {
            InitializeComponent();
        }
        public void almAdd(string almMsg)
        {
            listBox1.Items.Add(almMsg);
        }
        public void almRemove(string almMsg)
        {
            listBox1.Items.Remove(almMsg);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] newState = new string[16];
            newState[0] = global.alarmString.Substring(15, 1);
            if (oldState[0] != newState[0])
            {
                oldState[0] = newState[0];
                if (newState[0] == "1")
                {
                    this.almAdd("紧急停止");
                }
                else
                {
                    this.almRemove("紧急停止");
                }
            }
            newState[1] = global.alarmString.Substring(14, 1);
            if (oldState[1] != newState[1])
            {
                oldState[1] = newState[1];
                if (newState[1] == "1")
                {
                    this.almAdd("光栅报警");
                }
                else
                {
                    this.almRemove("光栅报警");
                }
            }
            newState[2] = global.alarmString.Substring(13, 1);
            if (oldState[2] != newState[2])
            {
                oldState[2] = newState[2];
                if (newState[2] == "1")
                {
                    this.almAdd("伺服报警");
                }
                else
                {
                    this.almRemove("伺服报警");
                }
            }
            newState[3] = global.alarmString.Substring(12, 1);
            if (oldState[3] != newState[3])
            {
                oldState[3] = newState[3];
                if (newState[3] == "1")
                {
                    this.almAdd("旋转气缸顺时针报警");
                }
                else
                {
                    this.almRemove("旋转气缸顺时针报警");
                }
            }
            newState[4] = global.alarmString.Substring(11, 1);
            if (oldState[4] != newState[4])
            {
                oldState[4] = newState[4];
                if (newState[4] == "1")
                {
                    this.almAdd("气缸逆时针报警");
                }
                else
                {
                    this.almRemove("气缸逆时针报警");
                }
            }
            newState[5] = global.alarmString.Substring(10, 1);
            if (oldState[5] != newState[5])
            {
                oldState[5] = newState[5];
                if (newState[5] == "1")
                {
                    this.almAdd("夹爪气缸上报警");
                }
                else
                {
                    this.almRemove("夹爪气缸上报警");
                }
            }
            newState[6] = global.alarmString.Substring(9, 1);
            if (oldState[6] != newState[6])
            {
                oldState[6] = newState[6];
                if (newState[6] == "1")
                {
                    this.almAdd("夹爪气缸下报警");
                }
                else
                {
                    this.almRemove("夹爪气缸下报警");
                }
            }
        }
    }
}
