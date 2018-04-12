using System;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace aync_test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _remainingTime = -1;
        private Task _countDown = null;
        private DispatcherTimer _timer;
        private CancellationTokenSource _cts;
        private event EventHandler<string> NewLog;

        public MainWindow()
        {
            InitializeComponent();
            NewLog += MainWindow_NewLog;
        }

        private void MainWindow_NewLog(object sender, string e)
        {
            AddLog(e);
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            SetBusy(true);
            this.button1.IsEnabled = false;
            this.button2.IsEnabled = false;
            this.lblStatus.Content = "Thread is running. Using Task.Delay";
            Task t = Task.Delay(5000).ContinueWith(antecednet =>
            {
                this.lblStatus.Content = "Thread stopped.";
                SetBusy(false);
                this.button1.IsEnabled = true;
                this.button2.IsEnabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SetBusy(bool isBusy)
        {
            this.spinnerWait.Visibility = isBusy ? Visibility.Visible : Visibility.Hidden;
            this.spinnerWait.Spin = isBusy;
        }

        private async void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            var task = GetStopTime();
            
            DateTime time = await GetStopTime();
            this.lblStatus.Content = $"Task stopped at time {time.ToLocalTime()}";

            SetBusy(false);
            this.button.IsEnabled = true;
            this.button2.IsEnabled = true;
        }

        private async void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            var length = await GetMsdnStringLth();
            this.lblStatus.Content = $"Task finished. MSDN page character length is {length}";

            SetBusy(false);
            this.button.IsEnabled = true;
            this.button1.IsEnabled = true;
        }

        private async Task<DateTime> GetStopTime()
        {

            Task<DateTime> task = new Task<DateTime>(() =>
            {
                Thread.Sleep(5000);
                return DateTime.Now;
            });

            this.lblStatus.Content = "Waiting for async task to finish...";
            SetBusy(false);
            this.button.IsEnabled = false;
            this.button2.IsEnabled = false;

            task.Start();
            DateTime time = await task;
            return time;
        }

        private async Task<int> GetMsdnStringLth()
        {
            this.lblStatus.Content = "Waiting for async task to finish...";
            SetBusy(true);
            this.button.IsEnabled = false;
            this.button1.IsEnabled = false;

            Task<string> getStringTask = new HttpClient().GetStringAsync("http://msdn.microsoft.com");
            string urlContents = await getStringTask;
            return urlContents.Length;
        }


        private void OnFireCallClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_cts != null)
            {
                AddLog($"Fire already pending to be called in {_remainingTime} seconds.");
                return;
            }

            SetBusy(true);
            //start countdown task, when time is up, continue with actual Fire call
            var newCts = new CancellationTokenSource();
            _cts = newCts;
            FireCall(_cts.Token);

            }
            catch (Exception)
            {
                AddLog("Something wrong happened.");
                SetBusy(false);
            }


        }

        private void FireCall(CancellationToken token)
        {
            Task.Factory.StartNew(() => {
                
                AddLog("FireCall task is entered.");

                int count = 0;
                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _timer.Tick += (s, e) =>
                {
                    try
                    {
                        count++;

                        token.ThrowIfCancellationRequested();
                        if (count >= 5) //wait until the 5th seconds
                        {
                            _timer.Stop();
                            _timer = null;
                            _cts = null;
                            AddLog("Fire is called.");
                            SetBusy(false);
                            return;
                        }
                        AddLog("Fire pending...");
                        
                    }
                    catch (OperationCanceledException exception)
                    {
                        _timer?.Stop();
                        _timer = null;
                        AddLog("Fire call cancelled.");
                        SetBusy(false);
                        
                    }
                  
                };
                //enter a standby mode for x seconds, display waiting time every 1 sec
                //check for token.ThrowIfCancellationRequested() for every 1 sec tick
                _timer.Start();
            }, token, TaskCreationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext() );
            
        }
        

        private void AddLog(string s)
        {
            resultsTextBox.Text += s + Environment.NewLine;
        }

        private void OnCancelFireCallClick(object sender, RoutedEventArgs e)
        {
            //cancel count-down task and reset __remainingTime to -1
            if (_cts == null)
                AddLog("No Fire call pending. Request ignored.");
            else
            {
                AddLog("Trying to cancel pending Fire call...");
                //add logic here
                _cts.Cancel();
                _cts = null;
            }
        }
    }
}
