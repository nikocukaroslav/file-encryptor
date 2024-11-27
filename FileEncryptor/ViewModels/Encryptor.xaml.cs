using FileEncryptor.Data;
using FileEncryptor.Factory;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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
    /// Interaction logic for Encryptor.xaml
    /// </summary>
    public partial class Encryptor : Window
    {
        private readonly IFileEncryptorFactory _repository;

        private string fileToEncrypt;
        private string stringToEncrypt;
        private string encryptionType;
        public Encryptor()
        {
            InitializeComponent();
            _repository = new FileEncryptorFactory();
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
                SelectFileToEncrypt.Content = openFileDialog.FileName;
            }
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(stringToEncrypt) && string.IsNullOrEmpty(fileToEncrypt) && string.IsNullOrEmpty(encryptionType))
            {
                MessageBox.Show("Select data");
                return;
            }

            DateTime startTime = DateTime.Now;

            byte[] key = _repository.GetFileEncryptorRepository(encryptionType).GenerateRandomKey();
            byte[] iv = _repository.GetFileEncryptorRepository(encryptionType).GenerateRandomIv();


            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save file",
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
                var repository = _repository.GetFileEncryptorRepository(encryptionType);

                if (!string.IsNullOrEmpty(fileToEncrypt))
                {
                    repository.EncryptFile(fileToEncrypt, path, key, iv, progress =>
                    {
                        worker.ReportProgress(progress);
                    });
                }
                else if (!string.IsNullOrEmpty(stringToEncrypt))
                {
                    repository.EncryptString(stringToEncrypt, path, key, iv, progress =>
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
                    MessageBox.Show("An error occurred: " + args.Error.Message);
                    ResetValues();
                    return;
                }

                var stringKey = Convert.ToBase64String(key);
                var stringIv = Convert.ToBase64String(iv);

                long fileSize = new FileInfo(path).Length;
                double fileSizeInMB = fileSize / (1024.0 * 1024.0);

                CustomEncryptorWindow customWindow = new CustomEncryptorWindow(stringKey, stringIv, encryptionDuration, fileSizeInMB);
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
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stringToEncrypt = MessageToEncrypt.Text;
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            encryptionType = SelectEncryptionTypeInput.SelectedValue.ToString();
        }
    }
}
