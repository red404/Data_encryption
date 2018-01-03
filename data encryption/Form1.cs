using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

using System.Net.Sockets;
using System.Net;

namespace data_encryption
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button3.Click += delegate
            {
                this.WindowState = FormWindowState.Maximized;
                textBox7.Enabled = false;
                progressBar2.Value = 0;
                progressBar1.Value = 0;
                Transfer mc = new Transfer(); // экземпляр класса, запускаемый в дочернем потоке, 
                // чтобы избежать зависания интерфейса во время вычислений
                mc.eventFromMyClass += delegate(object sender, MyEventArgs e)
                {
                    textBox3.Invoke((Action)delegate
                    {
                        if (textBox3.TextLength > 32000) textBox3.Text = "";
                        // лог
                        textBox3.AppendText(e.Message + Environment.NewLine);
                    });
                };

                mc.progressEventFromMyClass += delegate(object sender, ProgrEventArgs e)
                {
                    progressBar1.Invoke((Action)delegate
                    {
                        // отображаем текущий прогресс синхронизации

                        if (progressBar1.Value < e.Val)
                        {
                            progressBar1.Value = e.Val;
                            label6.Text = "Текущий прогресс: " + e.Val.ToString() + "%";
                        }
                    });
                    progressBar2.Invoke((Action)delegate
                    {
                        // отображаем общий прогресс синхронизации
                        /*if (progressBar2.Value < e.Val)
                        {
                            progressBar2.Value = e.Val;
                            label7.Text = "Лучший прогресс: " + e.Val.ToString() + "%";
                            this.Text = "Прогресс синхронизации: " + e.Val.ToString() + "%";
                        }*/
                        label7.Text = "Расчётный прогресс: " + e.CalcVal.ToString() + "%";
                        progressBar2.Value = e.CalcVal > 99 ? 100 : e.CalcVal;
                        this.Text = "Расчётный прогресс синхронизации: " + e.CalcVal.ToString() + "%";
                    });
                };

                button4.Click += delegate
                {
                    // приостанавливаем синхронизацию
                    mc.lol = 1;
                    button5.Visible = true;
                    button4.Visible = false;
                };
                button5.Click += delegate
                {
                    // продолжаем синхронизацию
                    mc.lol = 0;
                    button4.Visible = true;
                    button5.Visible = false;
                };
                button6.Click += delegate
                {
                    textBox7.Enabled = true;
                    // останавливаем синхронизацию
                    mc.lol = 2;
                };

                Task.Factory.StartNew((Action)delegate
                {
                    // начинаем синхронизацию в новом потоке
                    mc.Run(textBox4.Text, textBox5.Text, textBox6.Text, textBox1.Text, checkBox1, progressBar1, textBox7.Text);
                    // сообщение для наблюдателя
                    if (Convert.ToInt32(textBox6.Text) > 1)
                        MessageBox.Show("Заметьте, что теперь 8 совпадений недостаточно для синхронизации. Если \"поиграть\" с параметрами, то можно заметить, что амплитуда более всего влияет на скорость синхронизации");
                });
            };
        }


        private void button1_Click(object sender, EventArgs e)
        {/*
            openFileDialog1.FileName = textBox1.Text;
            if (!openFileDialog1.CheckFileExists) MessageBox.Show("Несуществующий файл");
            else
            {
                MessageBox.Show("Файл " + openFileDialog1.SafeFileName);
                var cryptogramm = data_encryption(textBox1.Text.ToString());
            }*/
        }

        private string data_encryption(string h)
        {
            string f="";
            return f;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {   
            // сообщение для наблюдателя 
            if (textBox1.Text != "") button1.Enabled = true; else button1.Enabled = false;
            /*if ((Convert.ToInt32(textBox1.Text)>8)&&(Convert.ToInt32(textBox6.Text)>1))
            MessageBox.Show("С подобными параметрами программа вероятнее всего \"подвиснет\" на некоторое время, т.к. вычислений становится много больше, по сравнению с более мелкой амплитудой или точностью синхронизации.");
        */
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            options f = new options();
            f.ShowDialog();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            /*
            int count = 0;
            ArtNeuronNetwork tpm1 = new ArtNeuronNetwork(Convert.ToInt32(textBox4.Text), Convert.ToInt32(textBox5.Text), Convert.ToInt32(textBox6.Text));
            ArtNeuronNetwork tpm2 = new ArtNeuronNetwork(Convert.ToInt32(textBox4.Text), Convert.ToInt32(textBox5.Text), Convert.ToInt32(textBox6.Text));
            synchronization(tpm1, tpm2, out count);
            MessageBox.Show("Сети синхронизированы!");
            ShowWeights(tpm1, tpm2, count);

            if (Convert.ToInt32(textBox6.Text) > 1)
                MessageBox.Show("Заметьте, что теперь 8 совпадений недостаточно для синхронизации. Если \"поиграть\" с параметрами, то можно заметить, что амплитуда более всего влияет на скорость синхронизации");
            */
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            // сообщение для наблюдателя 
            int i;
            if ( (!Int32.TryParse(textBox4.Text, out i)) || (!Int32.TryParse(textBox5.Text, out i)) || (!Int32.TryParse(textBox6.Text, out i)) || (!Int32.TryParse(textBox1.Text, out i)) )
            {
                MessageBox.Show("Просьба вводить только целочисленные данные в контролы параметров, защита ввода ещё не организована");
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            textBox3.Width = Convert.ToInt32(Math.Round(0.95 * this.Width));
        }


        private void button7_Click(object sender, EventArgs e)
        {
            // очистка поля лога
            textBox3.Text = "";
        }


        private void Form1_Activated(object sender, EventArgs e)
        {
            if (this.progressBar2.Value > 99) textBox7.Enabled = true;

        }


        public string IP = "127.0.0.1";
        public string Port = "2201";
        public Socket socket;
        public EndPoint end;

        private void отправитьСерверуСообщениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // инициализация сокета
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // создание параметра для подключения к серверу
            IPAddress ip = IPAddress.Parse(IP);
            IPEndPoint ipe = new IPEndPoint(ip, int.Parse(Port));
            end = (EndPoint)ipe;
            try
            {
                socket.Connect(ipe);
                this.Text += " - Соединение установлено";
            }
            catch // на случай каких-либо проблем
            {
                MessageBox.Show("Проблемы с установкой соединения.\nПриложение будет закрыто.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
        // отправка сообщения

        private delegate string Answer();

        // метод для получения ответа
        private string Answ() 
        {
            byte[] answer=new byte[64];
            //socket.ReceiveFrom(answer, 0, ref end);
            socket.Receive(answer, 0);
            return Encoding.Default.GetString(answer);
        }

        private void sendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(textBox2.Text);
            string str = "";
            for (int i = 3; i > 0; i--)
            {
                if ((id / (int)Math.Pow(10, i)) > 0) break;
                str += "0";
            }

            //int threadId;
            MessageBox.Show(str);
            str += id.ToString() + textBox1.Text;
            byte[] buffer = Encoding.Default.GetBytes(str);
            socket.Send(buffer, buffer.Length, 0);

            str = "";
            // ожидание ответа от сервера
            //new Answer(delegate() {str = Answ(); }).BeginInvoke(null, null);
            IAsyncResult res = textBox1.BeginInvoke(new Answer(Answ), null);
            str = (string)textBox1.EndInvoke(res);
            textBox1.Text = str;
            if (!socket.Connected) this.Text.Replace(" - Соединение установлено", null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Поехали");
            Visible = false;
            Form2 f = new Form2();
            f.ShowDialog();
            Close();
        }
    }
}
