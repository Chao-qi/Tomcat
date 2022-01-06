using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Tomcat
{
    public partial class Manual : Form
    {
        SerialPort serialPort = new SerialPort();
        private string SevorstateO;
        private string SevorstateP;
        private string SevorstateN;
        private string[] CylinderState = new string[16];
        public Manual()
        {
            InitializeComponent();
        }

        private void Init_Port_Confs()     //串口界面参数设置
        {
            //RegistryKey Com = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
            comboBox1.Items.Clear();
            string[] str = SerialPort.GetPortNames();   //检查是否含有串口
            if (str == null )
            {
                comboBox1.Text = "";
                //string[] str = Com.GetValueNames();
                //comboBox1.Items.Clear();
                 MessageBox.Show("本机没有串口！", "Error");
                return;
                }
                foreach (string s in str)      //添加串口
                {
                    //string sValue = (string)Com.GetValue(s);
                    //comboBox1.Items.Add(sValue);
                    comboBox1.Items.Add(s);
                }
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.SelectedIndex = 0;      //设置默认串口选项            
                }
                ASCIIradioButton1.Checked = true;
            }
        

        private void Manual_Load(object sender, EventArgs e)
        {
            Init_Port_Confs();
            Control.CheckForIllegalCrossThreadCalls = false;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);
            serialPort.DtrEnable = true;        //准备就绪
            serialPort.RtsEnable = true;
            serialPort.ReadTimeout = 1000;      //设置数据读取超时为1秒
            serialPort.Close();
            序列号textBox.Text = "C0000001";
        }
        private void dataReceived(object sender, SerialDataReceivedEventArgs e)    //串口数据接收
        {
            if (serialPort.IsOpen)
            {
                //DateTime dataTimeNow = DateTime.Now;
                //textBox1.Text += string.Format("{0}\r\n", dataTimeNow);
                回复textBox.ForeColor = Color.Blue;
                System.Threading.Thread.Sleep(5);
                Byte[] receivedData = new Byte[serialPort.BytesToRead];
                serialPort.Read(receivedData, 0, receivedData.Length);
                serialPort.DiscardInBuffer();
                // string strRcvh = null;
                string strRcva = null;
                for (int i = 0; i < receivedData.Length; i++)
                {
                    strRcva += receivedData[i].ToString("X2");
                    strRcva += " ";
                }
                if (HEXradioButton1.Checked == true)
                {
                    回复textBox.Text += strRcva + "\r\n";
                }
                else
                {
                    /* byte[] buff = new byte[strRcva.Length / 2];
                     int index = 0;
                     for (int i = 0; i < strRcva.Length; i += 2)
                     {
                         buff[index] = Convert.ToByte(strRcva.Substring(i, 2), 16);
                         ++index;
                     }*/
                    string result = Encoding.Default.GetString(receivedData);
                    回复textBox.Text += result + "\r\n";
                }
            }
            else
            {
                MessageBox.Show("请打开某个串口", "错误提示");
            }
        }
        private void 打开串口button_Click(object sender, EventArgs e)               //打开串口
        {
            if (!serialPort.IsOpen) //串口处于关闭状态
            {
                try
                {
                    if (comboBox1.SelectedIndex == -1)
                    {
                        MessageBox.Show("Error: 无效的端口,请重新选择", "Error");
                        return;
                    }
                    string strSerialName = comboBox1.SelectedItem.ToString();
                    serialPort.PortName = strSerialName;    //串口号
                    serialPort.BaudRate = 115200;           //波特率
                    serialPort.DataBits = 8;                //数据位
                    serialPort.StopBits = StopBits.One;     //停止位
                    serialPort.Parity = Parity.None;        //校验位                   
                    serialPort.Open();                      //打开串口                   
                    打开串口button.Text = "关闭串口";        //打开串口后设置将不再有效
                    打开串口button.BackColor = Color.SpringGreen;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message, "Error");
                    return;
                }
            }
            else                                //串口处于打开状态
            {
                serialPort.Close();             //关闭串口               
                comboBox1.Enabled = true;       //串口关闭时设置有效
                打开串口button.Text = "打开串口";
                打开串口button.BackColor = Color.Transparent;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)  //状态读取
        {
            #region 气缸状态读取
            当前位置textBox1.Text = global.tcpPLC.readD("500");
            当前压力textBox1.Text = global.tcpPLC.readD("502");
            global.CylinderString = global.tcpPLC.readD("10"); //气缸状态读取
            global.CylinderString = Convert.ToString(Convert.ToInt32(global.CylinderString), 2).PadLeft(16, '0');
            CylinderState[0] = global.CylinderString.Substring(15, 1);
            if (CylinderState[0] == "1")
            {
                夹爪气缸button1.BackColor = Color.Lime;
                夹爪气缸button1.Text = "夹爪气缸动位";
            }
            else
            {
                夹爪气缸button1.BackColor = Color.Transparent;
                夹爪气缸button1.Text = "夹爪气缸原位";
            }
            CylinderState[1] = global.CylinderString.Substring(14, 1);
            if (CylinderState[1] == "1")
            {
                进退气缸button1.BackColor = Color.Lime;
                进退气缸button1.Text = "进退气缸动位";
            }
            else
            {
                进退气缸button1.BackColor = Color.Transparent;
                进退气缸button1.Text = "进退气缸原位";
            }
            CylinderState[2] = global.CylinderString.Substring(13, 1);
            if (CylinderState[2] == "1")
            {
                卡扣气缸button1.BackColor = Color.Lime;
                卡扣气缸button1.Text = "卡扣气缸动位";
            }
            else
            {
                卡扣气缸button1.BackColor = Color.Transparent;
                卡扣气缸button1.Text = "卡扣气缸原位";
            }
            CylinderState[3] = global.CylinderString.Substring(12, 1);
            if (CylinderState[3] == "1")
            {
                cal气缸button4.BackColor = Color.Lime;
                cal气缸button4.Text = "cal气缸动位";
            }
            else
            {
                cal气缸button4.BackColor = Color.Transparent;
                cal气缸button4.Text = "cal气缸原位";
            }
            CylinderState[4] = global.CylinderString.Substring(11, 1);
            if (CylinderState[4] == "1")
            {
                test气缸button4.BackColor = Color.Lime;
                test气缸button4.Text = "test气缸动位";
            }
            else
            {
                test气缸button4.BackColor = Color.Transparent;
                test气缸button4.Text = "test气缸原位";
            }

            CylinderState[5] = global.CylinderString.Substring(10, 1);
            if (CylinderState[5] == "1")
            {
                旋转气缸逆时针button3.BackColor = Color.Lime;
                旋转气缸逆时针button3.Text = "旋转气缸T位";
            }
            else
            {
                旋转气缸逆时针button3.BackColor = Color.Transparent;
                旋转气缸逆时针button3.Text = "旋转气缸T位";
            }
            CylinderState[6] = global.CylinderString.Substring(9, 1);
            if (CylinderState[6] == "1")
            {
                旋转气缸顺时针button1.BackColor = Color.Lime;
                旋转气缸顺时针button1.Text = "旋转气缸C位";
            }
            else
            {
                旋转气缸逆时针button3.BackColor = Color.Transparent;
                旋转气缸逆时针button3.Text = "旋转气缸C位";
            }
            CylinderState[7] = global.CylinderString.Substring(8, 1);
            if (CylinderState[7] == "1")
            {
                旋转气缸原位button2.BackColor = Color.Lime;
                旋转气缸原位button2.Text = "旋转气缸原位";
            }
            else
            {
                旋转气缸逆时针button3.BackColor = Color.Transparent;
                旋转气缸逆时针button3.Text = "旋转气缸原位";
            }
            #endregion

            #region 伺服状态读取
            SevorstateO = global.tcpPLC.readM("8");
            if (SevorstateO == "1")
            {
                回原点button1.Text = "原点位";
                回原点button1.BackColor = Color.Lime;
            }
            else
            {
                回原点button1.Text = "回原点";
                回原点button1.BackColor = Color.Transparent;
            }
            SevorstateP = global.tcpPLC.readM("9");
            if (SevorstateP == "1")
            {
                正极限button.Text = "保护中";
                正极限button.BackColor = Color.Lime;
            }
            else
            {
                正极限button.Text = "触碰中";
                正极限button.BackColor = Color.Red;
            }
            SevorstateN = global.tcpPLC.readM("10");
            if (SevorstateN == "1")
            {
                负极限button.Text = "保护中";
                负极限button.BackColor = Color.Lime;
            }
            else
            {
                负极限button.Text = "触碰中";
                负极限button.BackColor = Color.Red;
            }
            #endregion
        }

        #region 设备操作
        private void 下降button3_MouseDown(object sender, MouseEventArgs e)
        {
            global.tcpPLC.SetM("50");
            下降button3.BackColor = Color.Lime;
        }
        private void 下降button3_MouseUp(object sender, MouseEventArgs e)
        {
            global.tcpPLC.RstM("50");
            下降button3.BackColor = Color.Transparent;
        }
        private void 上升button2_MouseDown(object sender, MouseEventArgs e)
        {
            global.tcpPLC.SetM("51");
            上升button2.BackColor = Color.Lime;
        }
        private void 上升button2_MouseUp(object sender, MouseEventArgs e)
        {
            global.tcpPLC.RstM("51");
            上升button2.BackColor = Color.Transparent;
        }
        private void 回原点button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (复位button.Text == "回原点")
            {
                global.tcpPLC.SetM("52");
            }         
        }
        private void 夹爪气缸button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (夹爪气缸button1.BackColor == Color.Transparent)
            {
                global.tcpPLC.SetM("53");
            }
            else if (夹爪气缸button1.BackColor == Color.Lime)
            {
                global.tcpPLC.RstM("53");
            }
        }
        private void 进退气缸button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (进退气缸button1.BackColor == Color.Transparent)
            {
                global.tcpPLC.SetM("54");
            }
            else if (进退气缸button1.BackColor == Color.Lime)
            {
                global.tcpPLC.RstM("54");
            }
        }
        private void 卡扣气缸button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (卡扣气缸button1.BackColor == Color.Transparent)
            {
                global.tcpPLC.SetM("55");
            }
            else if (卡扣气缸button1.BackColor == Color.Lime)
            {
                global.tcpPLC.RstM("55");
            }
        }
        private void cal气缸button4_MouseUp(object sender, MouseEventArgs e)
        {
            if (cal气缸button4.BackColor == Color.Transparent)
            {
                global.tcpPLC.SetM("56");
            }
            else if (cal气缸button4.BackColor == Color.Lime)
            {
                global.tcpPLC.RstM("56");
            }
        }
        private void test气缸button4_MouseUp(object sender, MouseEventArgs e)
        {
            if (test气缸button4.BackColor == Color.Transparent)
            {
                global.tcpPLC.SetM("57");
            }
            else if (test气缸button4.BackColor == Color.Lime)
            {
                global.tcpPLC.RstM("57");
            }
        }
        private void 旋转气缸逆时针button3_MouseUp(object sender, MouseEventArgs e)
        {
            if (旋转气缸逆时针button3.BackColor == Color.Transparent)
            {
                global.tcpPLC.SetM("58");
            }
            else if (旋转气缸逆时针button3.BackColor == Color.Lime)
            {
                global.tcpPLC.RstM("58");
            }
        }
        private void 旋转气缸原位button2_MouseUp(object sender, MouseEventArgs e)
        {
            if (旋转气缸原位button2.BackColor == Color.Transparent)
            {
                global.tcpPLC.SetM("59");
            }
            else if (旋转气缸原位button2.BackColor == Color.Lime)
            {
                global.tcpPLC.RstM("59");
            }
        }
        private void 旋转气缸顺时针button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (旋转气缸顺时针button1.BackColor == Color.Transparent)
            {
                global.tcpPLC.SetM("60");
            }
            else if (旋转气缸顺时针button1.BackColor == Color.Lime)
            {
                global.tcpPLC.RstM("60");
            }
        }


        #endregion

        #region 串口数据发送
        private void 夹爪气缸button_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("请先打开串口", "Error");
                return;
            }
            else if (夹爪气缸button.BackColor == Color.Lime)
            {
                String strsend = "$C.1:dut.connect";        //发送数据
                serialPort.WriteLine(strsend);              //发送一行数据
            }
            else if (夹爪气缸button.BackColor == Color.Transparent)
            {
                String strsend = "$C.1:dut.disconnect";     //发送数据
                serialPort.WriteLine(strsend);              //发送一行数据
            }          
        }

        private void 开卡扣button_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("请先打开串口", "Error");
                return;
            }
            else if (开卡扣button.BackColor == Color.Lime)
            {
                String strsend = "$C.1:carrier.connect";        //发送数据
                serialPort.WriteLine(strsend);              //发送一行数据
            }
            else if (开卡扣button.BackColor == Color.Transparent)
            {
                String strsend = "$C.1:carrier.disconnect";     //发送数据
                serialPort.WriteLine(strsend);              //发送一行数据
            }
        }

        private void CALbutton_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("请先打开串口", "Error");
                return;
            }
            String strsend = "$C.1:stimpad.rotate;CAL";  //发送数据
            serialPort.WriteLine(strsend);  //发送一行数据
        }

        private void MIDbutton_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("请先打开串口", "Error");
                return;
            }
            String strsend = "$C.1:stimpad.rotate;MID";  //发送数据
            serialPort.WriteLine(strsend);  //发送一行数据
        }

        private void TESTbutton_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("请先打开串口", "Error");
                return;
            }
            String strsend = "$C.1:stimpad.rotate;TEST";    //发送数据
            serialPort.WriteLine(strsend);                  //发送一行数据
        }

        private void 下压button_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("请先打开串口", "Error");
                return;
            }
            else if (下压button.BackColor == Color.Lime)
            {
                String strsend = "$C.1:stmpad.release";              //发送数据
                serialPort.WriteLine(strsend);                       //发送一行数据
            }
            else if (下压button.BackColor == Color.Transparent)
            {
                String strsend = "$C.1:stimpad.press_slow;2.4";     //发送数据
                serialPort.WriteLine(strsend);                      //发送一行数据
            }
        }

        private void 复位button_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("请先打开串口", "Error");
                return;
            }
            String strsend = "$C.1:reset";    //发送数据
            serialPort.WriteLine(strsend);    //发送一行数据
        }

        private void 序列号写入button_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("请先打开串口", "Error");
                return;
            }
            else if (序列号写入button.BackColor == Color.Transparent)
            {
                string SNstr = "$C.1:serial_number.write;" + 序列号textBox.Text;
                String strsend = SNstr;
                serialPort.WriteLine(strsend);
                序列号写入button.Text = "序列号读取";
                序列号写入button.BackColor = Color.Lime;
            }
            else if (序列号写入button.BackColor == Color.Lime)
            {
                String strsend = "$C.1:serial_number.read";
                serialPort.WriteLine(strsend);
                序列号写入button.Text = "序列号写入";
                序列号写入button.BackColor = Color.Transparent;
            }
        }

        private void 版本读取button_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("请先打开串口", "Error");
                return;
            }
            String strsend = "$C.1:fw_version.read";    //发送数据
            serialPort.WriteLine(strsend);    //发送一行数据
        }

        private void 清空回复button_Click(object sender, EventArgs e)
        {
            回复textBox.Text = "";
        }
        #endregion

        private void 保存文件button_Click(object sender, EventArgs e)  //保存文件
        {
            StreamWriter myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                myStream = new StreamWriter(saveFileDialog1.FileName);

                myStream.Write(回复textBox.Text); //写入

                myStream.Close();//关闭流
            }
        }

        private void 刷新串口button_Click(object sender, EventArgs e)  //刷新串口
        {
            Init_Port_Confs();
        }
    }
}
