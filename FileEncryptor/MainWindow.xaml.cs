using FileEncryptor.Data;
using FileEncryptor.Factory;
using FileEncryptor.Repository;
using Microsoft.Win32;
using System;
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
                SelectFileToEncrypt.Content = filePath;
            }
        }

        private void StartEncryption(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(stringToEncrypt) && string.IsNullOrEmpty(fileToEncrypt))
            {
                MessageBox.Show("Select data to encrypt");
                return;
            }
            DateTime startTime = DateTime.Now;

            var key = _repository.GetEncryptorRepository(encryptionType).GenerateRandomKey();
            var iv = _repository.GetEncryptorRepository(encryptionType).GenerateRandomIv();

            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save encrypted file",
                FileName = "encrypted_file.txt"
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            var path = saveFileDialog.FileName;

            BackgroundWorker worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };

            EncryptionProgress.Visibility = Visibility.Visible;

            worker.DoWork += (s, args) =>
            {
                
                if (!string.IsNullOrEmpty(stringToEncrypt))
                {
                    _repository.GetEncryptorRepository(encryptionType).EncryptString(stringToEncrypt, path, key, iv, progress =>
                    {
                        worker.ReportProgress(progress);
                    });
                }
                else if (!string.IsNullOrEmpty(fileToEncrypt))
                {
                    _repository.GetEncryptorRepository(encryptionType).EncryptFile(fileToEncrypt, path, key, iv, progress =>
                    {
                        worker.ReportProgress(progress);
                    });
                }
            };

            worker.ProgressChanged += (s, args) =>
            {
                EncryptionProgressBar.Value = args.ProgressPercentage;
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                DateTime endTime = DateTime.Now;

                TimeSpan encryptionDuration = endTime - startTime;

                if (args.Error != null)
                {
                    MessageBox.Show("An error occurred during encryption: " + args.Error.Message);
                    ResetValues();
                    return;
                }

                var stringKey = Convert.ToBase64String(key);
                var stringIv = Convert.ToBase64String(iv);

                long fileSize = new FileInfo(path).Length;
                double fileSizeInMB = fileSize / (1024.0 * 1024.0);

                CustomWindow customWindow = new CustomWindow(stringKey, stringIv, encryptionDuration, fileSizeInMB);
                customWindow.ShowDialog();

                ResetValues();
            };

            worker.RunWorkerAsync();
        }

        private void ResetValues()
        {
            EncryptionProgress.Visibility = Visibility.Hidden;
            SelectFileToEncrypt.Content = "Click to select";
            MessageToEncrypt.Text = null;
            fileToEncrypt = null;
            stringToEncrypt = null;
        }

        private void StartDecryption(object sender, RoutedEventArgs e)
        {

        }
    }
}