// Decompiled with JetBrains decompiler
// Type: MiniIMU.Form1
// Assembly: MiniIMU, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B3D3629B-0FCE-40C9-9203-896A37FD5DA0
// Assembly location: C:\Users\Tim White\Desktop\JY-61 PC program\MiniIMU.exe

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MiniIMU
{
  public class Form1 : Form
  {
    private Meter formMeter = new Meter();
    private Control3D Form3D = new Control3D();
    private ResourceManager rm = new ResourceManager(typeof (Form1));
    private DateTime TimeStart = DateTime.Now;
    private int Baund = 115200;
    private double[] a = new double[3];
    private double[] w = new double[3];
    private double[] Angle = new double[3];
    private double[] LastTime = new double[3];
    private byte[] RxBuffer = new byte[1000];
    private FileStream fsAccelerate = new FileStream("Acceleration.txt", FileMode.Create);
    private FileStream fsGyro = new FileStream("AngleVelocity.txt", FileMode.Create);
    private FileStream fsAngle = new FileStream("Angle.txt", FileMode.Create);
    private bool bListening;
    private bool bClosing;
    private double Temperature;
    private ushort usRxLength;
    private StreamWriter swAccelerate;
    private StreamWriter swGyro;
    private StreamWriter swAngle;
    private bool bForm3DShow;
    private IContainer components;
    private ToolStrip toolStrip1;
    private ToolStripDropDownButton toolStripComSet;
    private ToolStripButton toolStripButtonAngleInitial;
    private ToolStripButton toolStripButton3;
    private TabControl tabControlChart;
    private Chart chart2;
    private TabPage tabPage2;
    private Chart chart3;
    private SerialPort spSerialPort;
    private System.Windows.Forms.Timer timer1;
    private Panel panel1;
    private SplitContainer splitContainer1;
    private TextBox textBoxTextInfo;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel Status;
    private ToolStripButton toolStripButton1;
    private TabPage tabPage3;
    private TabPage tabPage1;
    private ToolStripDropDownButton toolStripDropDownButton1;
    private ToolStripMenuItem toolStripMenuItem2;
    private ToolStripMenuItem toolStripMenuItem3;
    private SplitContainer splitContainer2;
    private PictureBox pictureBox1;
    private LinkLabel linkLabel1;
    private SplitContainer splitContainer3;
    private TabPage tabPage4;
    private GroupBox groupBox1;
    private TextBox textBox3;
    private Label label9;
    private Label label2;
    private TextBox textBox2;
    private Label label1;
    private Button button2;
    private GroupBox groupBox4;
    private TextBox textBox8;
    private Label label12;
    private Label label7;
    private TextBox textBox9;
    private Label label8;
    private GroupBox groupBox3;
    private TextBox textBox6;
    private Label label11;
    private Label label5;
    private TextBox textBox7;
    private Label label6;
    private GroupBox groupBox2;
    private TextBox textBox4;
    private Label label10;
    private Label label3;
    private TextBox textBox5;
    private Label label4;
    private ToolStripDropDownButton toolStripDropDownButton2;
    private ToolStripMenuItem 串口模式ToolStripMenuItem;
    private ToolStripMenuItem iIC模式ToolStripMenuItem;
    private ToolStripDropDownButton toolStripDropDownButton3;
    private ToolStripMenuItem ToolStripMenuItemChinese;
    private ToolStripMenuItem ToolStripMenuItemEnglish;
    private SplitContainer splitContainer4;
    private Chart chart1;
    private ToolStripButton toolStripButton2;
    private ToolStripButton toolStripButton4;
    private ToolStripButton toolStripButton5;
    private ToolStripButton toolStripButton6;
    private ToolStripDropDownButton toolStripDropDownButton4;
    private ToolStripMenuItem sToolStripMenuItem;
    private ToolStripMenuItem sToolStripMenuItem1;
    private ToolStripMenuItem sToolStripMenuItem2;
    private ToolStripMenuItem sToolStripMenuItem3;
    private ToolStripMenuItem sToolStripMenuItem4;
    private ToolStripMenuItem sToolStripMenuItem5;
    private ToolStripMenuItem sToolStripMenuItem6;
    private ToolStripMenuItem sToolStripMenuItem7;
    private ToolStripMenuItem sToolStripMenuItem8;
    private ToolStripMenuItem sToolStripMenuItem9;
    private ToolStripMenuItem sToolStripMenuItem10;
    private ToolStripMenuItem sToolStripMenuItem11;
    private ToolStripMenuItem sToolStripMenuItem12;
    private ToolStripMenuItem sToolStripMenuItem13;
    private ToolStripMenuItem sToolStripMenuItem14;

    public Form1()
    {
      this.InitializeComponent();
      this.IsMdiContainer = true;
      this.formMeter.MdiParent = (Form) this;
      this.formMeter.Parent = (Control) this.splitContainer4.Panel2;
      this.formMeter.Dock = DockStyle.Fill;
      this.formMeter.Show();
    }

    private void RefreshComPort(object sender, EventArgs e)
    {
      this.toolStripComSet.DropDownItems.Clear();
      foreach (string portName in SerialPort.GetPortNames())
      {
        this.toolStripComSet.DropDownItems.Add(portName, (Image) null, new EventHandler(this.PortSelect));
        if (this.spSerialPort.IsOpen & this.spSerialPort.PortName == portName)
          ((ToolStripMenuItem) this.toolStripComSet.DropDownItems[this.toolStripComSet.DropDownItems.Count - 1]).Checked = true;
      }
      this.toolStripComSet.DropDownItems.Add((ToolStripItem) new ToolStripSeparator());
      this.toolStripComSet.DropDownItems.Add("Close", (Image) null, new EventHandler(this.PortClose));
    }

    private void languageMenuEnglish_Click(object sender, EventArgs e)
    {
      this.ToolStripMenuItemChinese.Checked = false;
      this.ToolStripMenuItemEnglish.Checked = true;
      Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
      this.rm = new ResourceManager(typeof (Form1));
      this.UpDataMainFormMenuLanguage();
    }

    private void languageMenuSimlpeChinese_Click(object sender, EventArgs e)
    {
      this.ToolStripMenuItemChinese.Checked = true;
      this.ToolStripMenuItemEnglish.Checked = false;
      Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CHS");
      this.rm = new ResourceManager(typeof (Form1));
      this.UpDataMainFormMenuLanguage();
    }

    private void UpDataMainFormMenuLanguage()
    {
      this.Text = "MiniIMU http://www.aliexpress.com/store/1836321";
      this.toolStripComSet.Text = this.rm.GetString("PortSet");
      this.toolStripDropDownButton1.Text = this.rm.GetString("Baund");
      this.toolStripDropDownButton2.Text = this.rm.GetString("Mode");
      this.串口模式ToolStripMenuItem.Text = this.rm.GetString("USART");
      this.toolStripButtonAngleInitial.Text = this.rm.GetString("AngleInitial");
      this.toolStripButton3.Text = this.rm.GetString("Record");
      this.toolStripButton2.Text = this.rm.GetString("Clear");
      this.toolStripButton4.Text = this.rm.GetString("ThreeD");
      this.toolStripButton5.Text = this.rm.GetString("BBS");
      this.toolStripButton6.Text = this.rm.GetString("Mall");
      this.toolStripDropDownButton4.Text = this.rm.GetString("Zero");
      this.toolStripDropDownButton3.Text = this.rm.GetString("Language");
      this.ToolStripMenuItemChinese.Text = this.rm.GetString("Chinese");
      this.ToolStripMenuItemEnglish.Text = this.rm.GetString("English");
      this.tabControlChart.TabPages[0].Text = this.rm.GetString("Acceleration");
      this.tabControlChart.TabPages[1].Text = this.rm.GetString("AngleVelocity");
      this.tabControlChart.TabPages[2].Text = this.rm.GetString("Angle");
      this.tabControlChart.TabPages[3].Text = this.rm.GetString("Config");
      this.groupBox1.Text = this.rm.GetString("Acceleration");
      this.groupBox2.Text = this.rm.GetString("AngleVelocity");
      this.groupBox3.Text = this.rm.GetString("Angle");
      this.groupBox4.Text = this.rm.GetString("Tempreture");
      this.label1.Text = this.rm.GetString("Offset");
      this.label2.Text = this.rm.GetString("Amplitude");
      this.label4.Text = this.rm.GetString("Offset");
      this.label3.Text = this.rm.GetString("Amplitude");
      this.label6.Text = this.rm.GetString("Offset");
      this.label5.Text = this.rm.GetString("Amplitude");
      this.label8.Text = this.rm.GetString("Offset");
      this.label7.Text = this.rm.GetString("Amplitude");
      this.button2.Text = this.rm.GetString("Reset");
      this.textBoxTextInfo.Text = this.rm.GetString("TextInfo1");
      this.chart1.Titles[0].Text = this.rm.GetString("AccCurve");
      this.chart2.Titles[0].Text = this.rm.GetString("AngleVelocityCurve");
      this.chart3.Titles[0].Text = this.rm.GetString("AngleCurve");
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      this.RefreshComPort((object) null, (EventArgs) null);
      this.UpDataMainFormMenuLanguage();
      this.fsAccelerate.Close();
      this.fsGyro.Close();
      this.fsAngle.Close();
      if (!File.Exists("Config.ini"))
      {
        this.textBox2.Text = "0";
        this.textBox3.Text = "16";
        this.textBox4.Text = "0";
        this.textBox5.Text = "2000";
        this.textBox6.Text = "0";
        this.textBox7.Text = "180";
        this.textBox8.Text = "36.53";
        this.textBox9.Text = "96.38";
        this.WriteConfig();
      }
      else
      {
        FileStream fileStream = new FileStream("Config.ini", FileMode.Open);
        StreamReader streamReader = new StreamReader((Stream) fileStream);
        this.Baund = int.Parse(streamReader.ReadLine());
        if (streamReader.ReadLine() == "en")
          this.languageMenuEnglish_Click((object) null, (EventArgs) null);
        else
          this.languageMenuSimlpeChinese_Click((object) null, (EventArgs) null);
        string str = streamReader.ReadLine();
        this.textBox2.Text = streamReader.ReadLine();
        str = streamReader.ReadLine();
        this.textBox3.Text = streamReader.ReadLine();
        str = streamReader.ReadLine();
        this.textBox4.Text = streamReader.ReadLine();
        str = streamReader.ReadLine();
        this.textBox5.Text = streamReader.ReadLine();
        str = streamReader.ReadLine();
        this.textBox6.Text = streamReader.ReadLine();
        str = streamReader.ReadLine();
        this.textBox7.Text = streamReader.ReadLine();
        str = streamReader.ReadLine();
        this.textBox8.Text = streamReader.ReadLine();
        str = streamReader.ReadLine();
        this.textBox9.Text = streamReader.ReadLine();
        streamReader.Close();
        fileStream.Close();
      }
      if (this.Baund == 115200)
      {
        this.toolStripMenuItem2.Checked = true;
        this.toolStripMenuItem3.Checked = false;
      }
      else
      {
        this.toolStripMenuItem2.Checked = false;
        this.toolStripMenuItem3.Checked = true;
      }
      this.Form3D.Owner = (Form) this;
    }

    private void PortSelect(object sender, EventArgs e)
    {
      ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem) sender;
      try
      {
        this.PortClose((object) null, (EventArgs) null);
        this.spSerialPort.PortName = toolStripMenuItem.Text;
        this.spSerialPort.BaudRate = !this.toolStripMenuItem2.Checked ? 9600 : 115200;
        this.spSerialPort.Open();
        toolStripMenuItem.Checked = true;
        this.bClosing = false;
      }
      catch (Exception ex)
      {
        toolStripMenuItem.Checked = false;
      }
    }

    private void PortClose(object sender, EventArgs e)
    {
      for (int index = 0; index < this.toolStripComSet.DropDownItems.Count - 2; ++index)
        ((ToolStripMenuItem) this.toolStripComSet.DropDownItems[index]).Checked = false;
      if (!this.spSerialPort.IsOpen)
        return;
      this.bClosing = true;
      while (this.bListening)
        Application.DoEvents();
      this.spSerialPort.Dispose();
      this.spSerialPort.Close();
    }

    private void UpdateIMUData(string strType, double TimeElapse, double[] Data)
    {
      if (!(strType == "Accelerate"))
      {
        if (!(strType == "AngleVelocity"))
        {
          if (strType == "Angle")
          {
            this.Angle[0] = Data[0];
            this.Angle[1] = Data[1];
            this.Angle[2] = Data[2];
            this.formMeter.RefreshAngle(this.Angle);
          }
        }
        else
        {
          this.w[0] = Data[0];
          this.w[1] = Data[1];
          this.w[2] = Data[2];
        }
      }
      else
      {
        this.a[0] = Data[0];
        this.a[1] = Data[1];
        this.a[2] = Data[2];
      }
      this.Temperature = Data[3];
      if (this.ToolStripMenuItemChinese.Checked)
        this.textBoxTextInfo.Text = "系统时间:" + DateTime.Now.ToLongTimeString() + "\r\n\r\n相对时间:" + TimeElapse.ToString("f3") + "\r\n\r\nx轴加速度：" + this.a[0].ToString("f3") + "\tg\r\n\r\ny轴加速度：" + this.a[1].ToString("f3") + "\tg\r\n\r\nz轴加速度：" + this.a[2].ToString("f3") + "\tg\r\n\r\nx轴角速度：" + this.w[0].ToString("f3") + "\t度/s\r\n\r\ny轴角速度：" + this.w[1].ToString("f3") + "\t度/s\r\n\r\nz轴角速度：" + this.w[2].ToString("f3") + "\t度/s\r\n\r\nx轴角度：" + this.Angle[0].ToString("f2") + "\t度\r\n\r\ny轴角度：" + this.Angle[1].ToString("f2") + "\t度\r\n\r\nz轴角度：" + this.Angle[2].ToString("f2") + "\t度\r\n\r\n温度：" + this.Temperature.ToString("f2") + "\t℃\r\n\r\n";
      else
        this.textBoxTextInfo.Text = "System Time:" + DateTime.Now.ToLongTimeString() + "\r\n\r\nRelative Time:" + TimeElapse.ToString("f3") + "\r\n\r\nAcceleration X:" + this.a[0].ToString("f3") + " g\r\n\r\nAcceleration Y:" + this.a[1].ToString("f3") + " g\r\n\r\nAcceleration Z:" + this.a[2].ToString("f3") + " g\r\n\r\nAngle Velocity X:" + this.w[0].ToString("f3") + " °/s\r\n\r\nAngle Velocity Y:" + this.w[1].ToString("f3") + " °/s\r\n\r\nAngle Velocity Z:" + this.w[2].ToString("f3") + " °/s\r\n\r\nAngle X:" + this.Angle[0].ToString("f2") + " °\r\n\r\nAngle Y:" + this.Angle[1].ToString("f2") + " °\r\n\r\nAngle Z:" + this.Angle[2].ToString("f2") + " °\r\n\r\nTemperature:" + this.Temperature.ToString("f2") + " ℃\r\n\r\n";
    }

    private void DecodeData(byte[] byteTemp)
    {
      double[] Data = new double[4];
      double num = (DateTime.Now - this.TimeStart).TotalMilliseconds / 1000.0;
      Data[0] = (double) BitConverter.ToInt16(byteTemp, 2);
      Data[1] = (double) BitConverter.ToInt16(byteTemp, 4);
      Data[2] = (double) BitConverter.ToInt16(byteTemp, 6);
      Data[3] = (double) BitConverter.ToInt16(byteTemp, 8);
      Data[3] = Data[3] / 32768.0 * double.Parse(this.textBox9.Text) + double.Parse(this.textBox8.Text);
      switch (byteTemp[1])
      {
        case 80:
          if (this.ToolStripMenuItemChinese.Checked)
          {
            this.textBoxTextInfo.Text = "系统时间:" + DateTime.Now.ToLongTimeString() + "\r\n\r\n相对时间:" + num.ToString("f3") + "\r\n\r\n检测到MPU6050传感器，使用IIC访问模式，禁止串口访问！";
            break;
          }
          this.textBoxTextInfo.Text = "System Time:" + DateTime.Now.ToLongTimeString() + "\r\n\r\nRelative Time:" + num.ToString("f3") + "\r\n\r\nDetected MPU6050 sensor, use IIC mode, serial port is forbidden";
          break;
        case 81:
          Data[0] = Data[0] / 32768.0 * double.Parse(this.textBox3.Text) + double.Parse(this.textBox2.Text);
          Data[1] = Data[1] / 32768.0 * double.Parse(this.textBox3.Text) + double.Parse(this.textBox2.Text);
          Data[2] = Data[2] / 32768.0 * double.Parse(this.textBox3.Text) + double.Parse(this.textBox2.Text);
          if (this.fsAccelerate.CanWrite)
            this.swAccelerate.WriteLine(num.ToString("f3") + "\t" + Data[0].ToString("f4") + "\t" + Data[1].ToString("f4") + "\t" + Data[2].ToString("f4") + "\t" + Data[3].ToString("f2"));
          if (num - this.LastTime[0] < 0.1)
            break;
          this.LastTime[0] = num;
          this.chart1.Series["ax"].Points.AddXY(num, Data[0]);
          this.chart1.Series["ay"].Points.AddXY(num, Data[1]);
          this.chart1.Series["az"].Points.AddXY(num, Data[2]);
          if (this.chart1.Series["ax"].Points.Count > 200)
          {
            this.chart1.Series["ax"].Points.RemoveAt(0);
            this.chart1.Series["ay"].Points.RemoveAt(0);
            this.chart1.Series["az"].Points.RemoveAt(0);
            this.chart1.ChartAreas[0].RecalculateAxesScale();
          }
          this.UpdateIMUData("Accelerate", num, Data);
          break;
        case 82:
          Data[0] = Data[0] / 32768.0 * double.Parse(this.textBox5.Text) + double.Parse(this.textBox4.Text);
          Data[1] = Data[1] / 32768.0 * double.Parse(this.textBox5.Text) + double.Parse(this.textBox4.Text);
          Data[2] = Data[2] / 32768.0 * double.Parse(this.textBox5.Text) + double.Parse(this.textBox4.Text);
          if (this.fsGyro.CanWrite)
            this.swGyro.WriteLine(num.ToString("f3") + "\t" + Data[0].ToString("f4") + "\t" + Data[1].ToString("f4") + "\t" + Data[2].ToString("f4") + "\t" + Data[3].ToString("f2"));
          if (num - this.LastTime[1] < 0.1)
            break;
          this.LastTime[1] = num;
          if (this.chart2.Series["wx"].Points.Count > 200)
          {
            this.chart2.Series["wx"].Points.RemoveAt(0);
            this.chart2.Series["wy"].Points.RemoveAt(0);
            this.chart2.Series["wz"].Points.RemoveAt(0);
          }
          this.chart2.Series["wx"].Points.AddXY(num, Data[0]);
          this.chart2.Series["wy"].Points.AddXY(num, Data[1]);
          this.chart2.Series["wz"].Points.AddXY(num, Data[2]);
          if (this.chart2.Series["wx"].Points.Count > 200)
          {
            this.chart2.Series["wx"].Points.RemoveAt(0);
            this.chart2.Series["wy"].Points.RemoveAt(0);
            this.chart2.Series["wz"].Points.RemoveAt(0);
            this.chart2.ChartAreas[0].RecalculateAxesScale();
          }
          this.UpdateIMUData("AngleVelocity", num, Data);
          break;
        case 83:
          Data[0] = Data[0] / 32768.0 * double.Parse(this.textBox7.Text) + double.Parse(this.textBox6.Text);
          Data[1] = Data[1] / 32768.0 * double.Parse(this.textBox7.Text) + double.Parse(this.textBox6.Text);
          Data[2] = Data[2] / 32768.0 * double.Parse(this.textBox7.Text) + double.Parse(this.textBox6.Text);
          this.Form3D.RefreshOSG(Data[0], Data[1], Data[2]);
          if (this.fsAngle.CanWrite)
            this.swAngle.WriteLine(num.ToString("f3") + "\t" + Data[0].ToString("f4") + "\t" + Data[1].ToString("f4") + "\t" + Data[2].ToString("f4") + "\t" + Data[3].ToString("f2"));
          if (num - this.LastTime[2] < 0.1)
            break;
          this.LastTime[2] = num;
          this.chart3.Series["Roll"].Points.AddXY(num, Data[0]);
          this.chart3.Series["Pitch"].Points.AddXY(num, Data[1]);
          this.chart3.Series["Yaw"].Points.AddXY(num, Data[2]);
          if (this.chart3.Series["Roll"].Points.Count > 200)
          {
            this.chart3.Series["Roll"].Points.RemoveAt(0);
            this.chart3.Series["Pitch"].Points.RemoveAt(0);
            this.chart3.Series["Yaw"].Points.RemoveAt(0);
            this.chart3.ChartAreas[0].RecalculateAxesScale();
          }
          this.UpdateIMUData("Angle", num, Data);
          break;
      }
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      byte[] numArray = new byte[1000];
      if (this.bClosing)
        return;
      try
      {
        this.bListening = true;
        ushort num = 0;
        try
        {
          num = (ushort) this.spSerialPort.Read(this.RxBuffer, (int) this.usRxLength, 100);
        }
        catch (Exception ex)
        {
        }
        this.usRxLength += num;
        while (this.usRxLength >= (ushort) 11)
        {
          Form1.UpdateData updateData = new Form1.UpdateData(this.DecodeData);
          this.RxBuffer.CopyTo((Array) numArray, 0);
          if (!(numArray[0] == (byte) 85 & (numArray[1] == (byte) 80 | numArray[1] == (byte) 81 | numArray[1] == (byte) 82 | numArray[1] == (byte) 83)))
          {
            for (int index = 1; index < (int) this.usRxLength; ++index)
              this.RxBuffer[index - 1] = this.RxBuffer[index];
            --this.usRxLength;
          }
          else
          {
            this.Invoke((Delegate) updateData, (object) numArray);
            for (int index = 11; index < (int) this.usRxLength; ++index)
              this.RxBuffer[index - 11] = this.RxBuffer[index];
            this.usRxLength -= (ushort) 11;
          }
        }
      }
      finally
      {
        this.bListening = false;
      }
    }

    private sbyte sbSumCheck(byte[] byteData, byte byteLength)
    {
      byte num = 0;
      for (byte index = 0; (int) index < (int) byteLength - 2; ++index)
        num += byteData[(int) index];
      return (int) byteData[(int) byteLength - 1] == (int) num ? (sbyte) 0 : (sbyte) -1;
    }

    public sbyte SendMessage(byte[] byteSend)
    {
      if (!this.spSerialPort.IsOpen)
      {
        int num = (int) MessageBox.Show(this.rm.GetString("PortNotOpen"), "Error!");
        return -1;
      }
      try
      {
        this.spSerialPort.Write(byteSend, 0, byteSend.Length);
        return 0;
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message);
        return -1;
      }
    }

    private void CommandToolBotton_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 82
      }) == (sbyte) 0)
        this.Status.Text = this.rm.GetString("InitialSuccess");
      else
        this.Status.Text = this.rm.GetString("InitialFailed");
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      Process.Start("http://RobotControl.taobao.com");
    }

    private void toolStripButtonStart_Click(object sender, EventArgs e)
    {
    }

    private void WriteConfig()
    {
      try
      {
        FileStream fileStream = new FileStream("Config.ini", FileMode.Create);
        StreamWriter streamWriter = new StreamWriter((Stream) fileStream);
        if (this.toolStripMenuItem3.Checked)
          streamWriter.WriteLine("9600");
        else
          streamWriter.WriteLine("115200");
        if (this.ToolStripMenuItemEnglish.Checked)
          streamWriter.WriteLine("en");
        else
          streamWriter.WriteLine("ch");
        streamWriter.WriteLine("加速度偏移：");
        streamWriter.WriteLine(this.textBox2.Text);
        streamWriter.WriteLine("加速度幅值：");
        streamWriter.WriteLine(this.textBox3.Text);
        streamWriter.WriteLine("角速度偏移：");
        streamWriter.WriteLine(this.textBox4.Text);
        streamWriter.WriteLine("角速度幅值：");
        streamWriter.WriteLine(this.textBox5.Text);
        streamWriter.WriteLine("角度偏移：");
        streamWriter.WriteLine(this.textBox6.Text);
        streamWriter.WriteLine("角度幅值：");
        streamWriter.WriteLine(this.textBox7.Text);
        streamWriter.WriteLine("温度偏移：");
        streamWriter.WriteLine(this.textBox8.Text);
        streamWriter.WriteLine("温度幅值：");
        streamWriter.WriteLine(this.textBox9.Text);
        streamWriter.Close();
        fileStream.Close();
      }
      catch
      {
      }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        this.WriteConfig();
        this.PortClose((object) null, (EventArgs) null);
      }
      catch
      {
      }
    }

    private void toolStripButton3_Click(object sender, EventArgs e)
    {
      if (this.toolStripButton3.Text == this.rm.GetString("Record"))
      {
        this.toolStripButton3.Text = this.rm.GetString("Stop");
        this.fsAccelerate = new FileStream("Acceleration.txt", FileMode.Create);
        this.fsGyro = new FileStream("AngleVelocity.txt", FileMode.Create);
        this.fsAngle = new FileStream("Angle.txt", FileMode.Create);
        this.swAccelerate = new StreamWriter((Stream) this.fsAccelerate);
        this.swGyro = new StreamWriter((Stream) this.fsGyro);
        this.swAngle = new StreamWriter((Stream) this.fsAngle);
        this.swAccelerate.WriteLine("Start Time：" + this.TimeStart.ToLongDateString() + this.TimeStart.ToString("HH:mm:ss.fff"));
        this.swAccelerate.WriteLine("Time(s)\tAcc X \tAcc Y \tAcc Z \tTemperature");
        this.swGyro.WriteLine("Start Time:" + this.TimeStart.ToLongDateString() + this.TimeStart.ToLongTimeString());
        this.swGyro.WriteLine("Time(s)\tAngle Velocity X \tAngle Velocity Y \tAngle Velocity Z \tTemperature");
        this.swAngle.WriteLine("Start Time:" + this.TimeStart.ToLongDateString() + this.TimeStart.ToLongTimeString());
        this.swAngle.WriteLine("Time(s)\tAngle X\tAngle Y\tAngleZ\tTemperature");
      }
      else
      {
        this.toolStripButton3.Text = this.rm.GetString("Record");
        this.swAccelerate.Flush();
        this.swAccelerate.Close();
        this.fsAccelerate.Close();
        this.swGyro.Flush();
        this.swGyro.Close();
        this.fsGyro.Close();
        this.swAngle.Flush();
        this.swAngle.Close();
        this.fsAngle.Close();
      }
    }

    private void toolStripMenuItem2_Click(object sender, EventArgs e)
    {
      byte[] byteSend = new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 99
      };
      this.spSerialPort.BaudRate = 9600;
      if (this.SendMessage(byteSend) != (sbyte) 0)
        return;
      Thread.Sleep(200);
      this.spSerialPort.BaudRate = 115200;
      if (this.SendMessage(byteSend) != (sbyte) 0)
        return;
      this.toolStripMenuItem2.Checked = true;
      this.toolStripMenuItem3.Checked = false;
      this.Status.Text = this.rm.GetString("BaundConfiged") + "115200！";
      this.WriteConfig();
    }

    private void toolStripMenuItem3_Click(object sender, EventArgs e)
    {
      byte[] byteSend = new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 100
      };
      this.spSerialPort.BaudRate = 115200;
      if (this.SendMessage(byteSend) != (sbyte) 0)
        return;
      Thread.Sleep(200);
      this.spSerialPort.BaudRate = 9600;
      if (this.SendMessage(byteSend) != (sbyte) 0)
        return;
      this.toolStripMenuItem3.Checked = true;
      this.toolStripMenuItem2.Checked = false;
      this.Status.Text = this.rm.GetString("BaundConfiged") + "9600！";
      this.WriteConfig();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.textBox2.Text = "0";
      this.textBox3.Text = "16";
      this.textBox4.Text = "0";
      this.textBox5.Text = "2000";
      this.textBox6.Text = "0";
      this.textBox7.Text = "180";
      this.textBox8.Text = "36.53";
      this.textBox9.Text = "96.38";
    }

    private void tabPage4_Click(object sender, EventArgs e)
    {
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      if (this.bForm3DShow)
      {
        this.Form3D.Hide();
        this.bForm3DShow = false;
      }
      else
      {
        this.Form3D.Show();
        this.bForm3DShow = true;
      }
    }

    private void 串口模式ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 97
      }) != (sbyte) 0)
        return;
      this.Status.Text = this.rm.GetString("ChooseUsartMode");
      this.串口模式ToolStripMenuItem.Checked = true;
      this.iIC模式ToolStripMenuItem.Checked = false;
      this.LastTime[0] = 0.0;
      this.LastTime[1] = 0.0;
      this.LastTime[2] = 0.0;
    }

    private void iIC模式ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 98
      }) != (sbyte) 0)
        return;
      this.Status.Text = this.rm.GetString("ChooseIICMode");
      this.串口模式ToolStripMenuItem.Checked = false;
      this.iIC模式ToolStripMenuItem.Checked = true;
      this.chart1.Series[0].Points.Clear();
      this.chart1.Series[1].Points.Clear();
      this.chart1.Series[2].Points.Clear();
      this.chart2.Series[0].Points.Clear();
      this.chart2.Series[1].Points.Clear();
      this.chart2.Series[2].Points.Clear();
      this.chart3.Series[0].Points.Clear();
      this.chart3.Series[1].Points.Clear();
      this.chart3.Series[2].Points.Clear();
      this.TimeStart = DateTime.Now;
      this.LastTime[0] = 0.0;
      this.LastTime[1] = 0.0;
      this.LastTime[2] = 0.0;
    }

    private void toolStripButton2_Click_1(object sender, EventArgs e)
    {
      this.chart1.Series[0].Points.Clear();
      this.chart1.Series[1].Points.Clear();
      this.chart1.Series[2].Points.Clear();
      this.chart2.Series[0].Points.Clear();
      this.chart2.Series[1].Points.Clear();
      this.chart2.Series[2].Points.Clear();
      this.chart3.Series[0].Points.Clear();
      this.chart3.Series[1].Points.Clear();
      this.chart3.Series[2].Points.Clear();
    }

    private void toolStripButton4_Click(object sender, EventArgs e)
    {
      if (this.ToolStripMenuItemChinese.Checked)
      {
        int num1 = (int) MessageBox.Show("启动三维显示引擎，按【F】键切换全屏显示状态", "Warning");
      }
      else
      {
        int num2 = (int) MessageBox.Show("Start 3D visual engine, press [F] to switch full screen mode", "Warning");
      }
      Process process = new Process();
      try
      {
        process.StartInfo.FileName = Application.StartupPath + "\\3D\\winOSG.exe";
        process.StartInfo.WorkingDirectory = Path.GetDirectoryName(process.StartInfo.FileName);
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.Start();
      }
      catch (Exception ex)
      {
      }
    }

    private void toolStripButton5_Click(object sender, EventArgs e)
    {
      Process.Start("http://Elecmaster.net");
    }

    private void toolStripButton6_Click(object sender, EventArgs e)
    {
      Process.Start("http://www.aliexpress.com/store/1836321");
    }

    private void ClearCheck()
    {
      this.sToolStripMenuItem.Checked = false;
      this.sToolStripMenuItem1.Checked = false;
      this.sToolStripMenuItem2.Checked = false;
      this.sToolStripMenuItem3.Checked = false;
      this.sToolStripMenuItem4.Checked = false;
      this.sToolStripMenuItem5.Checked = false;
      this.sToolStripMenuItem6.Checked = false;
      this.sToolStripMenuItem7.Checked = false;
      this.sToolStripMenuItem8.Checked = false;
      this.sToolStripMenuItem9.Checked = false;
      this.sToolStripMenuItem10.Checked = false;
      this.sToolStripMenuItem11.Checked = false;
      this.sToolStripMenuItem12.Checked = false;
      this.sToolStripMenuItem13.Checked = false;
      this.sToolStripMenuItem14.Checked = false;
    }

    private void sToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 113
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 114
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem2_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 115
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem3_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 116
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem4_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 117
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem5_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 118
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem6_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 119
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem7_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 120
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem8_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 121
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem9_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 122
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem10_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 123
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem11_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 124
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem12_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 125
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem13_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 126
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    private void sToolStripMenuItem14_Click(object sender, EventArgs e)
    {
      if (this.SendMessage(new byte[3]
      {
        byte.MaxValue,
        (byte) 170,
        (byte) 127
      }) != (sbyte) 0)
        return;
      this.ClearCheck();
      ((ToolStripMenuItem) sender).Checked = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Form1));
      ChartArea chartArea1 = new ChartArea();
      Legend legend1 = new Legend();
      Series series1 = new Series();
      Series series2 = new Series();
      Series series3 = new Series();
      Title title1 = new Title();
      ChartArea chartArea2 = new ChartArea();
      Legend legend2 = new Legend();
      Series series4 = new Series();
      Series series5 = new Series();
      Series series6 = new Series();
      Title title2 = new Title();
      ChartArea chartArea3 = new ChartArea();
      Legend legend3 = new Legend();
      Series series7 = new Series();
      Series series8 = new Series();
      Series series9 = new Series();
      Title title3 = new Title();
      this.toolStrip1 = new ToolStrip();
      this.toolStripComSet = new ToolStripDropDownButton();
      this.toolStripDropDownButton1 = new ToolStripDropDownButton();
      this.toolStripMenuItem2 = new ToolStripMenuItem();
      this.toolStripMenuItem3 = new ToolStripMenuItem();
      this.toolStripDropDownButton2 = new ToolStripDropDownButton();
      this.串口模式ToolStripMenuItem = new ToolStripMenuItem();
      this.iIC模式ToolStripMenuItem = new ToolStripMenuItem();
      this.toolStripButtonAngleInitial = new ToolStripButton();
      this.toolStripDropDownButton4 = new ToolStripDropDownButton();
      this.sToolStripMenuItem = new ToolStripMenuItem();
      this.sToolStripMenuItem1 = new ToolStripMenuItem();
      this.sToolStripMenuItem2 = new ToolStripMenuItem();
      this.sToolStripMenuItem3 = new ToolStripMenuItem();
      this.sToolStripMenuItem4 = new ToolStripMenuItem();
      this.sToolStripMenuItem5 = new ToolStripMenuItem();
      this.sToolStripMenuItem6 = new ToolStripMenuItem();
      this.sToolStripMenuItem7 = new ToolStripMenuItem();
      this.sToolStripMenuItem8 = new ToolStripMenuItem();
      this.sToolStripMenuItem9 = new ToolStripMenuItem();
      this.sToolStripMenuItem10 = new ToolStripMenuItem();
      this.sToolStripMenuItem11 = new ToolStripMenuItem();
      this.sToolStripMenuItem12 = new ToolStripMenuItem();
      this.sToolStripMenuItem13 = new ToolStripMenuItem();
      this.sToolStripMenuItem14 = new ToolStripMenuItem();
      this.toolStripButton3 = new ToolStripButton();
      this.toolStripDropDownButton3 = new ToolStripDropDownButton();
      this.ToolStripMenuItemChinese = new ToolStripMenuItem();
      this.ToolStripMenuItemEnglish = new ToolStripMenuItem();
      this.toolStripButton2 = new ToolStripButton();
      this.toolStripButton4 = new ToolStripButton();
      this.toolStripButton5 = new ToolStripButton();
      this.toolStripButton6 = new ToolStripButton();
      this.tabControlChart = new TabControl();
      this.tabPage1 = new TabPage();
      this.chart1 = new Chart();
      this.tabPage3 = new TabPage();
      this.chart2 = new Chart();
      this.tabPage2 = new TabPage();
      this.chart3 = new Chart();
      this.tabPage4 = new TabPage();
      this.groupBox1 = new GroupBox();
      this.textBox3 = new TextBox();
      this.label9 = new Label();
      this.label2 = new Label();
      this.textBox2 = new TextBox();
      this.label1 = new Label();
      this.button2 = new Button();
      this.groupBox4 = new GroupBox();
      this.textBox8 = new TextBox();
      this.label12 = new Label();
      this.label7 = new Label();
      this.textBox9 = new TextBox();
      this.label8 = new Label();
      this.groupBox3 = new GroupBox();
      this.textBox6 = new TextBox();
      this.label11 = new Label();
      this.label5 = new Label();
      this.textBox7 = new TextBox();
      this.label6 = new Label();
      this.groupBox2 = new GroupBox();
      this.textBox4 = new TextBox();
      this.label10 = new Label();
      this.label3 = new Label();
      this.textBox5 = new TextBox();
      this.label4 = new Label();
      this.spSerialPort = new SerialPort(this.components);
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.panel1 = new Panel();
      this.splitContainer1 = new SplitContainer();
      this.splitContainer2 = new SplitContainer();
      this.textBoxTextInfo = new TextBox();
      this.splitContainer3 = new SplitContainer();
      this.pictureBox1 = new PictureBox();
      this.linkLabel1 = new LinkLabel();
      this.splitContainer4 = new SplitContainer();
      this.statusStrip1 = new StatusStrip();
      this.Status = new ToolStripStatusLabel();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStrip1.SuspendLayout();
      this.tabControlChart.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.chart1.BeginInit();
      this.tabPage3.SuspendLayout();
      this.chart2.BeginInit();
      this.tabPage2.SuspendLayout();
      this.chart3.BeginInit();
      this.tabPage4.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.panel1.SuspendLayout();
      ((ISupportInitialize) this.splitContainer1).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((ISupportInitialize) this.splitContainer2).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      ((ISupportInitialize) this.splitContainer3).BeginInit();
      this.splitContainer3.Panel1.SuspendLayout();
      this.splitContainer3.Panel2.SuspendLayout();
      this.splitContainer3.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      ((ISupportInitialize) this.splitContainer4).BeginInit();
      this.splitContainer4.Panel1.SuspendLayout();
      this.splitContainer4.SuspendLayout();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[11]
      {
        (ToolStripItem) this.toolStripComSet,
        (ToolStripItem) this.toolStripDropDownButton1,
        (ToolStripItem) this.toolStripDropDownButton2,
        (ToolStripItem) this.toolStripButtonAngleInitial,
        (ToolStripItem) this.toolStripDropDownButton4,
        (ToolStripItem) this.toolStripButton3,
        (ToolStripItem) this.toolStripDropDownButton3,
        (ToolStripItem) this.toolStripButton2,
        (ToolStripItem) this.toolStripButton4,
        (ToolStripItem) this.toolStripButton5,
        (ToolStripItem) this.toolStripButton6
      });
      componentResourceManager.ApplyResources((object) this.toolStrip1, "toolStrip1");
      this.toolStrip1.Name = "toolStrip1";
      this.toolStripComSet.DisplayStyle = ToolStripItemDisplayStyle.Text;
      componentResourceManager.ApplyResources((object) this.toolStripComSet, "toolStripComSet");
      this.toolStripComSet.Name = "toolStripComSet";
      this.toolStripComSet.MouseEnter += new EventHandler(this.RefreshComPort);
      this.toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.toolStripMenuItem3
      });
      componentResourceManager.ApplyResources((object) this.toolStripDropDownButton1, "toolStripDropDownButton1");
      this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
      this.toolStripMenuItem2.Checked = true;
      this.toolStripMenuItem2.CheckState = CheckState.Checked;
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      componentResourceManager.ApplyResources((object) this.toolStripMenuItem2, "toolStripMenuItem2");
      this.toolStripMenuItem2.Click += new EventHandler(this.toolStripMenuItem2_Click);
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      componentResourceManager.ApplyResources((object) this.toolStripMenuItem3, "toolStripMenuItem3");
      this.toolStripMenuItem3.Click += new EventHandler(this.toolStripMenuItem3_Click);
      this.toolStripDropDownButton2.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.toolStripDropDownButton2.DropDownItems.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.串口模式ToolStripMenuItem,
        (ToolStripItem) this.iIC模式ToolStripMenuItem
      });
      componentResourceManager.ApplyResources((object) this.toolStripDropDownButton2, "toolStripDropDownButton2");
      this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
      this.串口模式ToolStripMenuItem.Checked = true;
      this.串口模式ToolStripMenuItem.CheckState = CheckState.Checked;
      this.串口模式ToolStripMenuItem.Name = "串口模式ToolStripMenuItem";
      componentResourceManager.ApplyResources((object) this.串口模式ToolStripMenuItem, "串口模式ToolStripMenuItem");
      this.串口模式ToolStripMenuItem.Click += new EventHandler(this.串口模式ToolStripMenuItem_Click);
      this.iIC模式ToolStripMenuItem.Name = "iIC模式ToolStripMenuItem";
      componentResourceManager.ApplyResources((object) this.iIC模式ToolStripMenuItem, "iIC模式ToolStripMenuItem");
      this.iIC模式ToolStripMenuItem.Click += new EventHandler(this.iIC模式ToolStripMenuItem_Click);
      this.toolStripButtonAngleInitial.DisplayStyle = ToolStripItemDisplayStyle.Text;
      componentResourceManager.ApplyResources((object) this.toolStripButtonAngleInitial, "toolStripButtonAngleInitial");
      this.toolStripButtonAngleInitial.Name = "toolStripButtonAngleInitial";
      this.toolStripButtonAngleInitial.Click += new EventHandler(this.CommandToolBotton_Click);
      this.toolStripDropDownButton4.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.toolStripDropDownButton4.DropDownItems.AddRange(new ToolStripItem[15]
      {
        (ToolStripItem) this.sToolStripMenuItem,
        (ToolStripItem) this.sToolStripMenuItem1,
        (ToolStripItem) this.sToolStripMenuItem2,
        (ToolStripItem) this.sToolStripMenuItem3,
        (ToolStripItem) this.sToolStripMenuItem4,
        (ToolStripItem) this.sToolStripMenuItem5,
        (ToolStripItem) this.sToolStripMenuItem6,
        (ToolStripItem) this.sToolStripMenuItem7,
        (ToolStripItem) this.sToolStripMenuItem8,
        (ToolStripItem) this.sToolStripMenuItem9,
        (ToolStripItem) this.sToolStripMenuItem10,
        (ToolStripItem) this.sToolStripMenuItem11,
        (ToolStripItem) this.sToolStripMenuItem12,
        (ToolStripItem) this.sToolStripMenuItem13,
        (ToolStripItem) this.sToolStripMenuItem14
      });
      componentResourceManager.ApplyResources((object) this.toolStripDropDownButton4, "toolStripDropDownButton4");
      this.toolStripDropDownButton4.Name = "toolStripDropDownButton4";
      this.sToolStripMenuItem.Name = "sToolStripMenuItem";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem, "sToolStripMenuItem");
      this.sToolStripMenuItem.Click += new EventHandler(this.sToolStripMenuItem_Click);
      this.sToolStripMenuItem1.Name = "sToolStripMenuItem1";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem1, "sToolStripMenuItem1");
      this.sToolStripMenuItem1.Click += new EventHandler(this.sToolStripMenuItem1_Click);
      this.sToolStripMenuItem2.Name = "sToolStripMenuItem2";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem2, "sToolStripMenuItem2");
      this.sToolStripMenuItem2.Click += new EventHandler(this.sToolStripMenuItem2_Click);
      this.sToolStripMenuItem3.Checked = true;
      this.sToolStripMenuItem3.CheckState = CheckState.Checked;
      this.sToolStripMenuItem3.Name = "sToolStripMenuItem3";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem3, "sToolStripMenuItem3");
      this.sToolStripMenuItem3.Click += new EventHandler(this.sToolStripMenuItem3_Click);
      this.sToolStripMenuItem4.Name = "sToolStripMenuItem4";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem4, "sToolStripMenuItem4");
      this.sToolStripMenuItem4.Click += new EventHandler(this.sToolStripMenuItem4_Click);
      this.sToolStripMenuItem5.Name = "sToolStripMenuItem5";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem5, "sToolStripMenuItem5");
      this.sToolStripMenuItem5.Click += new EventHandler(this.sToolStripMenuItem5_Click);
      this.sToolStripMenuItem6.Name = "sToolStripMenuItem6";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem6, "sToolStripMenuItem6");
      this.sToolStripMenuItem6.Click += new EventHandler(this.sToolStripMenuItem6_Click);
      this.sToolStripMenuItem7.Name = "sToolStripMenuItem7";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem7, "sToolStripMenuItem7");
      this.sToolStripMenuItem7.Click += new EventHandler(this.sToolStripMenuItem7_Click);
      this.sToolStripMenuItem8.Name = "sToolStripMenuItem8";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem8, "sToolStripMenuItem8");
      this.sToolStripMenuItem8.Click += new EventHandler(this.sToolStripMenuItem8_Click);
      this.sToolStripMenuItem9.Name = "sToolStripMenuItem9";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem9, "sToolStripMenuItem9");
      this.sToolStripMenuItem9.Click += new EventHandler(this.sToolStripMenuItem9_Click);
      this.sToolStripMenuItem10.Name = "sToolStripMenuItem10";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem10, "sToolStripMenuItem10");
      this.sToolStripMenuItem10.Click += new EventHandler(this.sToolStripMenuItem10_Click);
      this.sToolStripMenuItem11.Name = "sToolStripMenuItem11";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem11, "sToolStripMenuItem11");
      this.sToolStripMenuItem11.Click += new EventHandler(this.sToolStripMenuItem11_Click);
      this.sToolStripMenuItem12.Name = "sToolStripMenuItem12";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem12, "sToolStripMenuItem12");
      this.sToolStripMenuItem12.Click += new EventHandler(this.sToolStripMenuItem12_Click);
      this.sToolStripMenuItem13.Name = "sToolStripMenuItem13";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem13, "sToolStripMenuItem13");
      this.sToolStripMenuItem13.Click += new EventHandler(this.sToolStripMenuItem13_Click);
      this.sToolStripMenuItem14.Name = "sToolStripMenuItem14";
      componentResourceManager.ApplyResources((object) this.sToolStripMenuItem14, "sToolStripMenuItem14");
      this.sToolStripMenuItem14.Click += new EventHandler(this.sToolStripMenuItem14_Click);
      this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Text;
      componentResourceManager.ApplyResources((object) this.toolStripButton3, "toolStripButton3");
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripDropDownButton3.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.toolStripDropDownButton3.DropDownItems.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.ToolStripMenuItemChinese,
        (ToolStripItem) this.ToolStripMenuItemEnglish
      });
      componentResourceManager.ApplyResources((object) this.toolStripDropDownButton3, "toolStripDropDownButton3");
      this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
      this.ToolStripMenuItemChinese.Name = "ToolStripMenuItemChinese";
      componentResourceManager.ApplyResources((object) this.ToolStripMenuItemChinese, "ToolStripMenuItemChinese");
      this.ToolStripMenuItemChinese.Click += new EventHandler(this.languageMenuSimlpeChinese_Click);
      this.ToolStripMenuItemEnglish.Name = "ToolStripMenuItemEnglish";
      componentResourceManager.ApplyResources((object) this.ToolStripMenuItemEnglish, "ToolStripMenuItemEnglish");
      this.ToolStripMenuItemEnglish.Click += new EventHandler(this.languageMenuEnglish_Click);
      this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Text;
      componentResourceManager.ApplyResources((object) this.toolStripButton2, "toolStripButton2");
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Click += new EventHandler(this.toolStripButton2_Click_1);
      this.toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Text;
      componentResourceManager.ApplyResources((object) this.toolStripButton4, "toolStripButton4");
      this.toolStripButton4.Name = "toolStripButton4";
      this.toolStripButton4.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.Text;
      componentResourceManager.ApplyResources((object) this.toolStripButton5, "toolStripButton5");
      this.toolStripButton5.Name = "toolStripButton5";
      this.toolStripButton5.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton6.DisplayStyle = ToolStripItemDisplayStyle.Text;
      componentResourceManager.ApplyResources((object) this.toolStripButton6, "toolStripButton6");
      this.toolStripButton6.Name = "toolStripButton6";
      this.toolStripButton6.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabControlChart.Controls.Add((Control) this.tabPage1);
      this.tabControlChart.Controls.Add((Control) this.tabPage3);
      this.tabControlChart.Controls.Add((Control) this.tabPage2);
      this.tabControlChart.Controls.Add((Control) this.tabPage4);
      componentResourceManager.ApplyResources((object) this.tabControlChart, "tabControlChart");
      this.tabControlChart.Name = "tabControlChart";
      this.tabControlChart.SelectedIndex = 0;
      this.tabPage1.Controls.Add((Control) this.chart1);
      componentResourceManager.ApplyResources((object) this.tabPage1, "tabPage1");
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.UseVisualStyleBackColor = true;
      chartArea1.AxisX.MajorGrid.Interval = 10.0;
      chartArea1.AxisX.MajorGrid.IntervalType = DateTimeIntervalType.Number;
      chartArea1.AxisX.MinorGrid.Enabled = true;
      chartArea1.AxisX.MinorGrid.Interval = 2.0;
      chartArea1.AxisX.MinorGrid.IntervalType = DateTimeIntervalType.Number;
      chartArea1.AxisX.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
      chartArea1.AxisY.MajorGrid.Interval = 0.0;
      chartArea1.AxisY.MajorGrid.IntervalType = DateTimeIntervalType.Auto;
      chartArea1.AxisY.MinorGrid.Enabled = true;
      chartArea1.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
      chartArea1.BackColor = Color.DimGray;
      chartArea1.BorderDashStyle = ChartDashStyle.Solid;
      chartArea1.Name = "ChartArea1";
      this.chart1.ChartAreas.Add(chartArea1);
      componentResourceManager.ApplyResources((object) this.chart1, "chart1");
      legend1.DockedToChartArea = "ChartArea1";
      legend1.Name = "Legend1";
      this.chart1.Legends.Add(legend1);
      this.chart1.Name = "chart1";
      series1.BorderWidth = 2;
      series1.ChartArea = "ChartArea1";
      series1.ChartType = SeriesChartType.Line;
      series1.Legend = "Legend1";
      series1.Name = "ax";
      series2.BorderWidth = 2;
      series2.ChartArea = "ChartArea1";
      series2.ChartType = SeriesChartType.Line;
      series2.Legend = "Legend1";
      series2.Name = "ay";
      series3.BorderWidth = 2;
      series3.ChartArea = "ChartArea1";
      series3.ChartType = SeriesChartType.Line;
      series3.Legend = "Legend1";
      series3.Name = "az";
      this.chart1.Series.Add(series1);
      this.chart1.Series.Add(series2);
      this.chart1.Series.Add(series3);
      title1.Name = "Title1";
      title1.Text = "角速度曲线(°/s)";
      this.chart1.Titles.Add(title1);
      this.tabPage3.Controls.Add((Control) this.chart2);
      componentResourceManager.ApplyResources((object) this.tabPage3, "tabPage3");
      this.tabPage3.Name = "tabPage3";
      this.tabPage3.UseVisualStyleBackColor = true;
      chartArea2.AxisX.MajorGrid.Interval = 10.0;
      chartArea2.AxisX.MajorGrid.IntervalType = DateTimeIntervalType.Number;
      chartArea2.AxisX.MinorGrid.Enabled = true;
      chartArea2.AxisX.MinorGrid.Interval = 2.0;
      chartArea2.AxisX.MinorGrid.IntervalType = DateTimeIntervalType.Number;
      chartArea2.AxisX.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
      chartArea2.AxisY.MajorGrid.Interval = 0.0;
      chartArea2.AxisY.MajorGrid.IntervalType = DateTimeIntervalType.Auto;
      chartArea2.AxisY.MinorGrid.Enabled = true;
      chartArea2.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
      chartArea2.BackColor = Color.DimGray;
      chartArea2.BorderDashStyle = ChartDashStyle.Solid;
      chartArea2.Name = "ChartArea1";
      this.chart2.ChartAreas.Add(chartArea2);
      componentResourceManager.ApplyResources((object) this.chart2, "chart2");
      legend2.DockedToChartArea = "ChartArea1";
      legend2.Name = "Legend1";
      this.chart2.Legends.Add(legend2);
      this.chart2.Name = "chart2";
      series4.BorderWidth = 2;
      series4.ChartArea = "ChartArea1";
      series4.ChartType = SeriesChartType.Line;
      series4.Legend = "Legend1";
      series4.Name = "wx";
      series5.BorderWidth = 2;
      series5.ChartArea = "ChartArea1";
      series5.ChartType = SeriesChartType.Line;
      series5.Legend = "Legend1";
      series5.Name = "wy";
      series6.BorderWidth = 2;
      series6.ChartArea = "ChartArea1";
      series6.ChartType = SeriesChartType.Line;
      series6.Legend = "Legend1";
      series6.Name = "wz";
      this.chart2.Series.Add(series4);
      this.chart2.Series.Add(series5);
      this.chart2.Series.Add(series6);
      title2.Name = "Title1";
      title2.Text = "角速度曲线(°/s)";
      this.chart2.Titles.Add(title2);
      this.tabPage2.Controls.Add((Control) this.chart3);
      componentResourceManager.ApplyResources((object) this.tabPage2, "tabPage2");
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.UseVisualStyleBackColor = true;
      chartArea3.AxisX.MajorGrid.Interval = 10.0;
      chartArea3.AxisX.MajorGrid.IntervalType = DateTimeIntervalType.Number;
      chartArea3.AxisX.MinorGrid.Enabled = true;
      chartArea3.AxisX.MinorGrid.Interval = 2.0;
      chartArea3.AxisX.MinorGrid.IntervalType = DateTimeIntervalType.Number;
      chartArea3.AxisX.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
      chartArea3.AxisY.Maximum = 180.0;
      chartArea3.AxisY.Minimum = -180.0;
      chartArea3.AxisY.MinorGrid.Enabled = true;
      chartArea3.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
      chartArea3.BackColor = Color.DimGray;
      chartArea3.BorderDashStyle = ChartDashStyle.Solid;
      chartArea3.Name = "ChartArea1";
      this.chart3.ChartAreas.Add(chartArea3);
      componentResourceManager.ApplyResources((object) this.chart3, "chart3");
      legend3.DockedToChartArea = "ChartArea1";
      legend3.Name = "Legend1";
      this.chart3.Legends.Add(legend3);
      this.chart3.Name = "chart3";
      series7.BorderWidth = 2;
      series7.ChartArea = "ChartArea1";
      series7.ChartType = SeriesChartType.Line;
      series7.Legend = "Legend1";
      series7.Name = "Roll";
      series8.BorderWidth = 2;
      series8.ChartArea = "ChartArea1";
      series8.ChartType = SeriesChartType.Line;
      series8.Legend = "Legend1";
      series8.Name = "Pitch";
      series9.BorderWidth = 2;
      series9.ChartArea = "ChartArea1";
      series9.ChartType = SeriesChartType.Line;
      series9.Legend = "Legend1";
      series9.Name = "Yaw";
      this.chart3.Series.Add(series7);
      this.chart3.Series.Add(series8);
      this.chart3.Series.Add(series9);
      title3.Name = "Title1";
      title3.Text = "角度曲线(°)";
      this.chart3.Titles.Add(title3);
      this.tabPage4.Controls.Add((Control) this.groupBox1);
      this.tabPage4.Controls.Add((Control) this.button2);
      this.tabPage4.Controls.Add((Control) this.groupBox4);
      this.tabPage4.Controls.Add((Control) this.groupBox3);
      this.tabPage4.Controls.Add((Control) this.groupBox2);
      componentResourceManager.ApplyResources((object) this.tabPage4, "tabPage4");
      this.tabPage4.Name = "tabPage4";
      this.tabPage4.UseVisualStyleBackColor = true;
      this.tabPage4.Click += new EventHandler(this.tabPage4_Click);
      componentResourceManager.ApplyResources((object) this.groupBox1, "groupBox1");
      this.groupBox1.Controls.Add((Control) this.textBox3);
      this.groupBox1.Controls.Add((Control) this.label9);
      this.groupBox1.Controls.Add((Control) this.label2);
      this.groupBox1.Controls.Add((Control) this.textBox2);
      this.groupBox1.Controls.Add((Control) this.label1);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.TabStop = false;
      componentResourceManager.ApplyResources((object) this.textBox3, "textBox3");
      this.textBox3.Name = "textBox3";
      componentResourceManager.ApplyResources((object) this.label9, "label9");
      this.label9.Name = "label9";
      componentResourceManager.ApplyResources((object) this.label2, "label2");
      this.label2.Name = "label2";
      componentResourceManager.ApplyResources((object) this.textBox2, "textBox2");
      this.textBox2.Name = "textBox2";
      componentResourceManager.ApplyResources((object) this.label1, "label1");
      this.label1.Name = "label1";
      componentResourceManager.ApplyResources((object) this.button2, "button2");
      this.button2.Name = "button2";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.button2_Click);
      componentResourceManager.ApplyResources((object) this.groupBox4, "groupBox4");
      this.groupBox4.Controls.Add((Control) this.textBox8);
      this.groupBox4.Controls.Add((Control) this.label12);
      this.groupBox4.Controls.Add((Control) this.label7);
      this.groupBox4.Controls.Add((Control) this.textBox9);
      this.groupBox4.Controls.Add((Control) this.label8);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.TabStop = false;
      componentResourceManager.ApplyResources((object) this.textBox8, "textBox8");
      this.textBox8.Name = "textBox8";
      componentResourceManager.ApplyResources((object) this.label12, "label12");
      this.label12.Name = "label12";
      componentResourceManager.ApplyResources((object) this.label7, "label7");
      this.label7.Name = "label7";
      componentResourceManager.ApplyResources((object) this.textBox9, "textBox9");
      this.textBox9.Name = "textBox9";
      componentResourceManager.ApplyResources((object) this.label8, "label8");
      this.label8.Name = "label8";
      componentResourceManager.ApplyResources((object) this.groupBox3, "groupBox3");
      this.groupBox3.Controls.Add((Control) this.textBox6);
      this.groupBox3.Controls.Add((Control) this.label11);
      this.groupBox3.Controls.Add((Control) this.label5);
      this.groupBox3.Controls.Add((Control) this.textBox7);
      this.groupBox3.Controls.Add((Control) this.label6);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.TabStop = false;
      componentResourceManager.ApplyResources((object) this.textBox6, "textBox6");
      this.textBox6.Name = "textBox6";
      componentResourceManager.ApplyResources((object) this.label11, "label11");
      this.label11.Name = "label11";
      componentResourceManager.ApplyResources((object) this.label5, "label5");
      this.label5.Name = "label5";
      componentResourceManager.ApplyResources((object) this.textBox7, "textBox7");
      this.textBox7.Name = "textBox7";
      componentResourceManager.ApplyResources((object) this.label6, "label6");
      this.label6.Name = "label6";
      componentResourceManager.ApplyResources((object) this.groupBox2, "groupBox2");
      this.groupBox2.Controls.Add((Control) this.textBox4);
      this.groupBox2.Controls.Add((Control) this.label10);
      this.groupBox2.Controls.Add((Control) this.label3);
      this.groupBox2.Controls.Add((Control) this.textBox5);
      this.groupBox2.Controls.Add((Control) this.label4);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.TabStop = false;
      componentResourceManager.ApplyResources((object) this.textBox4, "textBox4");
      this.textBox4.Name = "textBox4";
      componentResourceManager.ApplyResources((object) this.label10, "label10");
      this.label10.Name = "label10";
      componentResourceManager.ApplyResources((object) this.label3, "label3");
      this.label3.Name = "label3";
      componentResourceManager.ApplyResources((object) this.textBox5, "textBox5");
      this.textBox5.Name = "textBox5";
      componentResourceManager.ApplyResources((object) this.label4, "label4");
      this.label4.Name = "label4";
      this.spSerialPort.BaudRate = 115200;
      this.spSerialPort.DataReceived += new SerialDataReceivedEventHandler(this.SerialPort_DataReceived);
      this.panel1.Controls.Add((Control) this.splitContainer1);
      componentResourceManager.ApplyResources((object) this.panel1, "panel1");
      this.panel1.Name = "panel1";
      componentResourceManager.ApplyResources((object) this.splitContainer1, "splitContainer1");
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.splitContainer2);
      this.splitContainer1.Panel2.Controls.Add((Control) this.splitContainer4);
      componentResourceManager.ApplyResources((object) this.splitContainer2, "splitContainer2");
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Panel1.Controls.Add((Control) this.textBoxTextInfo);
      this.splitContainer2.Panel2.Controls.Add((Control) this.splitContainer3);
      componentResourceManager.ApplyResources((object) this.textBoxTextInfo, "textBoxTextInfo");
      this.textBoxTextInfo.HideSelection = false;
      this.textBoxTextInfo.Name = "textBoxTextInfo";
      this.textBoxTextInfo.ReadOnly = true;
      componentResourceManager.ApplyResources((object) this.splitContainer3, "splitContainer3");
      this.splitContainer3.Name = "splitContainer3";
      this.splitContainer3.Panel1.Controls.Add((Control) this.pictureBox1);
      this.splitContainer3.Panel2.Controls.Add((Control) this.linkLabel1);
      componentResourceManager.ApplyResources((object) this.pictureBox1, "pictureBox1");
      this.pictureBox1.Image = (Image) MiniIMU.Properties.Resources.x;
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.TabStop = false;
      this.pictureBox1.Click += new EventHandler(this.toolStripButton1_Click);
      componentResourceManager.ApplyResources((object) this.linkLabel1, "linkLabel1");
      this.linkLabel1.Name = "linkLabel1";
      this.linkLabel1.TabStop = true;
      componentResourceManager.ApplyResources((object) this.splitContainer4, "splitContainer4");
      this.splitContainer4.Name = "splitContainer4";
      this.splitContainer4.Panel1.Controls.Add((Control) this.tabControlChart);
      componentResourceManager.ApplyResources((object) this.statusStrip1, "statusStrip1");
      this.statusStrip1.Name = "statusStrip1";
      this.Status.Name = "Status";
      componentResourceManager.ApplyResources((object) this.Status, "Status");
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
      componentResourceManager.ApplyResources((object) this.toolStripButton1, "toolStripButton1");
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
      componentResourceManager.ApplyResources((object) this, "$this");
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.statusStrip1);
      this.Controls.Add((Control) this.panel1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (Form1);
      this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new EventHandler(this.Form1_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.tabControlChart.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.chart1.EndInit();
      this.tabPage3.ResumeLayout(false);
      this.chart2.EndInit();
      this.tabPage2.ResumeLayout(false);
      this.chart3.EndInit();
      this.tabPage4.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((ISupportInitialize) this.splitContainer1).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel1.PerformLayout();
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((ISupportInitialize) this.splitContainer2).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.splitContainer3.Panel1.ResumeLayout(false);
      this.splitContainer3.Panel2.ResumeLayout(false);
      this.splitContainer3.Panel2.PerformLayout();
      ((ISupportInitialize) this.splitContainer3).EndInit();
      this.splitContainer3.ResumeLayout(false);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.splitContainer4.Panel1.ResumeLayout(false);
      ((ISupportInitialize) this.splitContainer4).EndInit();
      this.splitContainer4.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private delegate void UpdateData(byte[] byteData);
  }
}
