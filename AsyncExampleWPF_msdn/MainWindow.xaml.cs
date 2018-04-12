using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace AsyncExampleWPF_msdn
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

        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            spinnerWait.Visibility = Visibility.Visible;
            spinnerWait.Spin = true;
            resultsTextBox.Clear();
            await SumPageSizesAsync();
            resultsTextBox.Text += $"{Environment.NewLine}Control returned to startButton_Click.";
            startButton.IsEnabled = true;
            spinnerWait.Visibility = Visibility.Collapsed;
            spinnerWait.Spin = false;
        }
        private async Task SumPageSizesAsync()
        {
            // Make a list of web addresses.  
            List<string> urlList = SetUpURLList();

            var total = 0;
            foreach (var url in urlList)
            {
                // GetURLContentsAsync returns the contents of url as a byte array.  
                byte[] urlContents = await GetURLContentsAsync(url);
                // GetURLContentsAsync returns a Task<T>. At completion, the task produces a byte array.  
                //Task<byte[]> getContentsTask = GetURLContentsAsync(url);  
                //byte[] urlContents = await getContentsTask;  

                DisplayResults(url, urlContents);

                // Update the total.  
                total += urlContents.Length;
            }

            // Display the total count for all of the web addresses.  
            var newLine = Environment.NewLine;
            resultsTextBox.Text +=
                //$"\r\n\r\nTotal bytes returned:  {total}\r\n";
                $"{newLine}Total bytes returned:  {total}\r";
        }

        private List<string> SetUpURLList()
        {
            var urls = new List<string>
                            {
                                "http://msdn.microsoft.com/library/windows/apps/br211380.aspx",
                                "http://msdn.microsoft.com",
                                "http://msdn.microsoft.com/library/hh290136.aspx",
                                "http://msdn.microsoft.com/library/ee256749.aspx",
                                "http://msdn.microsoft.com/library/hh290138.aspx",
                                "http://msdn.microsoft.com/library/hh290140.aspx",
                                "http://msdn.microsoft.com/library/dd470362.aspx",
                                "http://msdn.microsoft.com/library/aa578028.aspx",
                                "http://msdn.microsoft.com/library/ms404677.aspx",
                                "http://msdn.microsoft.com/library/ff730837.aspx"
                            };
            return urls;
        }

        private async Task<byte[]> GetURLContentsAsync(string url)
        {
            // The downloaded resource ends up in the variable named content.  
            var content = new MemoryStream();

            // Initialize an HttpWebRequest for the current URL.  
            var webReq = (HttpWebRequest)WebRequest.Create(url);

            // Send the request to the Internet resource and wait for  
            // the response.  
            // Note: you can't use HttpWebRequest.GetResponse in a Windows Store app.  
            using (WebResponse response = await webReq.GetResponseAsync())
            {
                // Get the data stream that is associated with the specified URL.  
                using (Stream responseStream = response.GetResponseStream())
                {
                    // Read the bytes in responseStream and copy them to content.    
                    await responseStream.CopyToAsync(content);

                    // CopyToAsync returns a Task, not a Task<T>.  
                    //Task copyTask = responseStream.CopyToAsync(content);  

                    // When copyTask is completed, content contains a copy of responseStream.  
                    //await copyTask;  
                }
            }

            // Return the result as a byte array.  
            return content.ToArray();
        }

        private void DisplayResults(string url, byte[] content)
        {
            // Display the length of each website. The string format   
            // is designed to be used with a monospaced font, such as  
            // Lucida Console or Global Monospace.  
            var bytes = content.Length;
            // Strip off the "http://".  
            var displayUrl = url.Replace("http://", "");
            resultsTextBox.Text += $"{displayUrl,-58} {bytes,8}\r";
        }
    }
}
