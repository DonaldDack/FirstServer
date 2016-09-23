using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace client
{
    public partial class Form1 : Form
    {
        static private Socket Client;
        private IPAddress ip = null;
        private int port = 0;
        private Thread th;

        public Form1()
        {
            InitializeComponent();

            richTextBox1.Enabled = false;
            richTextBox2.Enabled = false;
            button3.Enabled = false;

            try
            {
                var sr = new StreamReader(@"Client_info/data_info.txt");
                string buffer = sr.ReadToEnd();
                sr.Close();

                string[] connectInfo = buffer.Split(':');
                ip = IPAddress.Parse(connectInfo[0]);
                port = int.Parse(connectInfo[1]);

                label4.ForeColor = Color.Green;
                label4.Text = "Настройки: \n IP сервера: " + connectInfo[0] + "\n Порт сервера: " + connectInfo[1];
            }
            catch(Exception ex)
            {
                label4.ForeColor = Color.Red;
                label4.Text = "Setting not found";
                Form form = new Form2();
                form.Show();
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        void SendMessage(string message)
        {
            if(message != "" && message != " ")
            {
                byte[] buffer = new byte[1024];
                buffer = Encoding.UTF8.GetBytes(message);
                Client.Send(buffer);
            }
        }
        void RecvMessage()
        {
            byte[] buffer = new byte[1024];
            for(int i = 0; i < buffer.Length; ++i)
            {
                buffer[i] = 0;
            }

            for (;;)
            {
                try
                {
                    Client.Receive(buffer);
                    string message = Encoding.UTF8.GetString(buffer);
                    int count = message.IndexOf(";;;5");
                    if (count == -1)
                    { continue; }

                    string clearMessage = "";

                    for (int i = 0; i < count; ++i)
                    {
                        clearMessage += message[i];
                    }

                    for (int i = 0; i < buffer.Length; ++i)
                    {
                        buffer[i] = 0;
                    }
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        richTextBox1.AppendText(clearMessage);
                    }
                    );
                   
                }
                catch(Exception ex) { }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != " " && textBox1.Text != "")
            {
                button3.Enabled = true;
                richTextBox2.Enabled = true;
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (ip != null)
                {
                    Client.Connect(ip, port);
                    th = new Thread(delegate () { RecvMessage(); });
                    th.Start();
                    this.Focus();
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SendMessage("\n" + textBox1.Text + " : " + richTextBox2.Text + ";;;5");
            richTextBox2.Clear();
        }

        private void авторToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("By Dimas\n");
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(th != null)
                th.Abort();
            if (Client != null)
                Client.Close();
            Application.Exit();
        }

        private void richTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                SendMessage("\n" + textBox1.Text + " : " + richTextBox2.Text + ";;;5");
                richTextBox2.Clear();
            }
        }
    }
}
