using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MenuItem = System.Windows.Controls.MenuItem;

namespace WPF_ExplorePath
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
        private void Open(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog()
            {
                Description = "Select directory to open"
            };
            DialogResult result = dlg.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                treeView.Items.Clear();
                DirectoryInfo di = new DirectoryInfo(dlg.SelectedPath);
                var root = MakeTreeDirectory(di);
                treeView.Items.Add(root); 
            }
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
        private TreeViewItem MakeTreeDirectory(DirectoryInfo di)
        {
            var root = new TreeViewItem
            {
                Header = di.Name,
                Tag = di.FullName
            };
            root.ContextMenu = new System.Windows.Controls.ContextMenu();
            var menuItem1 = new MenuItem { Header = "Create" };
            menuItem1.Click += new RoutedEventHandler(MenuItemCreateClick);
            var menuItem2 = new MenuItem { Header = "Delete" };
            menuItem2.Click += new RoutedEventHandler(MenuItemDeleteClick);
            root.ContextMenu.Items.Add(menuItem1);
            root.ContextMenu.Items.Add(menuItem2);

            foreach(DirectoryInfo d in di.GetDirectories())
            {
                root.Items.Add(MakeTreeDirectory(d));
            }
            foreach(FileInfo f in di.GetFiles())
            {
                root.Items.Add(MakeTreeFile(f));
            }
            root.Selected += new RoutedEventHandler(StatusBarUpdate);
            return root;

        }
        private TreeViewItem MakeTreeFile(FileInfo fi)
        {
            var item = new TreeViewItem
            {
                Header = fi.Name,
                Tag = fi.FullName
            };
            item.ContextMenu = new System.Windows.Controls.ContextMenu();
            var menuItem1 = new MenuItem { Header = "Open" };
            menuItem1.Click += new RoutedEventHandler(MenuItemOpenClick);
            var menuItem2 = new MenuItem { Header = "Delete" };
            menuItem2.Click += new RoutedEventHandler(MenuItemDeleteClick);
            item.ContextMenu.Items.Add(menuItem1);
            item.ContextMenu.Items.Add(menuItem2);
            item.Selected += new RoutedEventHandler(StatusBarUpdate);
            return item;
        }
        private void MenuItemOpenClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem it = (TreeViewItem)treeView.SelectedItem;
            string value = File.ReadAllText((string)it.Tag);
            scrollViewer.Content = new TextBlock() { Text = value };
        }
        private void MenuItemCreateClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem dir = (TreeViewItem)treeView.SelectedItem;
            string path = (string)dir.Tag;
            Dialog dialog = new Dialog(path);
            dialog.ShowDialog();
            if (dialog.Completed())
            {
                if (File.Exists(dialog.GetPath()))
                {
                    FileInfo fi = new FileInfo(dialog.GetPath());
                    dir.Items.Add(MakeTreeFile(fi));
                }else if (Directory.Exists(dialog.GetPath()))
                {
                    DirectoryInfo di = new DirectoryInfo(dialog.GetPath());
                    dir.Items.Add(MakeTreeDirectory(di));
                }
            }
        }
        private void MenuItemDeleteClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
            string path = (string)item.Tag;
            FileAttributes attr = File.GetAttributes(path);
            File.SetAttributes(path, attr & ~FileAttributes.ReadOnly);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DeleteDirectory(path);
            }
            else
            {
                File.Delete(path);
            }
            if ((TreeViewItem)treeView.Items[0] != item)
            {
                TreeViewItem parent = (TreeViewItem)item.Parent;
                parent.Items.Remove(item);
            }
            else
            {
                treeView.Items.Clear();
            }
        }
        private void DeleteDirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach(var d in di.GetDirectories())
            {
                DeleteDirectory(d.FullName);
            }
            foreach(var f in di.GetFiles())
            {
                File.Delete(f.FullName);
            }
            Directory.Delete(path);
        }
        private void StatusBarUpdate(object sender, RoutedEventArgs e)
        {
            TreeViewItem it = (TreeViewItem)treeView.SelectedItem;
            FileAttributes attr = File.GetAttributes((string)it.Tag);
            statusDOS.Text = "";
            statusDOS.Text += (attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly 
                ? 'r' : '-';
            statusDOS.Text += (attr & FileAttributes.Archive) == FileAttributes.Archive
                ? 'a' : '-';
            statusDOS.Text += (attr & FileAttributes.Hidden) == FileAttributes.Hidden
                ? 'h' : '-';
            statusDOS.Text += (attr & FileAttributes.System) == FileAttributes.System
                ? 's' : '-';
        }

    }
     
}
