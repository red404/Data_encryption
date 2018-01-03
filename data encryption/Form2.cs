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

using System.Security.Cryptography;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace data_encryption
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public event EventHandler<MyEventArgs> eventFromMyClass;
        public event EventHandler<MyEventArgs> messageFromMyClass;
        public event EventHandler<NewClient> newClientFromMyClass;
        public int id = 1;
        public string Port;
        public Socket socket;
        public EndPoint end;
        public string login;
        public string pass;
        private string SomeString = "Hello!";
        public byte[] answer = new byte[256];
        object state = new ConnectionState();
        string str;
        string cur_serv = "";
        public ConnectionInfo connection = new ConnectionInfo();

        public class ConnectionInfo
        {
            public Socket Socket;
            public byte[] Buffer;
            public int id;
        }

        public void showM(string s)
        {
            Thread.Sleep(2);
            set_s(s);
            Task.Factory.StartNew((Action)delegate
            {
                if (eventFromMyClass != null)
                {
                    eventFromMyClass(this, new MyEventArgs(SomeString));
                }
            });
        }

        
        public void showMes(string s)
        {
            Thread.Sleep(2);
            set_s(s);
            Task.Factory.StartNew((Action)delegate
            {
                if (messageFromMyClass != null)
                {
                    messageFromMyClass(this, new MyEventArgs(SomeString));
                }
            });
        }

        public void showC(string s)
        {
            Thread.Sleep(2);
            Task.Factory.StartNew((Action)delegate
            {
                if (newClientFromMyClass != null)
                {
                    newClientFromMyClass(this, new NewClient(s));
                }
            });
        }

        public void set_s(string ns)
        {
            SomeString = ns;
        }


        public void connect(string IP)
        {
            //ConnectionInfo connection = new ConnectionInfo();
            // инициализация сокета
            //socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connection.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // создание параметра для подключения к серверу
            IPAddress ip; str = "";
            if (IPAddress.TryParse(IP, out ip))
            {
                IPEndPoint ipe = new IPEndPoint(ip, int.Parse(Port));
                end = (EndPoint)ipe;

                try
                {
                    //socket.Connect(ipe);
                    connection.Socket.Connect(ipe);
                    this.Text += " - Соединение c " + ipe.ToString() + " установлено";
                    //button2.Enabled = button4.Enabled = true;

                    newClientFromMyClass += delegate(object sender, NewClient e)
                    {
                        listBox2.Invoke((Action)delegate
                        {
                            if (e.Message == "#CLEAR_ITEMS") listBox2.Items.Clear();
                            else
                                listBox2.Items.Add(e.Message);
                        });
                    };
                    
                    eventFromMyClass += delegate(object sender, MyEventArgs e)
                    {
                        textBox1.Invoke((Action)delegate
                        {
                            textBox1.AppendText(e.Message);
                            textBox1.AppendText(Environment.NewLine);
                        });
                    };
                    connection.Socket.BeginReceive(answer,
                        0, answer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback),
                        connection);
                }
                catch // на случай каких-либо проблем
                {
                    MessageBox.Show("Проблемы с установкой соединения.\nВыберите другой сервер.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //button1.Enabled = true;
                    //Application.Exit();
                }
            }
            else
            {
                try
                {
                    connection.Socket.Connect(IP, int.Parse(Port));
                    this.Text += " - Соединение c " + IP + ":" + Port + " установлено";
                    //button2.Enabled = button4.Enabled = true;
                    eventFromMyClass += delegate(object sender, MyEventArgs e)
                    {
                        textBox1.Invoke((Action)delegate
                        {
                            textBox1.AppendText(e.Message);
                            textBox1.AppendText(Environment.NewLine);
                        });
                    };
                    newClientFromMyClass += delegate(object sender, NewClient e)
                    {
                        listBox2.Invoke((Action)delegate
                        {
                            if (e.Message == "#CLEAR_ITEMS") listBox2.Items.Clear();
                            else
                                listBox2.Items.Add(e.Message);
                        });
                    };
                    connection.Socket.BeginReceive(answer,
                        0, answer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback),
                        connection);
                    //showM(Environment.NewLine + str);
                }
                catch // на случай каких-либо проблем
                {
                    MessageBox.Show("Проблемы с установкой соединения.\nВыберите другой сервер.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //button1.Enabled = true;
                    
                    //Application.Exit();
                }
            }
        }

        private void подключитьсяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connect(listBox1.SelectedItem.ToString());
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            cur_serv = listBox1.SelectedItem.ToString();
            connect(cur_serv);
            string sstr = id.ToString("D4");
            sstr += "auth";
            sstr += login.PadLeft(15);
            sstr += pass.PadLeft(15);
            Send2Serv(connection, sstr);
            button1.Enabled = false;
        }

        // отправка сообщения


        public void button2_Click(object sender, DialogEventArgs e)
        {
            string sstr = "";
            sstr += id.ToString("D4");
            sstr +="mess"; 
            sstr += e.mess;
            Send2Serv(connection, sstr);
            sstr = "";
            if (!connection.Socket.Connected) this.Text.Replace(" - Соединение установлено", null);
        }
        
        private void ReceiveCallback(IAsyncResult result)
        {
            //ConnectionInfo connection = result.AsyncState;
            ConnectionInfo conn = (ConnectionInfo)result.AsyncState;
            //str = "";
            try
            {

                //int bytesRead = socket.EndReceive(result);
                int bytesRead = conn.Socket.EndReceive(result);
                if (0 != bytesRead)
                {
                    str = "";
                    string from = Encoding.ASCII.GetString(answer, 0, 4);
                    string code = Encoding.ASCII.GetString(answer, 4, 4);
                    //if (code == "err_")
                    str += Encoding.ASCII.GetString(answer, 8, bytesRead - 8);
                        //MessageBox.Show("Message Received: " + str);
                    //showM("Message Received in ReceiveCallback: " + str);
                    switch (code)
                    {
                        case "mess":
                           //if (from!="serv")
                            using (Rijndael myRijndael = Rijndael.Create())
                            {
                                str = DecryptStringFromBytes(answer, myRijndael.Key, myRijndael.IV);
                            }
                            showMes("From " + from + ": " + str);
                            break;
                        case "sync":
                            /* some sync actions */
                            break;
                        case "err_":
                            /* some actions to fix error */
                            if(str == "wrong_name"){
                                Authoriz f = new Authoriz(this);
                                f.ShowDialog(this);
                                string msg = id.ToString("D4");
                                msg += "regi";
                                msg += login.PadLeft(15);
                                msg += pass.PadLeft(15);
                                connect(cur_serv);
                                Send2Serv(conn, msg);
                            }
                            else if(str=="wrong_pass"){
                                MessageBox.Show("Неправильный пароль или логин. Исправьте пароль или логин в настройках и повторите попытку");
                                Authoriz f = new Authoriz(this);
                                f.ShowDialog(this);
                            }
                            break;
                        case "list":
                            /* Fill clients list */
                            //listBox2.Items.Clear();
                            showC("#CLEAR_ITEMS");
                            showM("This is str: " + str);
                            string[] arrs = str.Trim().Split('#', ' ');
                            foreach (string s in arrs)
                            {
                                //MessageBox.Show(s);
                                //listBox2.Items.Add(s);
                                showM("Sub str: " + s);
                                showC(s);
                            }

                            break;
                    }
                }
                else
                {
                    //str = "";
                    //textBox1.Text += Environment.NewLine + str;
                    //showM(str);
                    //socket.Close();
                }
                connection.Socket.BeginReceive(answer,
                        0, answer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback),
                        connection);
            }
            catch (SocketException exc)
            {
                //CloseConnection(connection);
                MessageBox.Show("Socket exception: " +
                    exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                //CloseConnection(connection);
                MessageBox.Show("Exception: " + exc);
            }
        }

        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }


        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }


        private void Send2Serv(ConnectionInfo ci, string sstr)
        {byte[] buffer;
        if (sstr.Remove(4) != "mess") { buffer = Encoding.Default.GetBytes(sstr); ci.Socket.Send(buffer, buffer.Length, 0); }
        else using (Rijndael myRijndael = Rijndael.Create())
            {
                // Encrypt the string to an array of bytes.
                buffer = EncryptStringToBytes(sstr, myRijndael.Key, myRijndael.IV);
                ci.Socket.Send(buffer, buffer.Length, 0);
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (connection.Socket!=null)
            if(connection.Socket.Connected)
            {
                get_clients_list();
            }
            else
            {
                get_servers_list();
                if (listBox1.Items.Count > 0)
                {
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    button1.Enabled = true;
                }
                else button1.Enabled = false;
            }
        }

        private void get_clients_list()
        {
            //socket.Send
            string sstr = "";
            sstr += id.ToString("D4");
            sstr += "getl";
            Send2Serv(connection, sstr);
        }
        private void get_servers_list()
        {
            listBox1.Items.Clear();
            /* Получаем список доступных серверов */

            string line;
            string path2file = Environment.CurrentDirectory + @"\ip_list.txt";
            MessageBox.Show(path2file);
            System.IO.StreamReader file =
        new System.IO.StreamReader(path2file);
            string[] str = new string[20];
            int counter = 0;
            if (File.Exists(path2file))
                while ((line = file.ReadLine()) != null)
                {
                    str[counter] = line;
                }
            else
            {
                File.Create(path2file);
            }
            file.Close();

            string[] Servers = str;//{ "49538.dyn.orsk.ufanet.ru" };
            Ping pingSender = new Ping();
            IPAddress resultIP;

            for (int i = 0; i < Servers.Length; i++)
            {
                if (IPAddress.TryParse(Servers[i], out resultIP))
                {
                    IPAddress address = resultIP;
                    PingReply reply = pingSender.Send(address);
                    if (reply.Status == IPStatus.Success)
                    {
                        listBox1.Items.Add(Servers[i]);
                    }
                    else
                    {
                        MessageBox.Show(reply.Status.ToString());
                    }
                }
                else
                {
                    string address = Servers[i];
                    PingReply reply = pingSender.Send(address);
                    if (reply.Status == IPStatus.Success)
                    {
                        listBox1.Items.Add(Servers[i]);
                    }
                    else
                    {
                        MessageBox.Show(reply.Status.ToString());
                    }
                }
            }
        }

        string[] lines = new string[20];

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadOptions();


            get_servers_list();
            if (listBox1.Items.Count > 0) listBox1.SelectedIndex = listBox1.Items.Count - 1;
            else button1.Enabled = false;
        }

        private void LoadOptions()
        {
            //lines = null;
            int counter = 0;

            System.IO.StreamReader file = new System.IO.StreamReader(@"../../data/options.ini");
            do
            {
                lines[counter] = file.ReadLine();
                if (counter == 3) Port = lines[counter];
                if (counter == 5) login = lines[counter];
                if (counter == 7) pass = lines[counter];
                counter++;
            }
            while (lines[counter - 1] != null);
            file.Close();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            options f = new options();
            f.ShowDialog();
            LoadOptions();
        }

        private void отключитьсяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            button1.Enabled = true;
            connection.Socket.Disconnect(false);
            connection.Socket.Close();
            connection.Socket.Dispose();
        }

        private void listBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            id = Convert.ToInt32(listBox2.Items[listBox2.SelectedIndex]);
        }

        private void информацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            id = Convert.ToInt32(listBox2.Items[listBox2.SelectedIndex]);
        }

        private void диалогСПользователемToolStripMenuItem_Click(object sender, EventArgs e)
        {
            id = Convert.ToInt32(listBox2.Items[listBox2.SelectedIndex]);
            Dialog f = new Dialog(this);
            f.ShowDialog();
        }
    }
}
