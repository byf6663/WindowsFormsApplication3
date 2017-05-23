using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(txt_ip.Text);
            IPEndPoint point = new IPEndPoint(ip, int.Parse(txt_port.Text));

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Bind(point);
                socket.Listen(10);
                ShowMsg("服务器开始监听");
                Thread thread = new Thread(AcceptInfo);
                thread.IsBackground = true;
                thread.Start(socket);
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

        Dictionary<string, Socket> dic = new Dictionary<string, Socket>();

        void AcceptInfo(object o)
        {
            Socket socket = o as Socket;
            while (true)
            {
                try
                {
                    Socket tSocket = socket.Accept();
                    string point = tSocket.RemoteEndPoint.ToString();
                    ShowMsg(point + "连接成功！");
                    cboIpPort.Items.Add(point);
                    dic.Add(point, tSocket);

                    Thread th = new Thread(ReceiveMsg);
                    th.IsBackground = true;
                    th.Start(tSocket);

                }
                catch (Exception ex)
                {

                    ShowMsg(ex.Message);
                    break;
                }
            }
        }
        void ReceiveMsg(object o)
        {
            Socket client = o as Socket;
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int n = client.Receive(buffer);
                    string words = Encoding.UTF8.GetString(buffer, 0, n);
                    ShowMsg(client.RemoteEndPoint.ToString() + ":" + words);
                }
                catch (Exception ex)
                {

                    ShowMsg(ex.Message);
                    break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ShowMsg(txtMsg.Text);
                string ip = cboIpPort.Text;
                byte[] buffer = Encoding.UTF8.GetBytes(txtMsg.Text);
                dic[ip].Send(buffer);

            }
            catch (Exception ex)
            {

                ShowMsg(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }
    }
}
