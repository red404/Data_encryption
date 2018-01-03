using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ClientMess
{

    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class MyClass
    {
        public string serverIP;
        private delegate void RecvR(object num);
        public event EventHandler<MyEventArgs> eventFromMyClass;
        public event EventHandler<NewClient> newClientFromMyClass;

        public string SomeString = "Hello!";
        public bool lol = true;
        //public TcpClient[] clients = new TcpClient[25];
        public Socket[] clients = new Socket[25];

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

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public void Run()
        {
            //StartListening();
        }

        public void Run1()
        {
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                //Byte[] message = encoding.GetBytes("Ya nemnogo zanyat, prihodite popozhe");

                try
                {
                    showM("Начало работы.");
                    IPAddress localAddress = IPAddress.Parse(serverIP);
                    //IPAddress localAddress = IPAddress.Parse("192.168.0.41");
                    //IPAddress localAddress = IPAddress.Parse("ivan-andreyev.zapto.org");
                    //IPAddress localAddress = IPAddress.Parse("94.41.142.143");
                    Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //TcpListener listener = new TcpListener(localAddress, 2201);

                    IPEndPoint ipEndpoint = new IPEndPoint(localAddress, 2201);
                    listenSocket.Bind(ipEndpoint);
                    listenSocket.Listen(100);
                    //listener.Start(1);
                    //while (true)
                    int i = 0;
                    while (i < 25)
                    {
                        allDone.Reset();

                        //showM("Сервер ожидает " + listener.LocalEndpoint);
                        showM("Сервер ожидает " + ipEndpoint);
                        Thread.Sleep(100);
                        //TcpClient client = listener.AcceptTcpClient();
                        //clients[i] = new TcpClient();
                        //clients[i] = listener.AcceptTcpClient();
                        //NetworkStream io = clients[i].Accept();
                        clients[i] = listenSocket;
                        clients[i].BeginAccept(new AsyncCallback(AcceptCallback), listenSocket);
                        showM("Принято соединение от " + clients[i].RemoteEndPoint);

                        //clients[i].BeginInvoke(new RecvR(ReceiveRun), i);

                        //clients[i].BeginReceive(
                        
                            /* синхронный приём */
                            ReceiveRun(i);

                        // запрос сообщения
                        //handler.Send(encoding.GetBytes("Сообщение получено"));
                        //showM("Закрытие соединения");
                        //handler.Close();
                        //clients[i].Close();
                        lol = true;
                        i++;
                    }
                }
                catch (Exception e)
                {
                    showM("Произошла ошибка " + e.Message);
                }
            }
        }


        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    showM("Read " + content.Length + " bytes from socket. " + Environment.NewLine + " Data : " + content);
                    // Echo the data back to the client.
                    sendToClient(handler, content);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        /*
        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }*/

        public void sendToClient(string s, NetworkStream io)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            //Byte[] message = encoding.GetBytes(s);
            Byte[] message = Encoding.Default.GetBytes(s);
            
            showM("Отправляем сообщение...");
            io.Write(message, 0, message.Length);
        }

        public void sendToClient(Socket dest, string s)
        {/*
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] message = encoding.GetBytes(s);
            showM("Отправляем сообщение...");
            dest.Send(message, message.Length, 0);*/

            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(s);

            // Begin sending the data to the remote device.
            dest.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), dest);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                showM(e.ToString());
            }
        }


        public Byte[] inputMess()
        {
            inputMessage f = new inputMessage();
            ASCIIEncoding encoding = new ASCIIEncoding();
            f.ShowDialog();
            Byte[] s = encoding.GetBytes(f.message);
            return s;
        }

        public void set_s(string ns)
        {
            SomeString = ns;
        }

        void ReceiveRun(object num)
        {
            int i = -1;
            
            //NetworkStream stream = clients[(int)num].Receive();
            showM("Запустили приём сообщения");
            //NetworkStream stream = clients[(int)num].Receive();
            string s = null, mess = null;
            // Loop to receive all the data sent by the client.
            //while ((ii = ns.Read(bytes, 0, bytes.Length)) != 0) 
            //byte[] buffer = new byte[clients[(int)num].Available];

            //while ((s != "exit")) // зависает
            //while (((i = clients[(int)num].Receive(buffer)) != 0) && (ns.DataAvailable == true))
            //while ((i = ns.Read(buffer, 0, buffer.Length)) != 0)


                    //NetworkStream ns = new NetworkStream(clients[(int)num]);

                    // Раскомментировав строчку ниже,
                    // тем самым уменьшив размер приемного буфера, можно убедиться,
                    // что прием данных будет все равно осуществляться полностью.
                    // clients[(int)num].ReceiveBufferSize = 2;
                    
                    //while (ns.DataAvailable == true)
                    //while ((s != "exit") && ((i = clients[(int)num].Receive(buffer)) != 0) && (ns.DataAvailable == true))


                    // Определить точный размер буфера приема
                    // позволяет свойство класса TcpClient Available

                    //if (ns.CanRead) { i = ns.Read(buffer, 0, buffer.Length); showM("Число прочитанных байт: " + i.ToString()); }
                    //else showM("Невозможно начать чтение.");
                    // Волшебная функция превращающая поток байтов
                    // в текстовые символы.

            byte[] buffer = new byte[1024];
            mess = s = String.Empty;
            try
            {
                byte[] bytes = new byte[1024];
                //while (
                    clients[i].BeginReceive(bytes, 0, clients[i].Available,
                                   SocketFlags.None, null, null);
            /* через NetworkStream всё работает */
                /*    

                 * NetworkStream ns = new NetworkStream(clients[(int)num]);
                    byte[] buffer = new byte[1024];
                        do
                        {
                            ns.Read(buffer, 0, buffer.Length);
                            s += Encoding.Default.GetString(buffer);
                            showM("Текущее сообщение: " + s);//не отображает даже текущее сообщение
                        }
                        while (ns.DataAvailable == true);
                */
                if ((s != null) && (s != ""))
                    {
                        // Данный метод, хотя и вызывается в отдельном потоке (не в главном),
                        // но находит родительский поток и выполняет
                        // делегат указанный в качестве параметра
                        // в главном потоке, безопасно обновляя интерфейс формы.
                        //Invoke(new UpdateReceiveDisplayDelegate(UpdateReceiveDisplay),
                        //        new object[] { (int)num, s });

                        // Принятое сообщение от клиента перенаправляем всем клиентам
                        // кроме текущего.
                        string id = s;
                        int j = 0;
                        //for (int i = 0; i < 4; i++)
                        j = Convert.ToInt32(id.Remove(4));
                        showC(j.ToString());
                        mess = s.Remove(0, 4);
                        showM("Cообщение от клиента с id = " + j.ToString() + ": " + mess);
                        mess = "#" + ((int)num).ToString() + ": " + mess;
                        // отправить клиенту с идентификатором j сообщение s
                        //    SendToClients(s, j);
                        showM("Подготовленное для клиента сообщение: " + mess);
                        //sendToClient(mess + " : message recd", ns);
                        //sendToClient(mess + " : message recd", clients[(int)num]);
                    }

                    // Пока доступных данных нет притормозим деятельность потока.
                    // Вынужденная строчка для экономия ресурсов процессора.
                    // 100 мс достаточное время остановки потока на каждой итерации цикла
                    // чтобы разгрузить процессор. Для корректного извлечения сообщений это
                    // неизящный способ регламентирования работы потока.
                    Thread.Sleep(100);
                }
                catch
                {
                    // Перехватим возможные исключения
                    //ErrorSound();
                }


                //if (_stopNetwork == true) break;

            //}
            
        }
    }
}
