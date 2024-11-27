using FileEncryptor.Data;
using FileEncryptor.Factory;
using FileEncryptor.Repository;
using FileEncryptor.ViewModels;
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
        public MainWindow()
        {
            InitializeComponent();

        }
        private void StartEncryptor(object sender, RoutedEventArgs e)
        {
            Encryptor encryptor = new Encryptor();
            encryptor.Show();
        }
        private void StartDecryptor(object sender, RoutedEventArgs e)
        {
            Decryptor decryptor = new Decryptor();
            decryptor.Show();
        }
    }
}