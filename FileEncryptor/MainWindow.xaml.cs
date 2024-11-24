using FileEncryptor.Data;
using FileEncryptor.Factory;
using FileEncryptor.Repository;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileEncryptor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IFileEncryptorFactory _repository;

        private string fileToEncrypt;
        private string stringToEncrypt;
        private string encryptionType = EncryptionMethods.GetEncryptionMethods().First().Name;
        public MainWindow()
        {
            _repository = new FIleEncryptorFactory();
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stringToEncrypt = MessageToEncrypt.Text;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            encryptionType = SelectEncryptionTypeInput.Name;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select a file",
                Multiselect = false,
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                fileToEncrypt = filePath;
                SelectButton.Content = filePath;
            }
        }

        private void StartEncryption(object sender, RoutedEventArgs e)
        {
            var key = _repository.GetEncryptorRepository(encryptionType).GenerateRandomKey();
            var iv = _repository.GetEncryptorRepository(encryptionType).GenerateRandomIv();

            BackgroundWorker worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };

            Progress.Visibility = Visibility.Visible;

            worker.DoWork += (s, args) =>
            {
                var path = $"C:\\Users\\yarek\\Desktop\\study\\.Net\\lab3\\FileEncryptor\\Data\\{encryptionType}_EncryptedData.dat";
                byte[] encryptedData = null;

                if (!string.IsNullOrEmpty(stringToEncrypt))
                {
                    encryptedData = _repository.GetEncryptorRepository(encryptionType).EncryptString(stringToEncrypt, key, iv);
                }
                else if (!string.IsNullOrEmpty(fileToEncrypt))
                {
                    _repository.GetEncryptorRepository(encryptionType).EncryptFile(fileToEncrypt, path, key, iv, progress =>
                    {
                        worker.ReportProgress(progress);
                    });
                }

                args.Result = new
                {
                    EncryptedData = encryptedData,
                    Path = path
                };
            };

            worker.ProgressChanged += (s, args) =>
            {
                ProgressBar.Value = args.ProgressPercentage;
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                if (args.Error != null)
                {
                    MessageBox.Show("An error occurred during encryption: " + args.Error.Message);
                    Progress.Visibility = Visibility.Hidden;
                    return;
                }

                var result = (dynamic)args.Result;

                if (result.EncryptedData != null)
                {
                    using (FileStream fileStream = new FileStream(result.Path, FileMode.Create, FileAccess.Write))
                    {
                        fileStream.Write(result.EncryptedData, 0, result.EncryptedData.Length);
                    }
                    MessageBox.Show($"Encrypted data saved to: {result.Path}");
                }
                MessageToEncrypt.Text = null;
                SelectButton.Content = "Click to select";
                Progress.Visibility = Visibility.Hidden;
            };

            worker.RunWorkerAsync();
        }
    }
}