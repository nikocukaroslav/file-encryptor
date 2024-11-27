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
    /// Interaction logic for Decryptor.xaml
    /// </summary>
    public partial class Decryptor : Window
    {
        private readonly IFileEncryptorFactory _repository;

        private string fileToDecrypt;
        private string stringToDecrypt;
        private string decryptionType;
        public Decryptor()
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
                fileToDecrypt = filePath;
                SelectFileToDecrypt.Content = openFileDialog.FileName;
            }
        }
        private void Start(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(stringToDecrypt) && string.IsNullOrEmpty(fileToDecrypt))
            {
                MessageBox.Show("Select data");
                return;
            }

            DateTime startTime = DateTime.Now;

            if (string.IsNullOrEmpty(KeyTextBox.Text) || string.IsNullOrEmpty(IVTextBox.Text))
            {
                MessageBox.Show("Key and IV are required for decryption.");
                return;
            }

            byte[] key = Convert.FromBase64String(KeyTextBox.Text);
            byte[] iv = Convert.FromBase64String(IVTextBox.Text);

            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save file",
                FileName = "decrypted_file.txt"
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

            DecryptionProgress.Visibility = Visibility.Visible;

            worker.DoWork += (s, args) =>
            {
                var repository = _repository.GetFileEncryptorRepository(decryptionType);

                if (!string.IsNullOrEmpty(fileToDecrypt))
                {
                    repository.DecryptFile(fileToDecrypt, path, key, iv, progress =>
                    {
                        worker.ReportProgress(progress);
                    });
                }
                else if (!string.IsNullOrEmpty(stringToDecrypt))
                {
                    repository.DecryptString(stringToDecrypt, path, key, iv, progress =>
                    {
                        worker.ReportProgress(progress);
                    });
                }
            };

            worker.ProgressChanged += (s, args) =>
            {
                DecryptionProgressBar.Value = args.ProgressPercentage;
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

                long fileSize = new FileInfo(path).Length;
                double fileSizeInMB = fileSize / (1024.0 * 1024.0);

                CustomDecryptorWindow customWindow = new CustomDecryptorWindow(encryptionDuration, fileSizeInMB);
                customWindow.ShowDialog();

                ResetValues();
            };

            worker.RunWorkerAsync();
        }
        private void ResetValues()
        {
            DecryptionProgress.Visibility = Visibility.Hidden;
            SelectFileToDecrypt.Content = "Click to select";
            MessageToDecrypt.Text = null;
            KeyTextBox.Text = null;
            IVTextBox.Text = null;
            fileToDecrypt = null;
            stringToDecrypt = null;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stringToDecrypt = MessageToDecrypt.Text;
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            decryptionType = SelectDecryptionTypeInput.SelectedValue.ToString();
        }
    }
}
