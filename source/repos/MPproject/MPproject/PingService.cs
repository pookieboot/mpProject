
using Newtonsoft.Json.Linq;

namespace SyncplayMediaPlayer
{
    public class PingService
    {
        private Thread pingThread;
        private NetworkManager networkManager;

        public PingService(NetworkManager networkManager)
        {
            this.networkManager = networkManager;
        }

        public void StartPing()
        {
            pingThread = new Thread(Ping);
            pingThread.Start();
        }

        private void Ping()
        {
            try
            {
                while (true)
                {
                    var pingMessage = new JObject { ["ping"] = new JObject() };
                    networkManager.SendMessage(pingMessage.ToString());
                    Thread.Sleep(4000); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ping thread: {ex.Message}");
            }
        }

        public void StopPing()
        {
            if (pingThread != null && pingThread.IsAlive)
            {
                pingThread.Abort();
            }
        }
    }
}

