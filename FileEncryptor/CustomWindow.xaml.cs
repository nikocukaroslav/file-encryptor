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
using System.Windows.Shapes;

namespace FileEncryptor
{
    /// <summary>
    /// Interaction logic for CustomWindow.xaml
    /// </summary>
    public partial class CustomWindow : Window
    {
        public CustomWindow(string key, string iv, TimeSpan time, double fileSize)
        {
            InitializeComponent();
            TimeTextBox.Text =$"Duration: {time.ToString().Substring(0, 8)}";
            FileSizeTextBox.Text = $"File size: {fileSize:F2} Mb";
            KeyTextBox.Text = $"Key: {key}";
            IvTextBox.Text = $"IV: {iv}";
        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(KeyTextBox.Text + "\n" + IvTextBox.Text);

            this.Close();
        }
    }
}
