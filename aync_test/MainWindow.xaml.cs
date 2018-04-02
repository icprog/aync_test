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
            this.lblStatus.Content = "Thread is running";
            Task t = Task.Factory.StartNew(
                () => Thread.Sleep(5000)
            );
            t.ContinueWith(antecednet =>
            {
                this.lblStatus.Content = "Thread not running.";
                this.spinnerWait.Visibility = Visibility.Hidden;
                this.spinnerWait.Spin = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
