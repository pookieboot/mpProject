using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;

namespace SyncplayMediaPlayer
{
    public partial class MainWindow : Window
    {
        private bool isPlaying = false;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Connected successfully!", "Connection", MessageBoxButton.OK, MessageBoxImage.Information);
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = true;
            RewindButton.IsEnabled = true;
            ProgressBar.IsEnabled = true;
            new Thread(ListenForMessages).Start();
        }

        private void SendMessage(string message)
        { 
            Console.WriteLine("Sent: " + message);
        }

        private void ListenForMessages()
        {

            while (true)
            { 
                string response = @"{""state"": ""playing""}";
                Console.WriteLine("Received: " + response);
                Thread.Sleep(5000);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var playMessage = new JObject { ["Set"] = new JObject { ["state"] = "playing" } };
                SendMessage(playMessage.ToString());
                Dispatcher.Invoke(() =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Filter = "Media Files|*.mp4;*.mp3;*.avi;*.mkv|All Files|*.*"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        MediaPlayer.Source = new Uri(openFileDialog.FileName);
                        MediaPlayer.Play();
                        timer.Start();
                        isPlaying = true;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while playing media: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pauseMessage = new JObject { ["Set"] = new JObject { ["state"] = "paused" } };
                SendMessage(pauseMessage.ToString());
                Dispatcher.Invoke(() =>
                {
                    MediaPlayer.Pause();
                    timer.Stop();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while pausing media: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    MediaPlayer.Position = TimeSpan.Zero;
                    ProgressBar.Value = 0;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while rewinding media: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.Volume = VolumeSlider.Value;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                ProgressBar.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                ProgressBar.Value = MediaPlayer.Position.TotalSeconds;
            }
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(MediaPlayer.Position.TotalSeconds - ProgressBar.Value) > 1)
            {
                MediaPlayer.Position = TimeSpan.FromSeconds(ProgressBar.Value);
            }
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                ProgressBar.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Stop();
            timer.Stop();
            ProgressBar.Value = 0;
            isPlaying = false;
        }
    }
}




