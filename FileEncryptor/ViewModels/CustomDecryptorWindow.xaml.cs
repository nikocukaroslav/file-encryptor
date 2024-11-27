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

namespace FileEncryptor.ViewModels
{
    /// <summary>
    /// Interaction logic for CustomDecryptorWindow.xaml
    /// </summary>
    public partial class CustomDecryptorWindow : Window
    {
        public CustomDecryptorWindow(TimeSpan time, double fileSize)
        {
            InitializeComponent();
            TimeTextBox.Text = $"Duration: {time.ToString().Substring(0, 8)}";
            FileSizeTextBox.Text = $"File size: {fileSize:F2} Mb";
        }
        private void Finish(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
