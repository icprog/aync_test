using System;
using System.Net.Http;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;

namespace aync_test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            this.spinnerWait.Visibility = Visibility.Visible;
            this.spinnerWait.Spin = true;
            this.button1.IsEnabled = false;
            this.button2.IsEnabled = false;
            this.lblStatus.Content = "Thread is running";
            Task t = Task.Factory.StartNew(
                () =>
                {
                    Thread.Sleep(5000);
                }
            ).ContinueWith(antecednet =>
            {
                this.lblStatus.Content = "Thread stopped.";
                this.spinnerWait.Visibility = Visibility.Hidden;
                this.spinnerWait.Spin = false;
                this.button1.IsEnabled = true;
                this.button2.IsEnabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            DateTime time = await GetStopTime();
            this.lblStatus.Content = $"Task stopped at time {time.ToLocalTime()}";

            this.spinnerWait.Spin = false;
            this.spinnerWait.Visibility = Visibility.Collapsed;
            this.button.IsEnabled = true;
            this.button2.IsEnabled = true;
        }

        private async void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            var length = await GetMsdnStringLth();
            this.lblStatus.Content = $"Task finished. MSDN page character length is {length}";

            this.spinnerWait.Spin = false;
            this.spinnerWait.Visibility = Visibility.Collapsed;
            this.button.IsEnabled = true;
            this.button1.IsEnabled = true;
        }

        private async Task<DateTime> GetStopTime()
        {

            Task<DateTime> task = new Task<DateTime>(() =>
            {
                //Task.Delay(5000);
                Thread.Sleep(5000);
                return DateTime.Now;
            });

            this.lblStatus.Content = "Waiting for async task to finish...";
            this.spinnerWait.Spin = true;
            this.spinnerWait.Visibility = Visibility.Visible;
            this.button.IsEnabled = false;
            this.button2.IsEnabled = false;

            task.Start();
            DateTime time = await task.ConfigureAwait(false);
            return time;
        }

        private async Task<int> GetMsdnStringLth()
        {
            this.lblStatus.Content = "Waiting for async task to finish...";
            this.spinnerWait.Spin = true;
            this.spinnerWait.Visibility = Visibility.Visible;
            this.button.IsEnabled = false;
            this.button1.IsEnabled = false;

            Task<string> getStringTask = new HttpClient().GetStringAsync("http://msdn.microsoft.com");
            string urlContents = await getStringTask;
            return urlContents.Length;
        }

     
    }
}
