using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace SyncplayMediaPlayer
{
    public class NetworkManager
    {
        private TcpClient client;
        private NetworkStream stream;

        public void Connect(string serverAddress, int port, string username, string roomName)
        {
            try
            {
                client = new TcpClient(serverAddress, port);
                stream = client.GetStream();

                var helloMessage = new
                {
                    Hello = new
                    {
                        username = username,
                        room = new { name = roomName },
                        version = "1.2.7"
                    }
                };
                SendMessage(JsonConvert.SerializeObject(helloMessage));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to connect: {ex.Message}");
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send message: {ex.Message}");
            }
        }

        public string ReceiveMessage()
        {
            try
            {
                byte[] data = new byte[256];
                int bytes = stream.Read(data, 0, data.Length);
                return Encoding.UTF8.GetString(data, 0, bytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to receive message: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            try
            {
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to disconnect: {ex.Message}");
            }
        }
    }
}

