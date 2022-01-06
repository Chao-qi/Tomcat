using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tomcat
{
    public partial class Main : Form
    {
        //private Alarm almfrm = null;
        Alarm almfrm = new Alarm();
        private int TimeCounts;
        private int TimeCountm;
        private int TomcatState;
        private bool almflag = true;
        private string[] machineState = new string[16];
        //private double intNGCounts = 0;
        //private double intTotalCounts = 0;
        private string[] oldState = new string[16];
        private double[] ccdData = new double[100];
        private double[] Weight = new double[100];
        private double[] FinalWt = new double[100];
        public Main()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        private void Main_Load(object sender, EventArgs e)
        {           
            global.tcpPLC = new ModbusTCP();
            if (!global.tcpPLC.tcpConnect("192.168.1.1"))
            {
                timer1.Enabled = false;            
                timer2.Enabled = false;
                设备状态textBox1.Text = "连接断开";
                设备状态textBox1.BackColor = Color.Red;
                MessageBox.Show("PLC连接错误!");
            }
            测试时间textBox1.Text = "0.0";           
        }
        private void 登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            login admin = new login();
            admin.ShowDialog();
        }
        private void 参数设置toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Parameter tl = new Parameter();
            tl.ShowDialog();
        }
        private void 手动toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Manual manualfrm = new Manual();
            manualfrm.ShowDialog();
        }

        private void 计数清除toolStripMenuItem3_Click(object sender, EventArgs e)
        {

            测试数量textBox1.Text = "0";
            global.tcpPLC.writeD("564", 测试数量textBox1.Text);
        }

        private void 退出toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DialogResult dr;
            dr = MessageBox.Show(this, "要退出当前界面吗？", "是否退出？", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            switch (dr)
            {
                case System.Windows.Forms.DialogResult.Yes://保存修改    
                    this.Close();
                    break;
                case System.Windows.Forms.DialogResult.No:
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)   //界面登录
        {        
                if (global.adm)
            {
                参数设置toolStripMenuItem1.Visible = true;
                手动toolStripMenuItem2.Visible = true;
                计数清除toolStripMenuItem3.Visible = true;
            }
            else
            {
                参数设置toolStripMenuItem1.Visible = false;
                手动toolStripMenuItem2.Visible = false;
                计数清除toolStripMenuItem3.Visible = false;
            }
        }
        private void timer2_Tick(object sender, EventArgs e) 
        {

            #region 报警触发
            //Alarm almfrm = new Alarm();
            global.alarmString = global.tcpPLC.readD("20");
            global.alarmString = Convert.ToString(Convert.ToInt32(global.alarmString), 2).PadLeft(16, '0');
            if (global.alarmString != "0000000000000000")       //报警发生
            {
                if (almfrm.Visible == false)
                {
                    almfrm.Visible = true;    //报警框弹出          
                }
            }
            else
            {
                if (almfrm.Visible == true)
                {
                    almfrm.Visible = false;
                }
            }
            #endregion

            #region 状态读取
            TomcatState = Convert.ToInt32(global.tcpPLC.readD("1"));  //设备状态
            if (TomcatState == 0)
            {
                设备状态textBox1.Text = "手动中";
                设备状态textBox1.BackColor = Color.Yellow;
            }
            else if (TomcatState == 1)
            {
                设备状态textBox1.Text = "自动中";
                设备状态textBox1.BackColor = Color.Lime;
            }
            else 
            {
                设备状态textBox1.Text = "设备异常";
                设备状态textBox1.BackColor = Color.Red;
            }

            global.StateString = global.tcpPLC.readD("508"); //运行状态
            global.StateString = Convert.ToString(Convert.ToInt32(global.StateString), 2).PadLeft(16, '0');
            machineState[0] = global.StateString.Substring(15, 1); //光栅状态
            if (machineState[0] == "1")
            {
                光栅状态textBox2.Text = "保护中";
                光栅状态textBox2.BackColor = Color.Lime; 
            }
            else
            {
                光栅状态textBox2.Text = "触碰中";
                光栅状态textBox2.BackColor = Color.Red;
            }

            machineState[1] = global.StateString.Substring(14, 1); //急停状态
            if (machineState[1] == "1")
            {
                急停状态textBox6.Text = "保护中";
                急停状态textBox6.BackColor = Color.Lime;
            }
            else
            {
                急停状态textBox6.Text = "急停中";
                急停状态textBox6.BackColor = Color.Red;
            }

            machineState[2] = global.StateString.Substring(13, 1); //伺服状态
            if (machineState[2] == "1")
            {
                伺服状态textBox5.Text = "伺服ON";
                伺服状态textBox5.BackColor = Color.Lime;
            }
            else
            {
                伺服状态textBox5.Text = "伺服OFF";
                伺服状态textBox5.BackColor = Color.Red;
            }

            machineState[3] = global.StateString.Substring(12, 1); //夹爪气缸状态
            if (machineState[3] == "1")
            {
                夹爪气缸textBox3.Text = "气缸动位";
                夹爪气缸textBox3.BackColor = Color.Lime;
            }
            else
            {
                夹爪气缸textBox3.Text = "气缸原位";
                夹爪气缸textBox3.BackColor = Color.WhiteSmoke;
            }

            machineState[4] = global.StateString.Substring(11, 1); //旋转气缸状态
            if (machineState[4] == "1")
            {
                旋转气缸textBox4.Text = "气缸原位";
                旋转气缸textBox4.BackColor = Color.WhiteSmoke;
            }
            machineState[5] = global.StateString.Substring(10, 1);
            if (machineState[5] == "1")
            {
                旋转气缸textBox4.Text = "气缸CAL位";
                旋转气缸textBox4.BackColor = Color.Lime;
            }
            machineState[6] = global.StateString.Substring(9, 1);
            if (machineState[6] == "1")
            {
                旋转气缸textBox4.Text = "气缸TEST位";
                旋转气缸textBox4.BackColor = Color.Lime;
            }
            if (machineState[4] == "0" && machineState[5] == "0" && machineState[6] == "0")
            {
                旋转气缸textBox4.Text = "气缸异常";
                旋转气缸textBox4.BackColor = Color.Red;
            }

            machineState[7] = global.StateString.Substring(8, 1); //进退气缸状态
            if (machineState[7] == "1")
            {
                进退气缸textBox7.Text = "气缸动位";
                进退气缸textBox7.BackColor = Color.Lime;
            }
            else
            {
                进退气缸textBox7.Text = "气缸原位";
                进退气缸textBox7.BackColor = Color.WhiteSmoke;
            }

            machineState[8] = global.StateString.Substring(7, 1);//卡扣气缸状态
            if (machineState[8] == "1")
            {
                卡扣气缸textBox8.Text = "气缸动位";
                卡扣气缸textBox8.BackColor = Color.Lime;
            }
            else
            {
                卡扣气缸textBox8.Text = "气缸原位";
                卡扣气缸textBox8.BackColor = Color.WhiteSmoke;
            }

            machineState[9] = global.StateString.Substring(6, 1);//CAL气缸状态
            if (machineState[9] == "1")
            {
                CAL气缸textBox9.Text = "气缸动位";
                CAL气缸textBox9.BackColor = Color.Lime;
            }
            else
            {
                CAL气缸textBox9.Text = "气缸原位";
                CAL气缸textBox9.BackColor = Color.WhiteSmoke;
            }

            machineState[10] = global.StateString.Substring(5, 1);//TEST气缸状态
            if (machineState[10] == "1")
            {
                Test气缸textBox10.Text = "气缸动位";
                Test气缸textBox10.BackColor = Color.Lime;
            }
            else
            {
                Test气缸textBox10.Text = "气缸原位";
                Test气缸textBox10.BackColor = Color.WhiteSmoke;
            }

            machineState[11] = global.StateString.Substring(4, 1);//伺服位置感应
            if (machineState[11] == "1")
            {
                伺服上极限textBox.Text = "保护中";
                伺服上极限textBox.BackColor = Color.Lime;
            }
            else
            {
                伺服上极限textBox.Text = "触碰中";
                伺服上极限textBox.BackColor = Color.Red;
            }

            machineState[12] = global.StateString.Substring(3, 1);//伺服位置感应
            if (machineState[12] == "1")
            {
                伺服下极限textBox.Text = "保护中";
                伺服下极限textBox.BackColor = Color.Lime;
            }
            else
            {
                伺服下极限textBox.Text = "触碰中";
                伺服下极限textBox.BackColor = Color.Red;
            }

            #endregion

            #region 参数读取
            伺服位置textBox3.Text = global.tcpPLC.readD("500");
            当前压力textBox2.Text = global.tcpPLC.readD("502");
            测试数量textBox1.Text = global.tcpPLC.readD("504");
            
            global.CountString = global.tcpPLC.readM("506");       //测试时间   
            if (global.CountString == "1")
            {
                timer3.Enabled = true;
            }
            else
            {
                timer3.Enabled = false;
                TimeCounts = 0;
                TimeCountm = 0;
            }
            #endregion  
        }

        private void timer3_Tick(object sender, EventArgs e)   //测试时间
        {           
            TimeCounts = TimeCounts + 1;
            if (TimeCounts > 9)
            {
                TimeCountm = TimeCountm + 1;
                TimeCounts = 0;
            }        
            测试时间textBox1.Text = (TimeCountm + "."+ TimeCounts).ToString();
        }       
    }
}
