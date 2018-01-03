using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ClientMess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1.Click += delegate
            {
                button1.Enabled = false;
                //MyClass mc = new MyClass();
                AsyncServer aS = new AsyncServer(2201);

                //mc.eventFromMyClass += delegate(object sender, MyEventArgs e)
                aS.eventFromMyClass += delegate(object sender, MyEventArgs e)
                {
                    textBox1.Invoke((Action)delegate
                    {
                        textBox1.AppendText(e.Message);
                        textBox1.AppendText(Environment.NewLine);
                    });
                };
                aS.newClientFromMyClass += delegate(object sender, NewClient e)
                {
                    listBox1.Invoke((Action)delegate
                    {
                        if (e.Message == "#CLEAR_ITEMS") listBox1.Items.Clear();
                        else
                            listBox1.Items.Add(e.Message);
                    });
                };

                //mc.serverIP = comboBox1.Text;

                Task.Factory.StartNew((Action)delegate
                    {
                        //mc.Run();
                        aS.Start();
                    });
                
                //mc.lol = true;
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string line;
            string path2file = Environment.CurrentDirectory + @"\ip_list.txt";
            comboBox1.Items.Clear();
            listBox1.Items.Clear();
            System.IO.StreamReader file =
        new System.IO.StreamReader(path2file);
            if (File.Exists(path2file))
                while ((line = file.ReadLine()) != null)
                {
                    comboBox1.Items.Add(line);
                }
            else
            {
                File.Create(path2file);
            }
            file.Close();
        }
    }
}
