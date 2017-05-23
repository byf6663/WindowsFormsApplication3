using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private void button1_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(txtIP.Text);
            IPEndPoint point = new IPEndPoint(ip, int.Parse(txtPort.Text));
            try
            {
                client.Connect(point);
                ShowMsg("连接成功");
                ShowMsg("服务器" + client.RemoteEndPoint.ToString());
                ShowMsg("客户端:" + client.LocalEndPoint.ToString());
                Thread th = new Thread(ReceiveMsg);
                th.IsBackground = true;
                th.Start();

            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
        }
        private void ShowMsg(string msg)
        {
            txtLog.AppendText(msg + "\r\n");
        }
        void ReceiveMsg()
        {
            List<byte> byteSource = new List<byte>();
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int n = client.Receive(buffer,buffer.Length, SocketFlags.None);
                    
                    if(n == 5 && Encoding.UTF8.GetString(buffer, 0, n) == "start" && byteSource.Count >=1) {
                        
                        byte[] data = byteSource.ToArray();
                        byteSource.Clear();
                        pictureBox1.Image = BytesToImage(data);
                        
                    }
                    if (n == 5)
                    {
                        ShowMsg(Encoding.UTF8.GetString(buffer, 0, n));

                    }else
                    {
                        byte[] tmp = new byte[n];
                        Array.Copy(buffer, 0, tmp, 0, n);
                        byteSource.AddRange(tmp);
                    }

                    Console.Write("数量");
                    Console.Write(n);
                   
                    //pictureBox1.Image = BytesToImage(buffer);
                    //FileStream fs = File.Create("1.jpg");
                    //fs.Write(buffer, 0, n);
                    //fs.Flush();
                    //fs.Close();
                    //Console.Write("完成");
                    //string s = Encoding.UTF8.GetString(buffer, 0, n);

                    ShowMsg(client.RemoteEndPoint.ToString() + ":" + "完成:"+ n.ToString());

                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message);
                    break;
                }
            }
        }
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //客户端给服务器发消息

            if (client != null)

            {

                try

                {

                    ShowMsg(txtMsg.Text);

                    byte[] buffer = Encoding.UTF8.GetBytes(txtMsg.Text);

                    client.Send(buffer);

                }

                catch (Exception ex)

                {

                    ShowMsg(ex.Message);

                }

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Close();
        }
    }
}
