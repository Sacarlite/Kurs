using ServerBroadcast;
using System.Net;
using System.Net.Sockets;
using System.Text;
using AproximationFunction;

namespace AproximateServer
{
    internal static class Server
    {
        static void Main(string[] args)
        {

            TcpListener server = new TcpListener(IPAddress.Any, 8888);
            server.Start();
            ServerTransfer dataProcessing = new ServerTransfer(); // запускаем сервер
            while (true) // бесконечный цикл обслуживания клиентов
            {
                TcpClient client = server.AcceptTcpClient(); // ожидаем подключение клиента
                var stream = client.GetStream();
                // буфер для входящих данных
                var response = new List<byte>();
                int bytesRead = 10;
                while (true)
                {
                    // считываем данные до конечного символа
                    while ((bytesRead = stream.ReadByte()) != '\n')
                    {
                        // добавляем в буфер
                        response.Add((byte)bytesRead);
                    }

                    var translation = Encoding.UTF8.GetString(response.ToArray());
                    if (translation == "END")
                    {
                        stream.Close();
                        break;
                    }
                    var tmpTyple = dataProcessing.ServerDecoder(translation);
                    Aproximation pointAproximation = new Aproximation(tmpTyple.Item3, tmpTyple.Item1, tmpTyple.Item2);
                    stream.WriteAsync(Encoding.UTF8.GetBytes(dataProcessing.ServerEncoder(pointAproximation.Aproximat()) + "\n"));
                    response.Clear();

                }

            }

        }

    }
}