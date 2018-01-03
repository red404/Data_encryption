using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;


namespace Messaging
{
    class Program
    {
        static void Main(string[] args)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            
            Byte[] message = encoding.GetBytes("Ya nemnogo zanyat, prihodite popozhe");

            try
            {
                //IPAddress localAddress = IPAddress.Parse("127.0.0.1");
                IPAddress localAddress = IPAddress.Parse("192.168.0.41");
                //IPAddress localAddress = IPAddress.Parse("ivan-andreyev.zapto.org");
                //IPAddress localAddress = IPAddress.Parse("136.169.244.1");

                TcpListener listener = new TcpListener(localAddress, 2200);

                listener.Start(1);

                while (true)
                {
                    Console.WriteLine("Сервер ожидает {0}", listener.LocalEndpoint);
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream io = client.GetStream();

                    Console.WriteLine("Принято соединение от {0}", client.Client.RemoteEndPoint);

                    Console.WriteLine("Отправляем сообщение...");
                    //byte[] r = Encoding.Convert(Encoding.Unicode, Encoding.Default, message);
                    io.Write(message, 0, message.Length);
                    //io.Write(r, 0, r.Length);


                    Console.WriteLine("Закрытие соединения");
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Произошла ошибка {0}", e.Message);
            }
        }
    }
}
