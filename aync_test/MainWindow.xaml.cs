using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

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
            this.spinnerWait.Visibility = System.Windows.Visibility.Visible;
            this.spinnerWait.Spin = true;
            this.lblStatus.Content = "Thread is running";
            Task t = Task.Factory.StartNew(
                () => Thread.Sleep(5000)
            );
            t.ContinueWith(antecednet =>
            {
                this.lblStatus.Content = "Thread not running.";
                this.spinnerWait.Visibility = System.Windows.Visibility.Hidden;
                this.spinnerWait.Spin = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
