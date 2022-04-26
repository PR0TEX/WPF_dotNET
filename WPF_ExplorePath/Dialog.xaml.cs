using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_ExplorePath
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        private string path;
        private string name;
        private bool complete;
        public Dialog(string path)
        {
            InitializeComponent();
            this.path = path;
            this.complete = false;
        }
        public void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            bool isFile = (bool)radioButtonFile.IsChecked;
            bool isDir = (bool)radioButtonDirectory.IsChecked;
            if (isFile && !Regex.IsMatch(dialogName.Text, "^[a-zA-Z0-9_~-]{1,8}\\.(txt|php|html)$"))
            {
                System.Windows.MessageBox.Show("Incorrect name!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (!isFile && !isDir)
            {
                System.Windows.MessageBox.Show("Select what do you want to create!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                name = dialogName.Text;
                path = path + "\\" + name;
                FileAttributes attr = FileAttributes.Normal;

                attr |= (bool)checkBoxRO.IsChecked ? FileAttributes.ReadOnly : 0;
                attr |= (bool)checkBoxA.IsChecked ? FileAttributes.Archive : 0;
                attr |= (bool)checkBoxH.IsChecked ? FileAttributes.Hidden : 0;
                attr |= (bool)checkBoxS.IsChecked ? FileAttributes.System : 0;
                if (isFile)
                {
                    File.Create(path);
                }
                else if (isDir)
                {
                    Directory.CreateDirectory(path);
                }
                File.SetAttributes(path, attr);
                complete = true;
                Close();
            }

        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public bool Completed()
        {
            return complete;
        }

        public string GetPath()
        {
            return path;
        }
    }
}
