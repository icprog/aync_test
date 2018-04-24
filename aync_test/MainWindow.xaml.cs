using System;
using System.Collections.Concurrent;
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
            AddLog("Thread is running. Using Task.Delay");
            Task t = Task.Delay(5000).ContinueWith(antecednet =>
            {
                AddLog("Thread stopped.");
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
            AddLog($"Task stopped at time {time.ToLocalTime()}");

            SetBusy(false);
            this.button.IsEnabled = true;
            this.button2.IsEnabled = true;
        }

        private async void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            var length = await GetMsdnStringLth();
            AddLog($"Task finished. MSDN page character length is {length}");

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

            AddLog("Waiting for async task to finish...");
            SetBusy(false);
            this.button.IsEnabled = false;
            this.button2.IsEnabled = false;

            task.Start();
            DateTime time = await task;
            return time;
        }

        private async Task<int> GetMsdnStringLth()
        {
            AddLog("Waiting for async task to finish...");
            SetBusy(true);
            this.button.IsEnabled = false;
            this.button1.IsEnabled = false;

            Task<string> getStringTask = new HttpClient().GetStringAsync("http://msdn.microsoft.com");
            string urlContents = await getStringTask;
            return urlContents.Length;
        }


        private async void OnFireCallClick(object sender, RoutedEventArgs e)
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
                CancellationToken token = _cts.Token;

                var task = DoWorkAsyncInfiniteLoop2(token);

                try
                {
                    await task;
                }
                catch (OperationCanceledException oe)
                {
                    AddLog("DoWorkAsyncInfiniteLoop2 is cancelled.");
                    SetBusy(false);
                }
                catch (Exception)
                {
                    
                    throw;
                }

                //Task.Run(async () =>
                //{
                //    int count = 20;
                //    while (count>0)
                //    {
                //        Dispatcher.Invoke(() =>
                //        {
                //            string sUnit = count > 1 ? "seconds" : "second";
                //            AddLog($"test Fire pending in {count} {sUnit}...");
                //        });
                //        count--;
                //        await Task.Delay(1000);
                //    }
                //});


                //    Task T = Task.Factory.StartNew(() => FireCall_retry(token), token,
                //        TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext())
                //        .ContinueWith(task =>
                //        {
                //            try
                //            {
                //                var result = task.Result;
                //            }
                //            catch (OperationCanceledException oce)
                //            {
                //                AddLog("Fire call cancelled.");
                //            }
                //            catch (Exception)
                //            {

                //                throw;
                //            }
                //            finally
                //            {

                //            }

                //            switch (task.Status)
                //            {
                //                case TaskStatus.RanToCompletion:
                //                    // The normal stuff
                //                    AddLog("Fire is called successfully.");
                //                    break;
                //                case TaskStatus.Canceled:
                //                    // Handle cancellation
                //                    AddLog("Fire call cancelled.");
                //                    break;
                //                case TaskStatus.Faulted:
                //                    AddLog("Something wrong happened.");
                //                    // Handle other exceptions
                //                    break;
                //            }

                //        }, TaskScheduler.FromCurrentSynchronizationContext());
                //    //.ContinueWith(task =>
                //    //{
                //    //    AddLog("Fire call cancelled.");
                //    //}, TaskContinuationOptions.OnlyOnCanceled)
                //    //.ContinueWith(task =>
                //    //{
                //    //    AddLog("Something wrong happened.");
                //    //}, TaskContinuationOptions.OnlyOnRanToCompletion);
                //    //var tasks = new ConcurrentBag<Task>();
                //    //tasks.Add(T);
                //    //Task.WaitAll(tasks.ToArray());

            }
            catch (AggregateException ae)
            {
                AddLog("Fire call cancelled.");
            }
            catch (Exception)
            {
                AddLog("Something wrong happened.");
                SetBusy(false);
            }


        }

        private async Task DoWorkAsyncInfiniteLoop2(CancellationToken token)
        {
            AddLog("DoWorkAsyncInfiniteLoop2 task is entered.");
            int count = 10;

            while (count > 0 /*&& ! token.IsCancellationRequested*/)
            {
                token.ThrowIfCancellationRequested();

                // update the UI
                string sUnit = count > 1 ? "seconds" : "second";
                AddLog($"Fire pending in {count} {sUnit}...");

                await Task.Delay(1000);
                count--;
            }
            AddLog("DoWorkAsyncInfiniteLoop2 task is finished.");
            SetBusy(false);
        }

        private async Task DoWorkAsyncInfiniteLoop()
        {
            while (true)
            {
                // do the work in the loop
                string newData = DateTime.Now.ToLongTimeString();

                // update the UI
                AddLog("ASYNC LOOP - " + newData);

                // don't run again for at least 200 milliseconds
                await Task.Delay(1000);
            }
        }

        private void FireCall(CancellationToken token)
        {
            Task.Factory.StartNew(() => {
                
                AddLog("FireCall task is entered.");

                int count = 120;
                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _timer.Tick += (s, e) =>
                {
                    try
                    {
                        count--;

                        token.ThrowIfCancellationRequested();
                        if (count <= 0) //wait until the 5th seconds
                        {
                            _timer.Stop();
                            _timer = null;
                            _cts = null;
                            AddLog("Fire is called.");
                            SetBusy(false);
                            return;
                        }
                        string sUnit = count > 1 ? "seconds" : "second";
                        AddLog($"Fire pending in {count} {sUnit}...");
                        
                    }
                    catch (OperationCanceledException exception)
                    {
                        _timer?.Stop();
                        _timer = null;
                        SetBusy(false);
                        
                    }
                  
                };
                //enter a standby mode for x seconds, display waiting time every 1 sec
                //check for token.ThrowIfCancellationRequested() for every 1 sec tick
                _timer.Start();
            }, token, TaskCreationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext() );
            
        }

        private bool FireCall_retry(CancellationToken token)
        {
            AddLog("FireCall task is entered.");

            int count = 10;

            while (count > 0)
            {
                //check for token.ThrowIfCancellationRequested() for every 1 sec tick
                
                token.ThrowIfCancellationRequested();

                 Task.Delay(1000).ContinueWith(task => {
                    count--;
                    string sUnit = count > 1 ? "seconds" : "second";
                    AddLog($"Fire pending in {count} {sUnit}...");
                }, token, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            }
            return true;
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
                _cts = new CancellationTokenSource();
            }
        }
    }
}
