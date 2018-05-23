using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            // 初始化窗口时先把配置界面隐藏，也可以在xaml中设置，但是xaml的效果是实时的，不方便调试
            config_page.Visibility = Visibility.Hidden;

            // 写死在主界面的数据，通过数据列表的绑定进行显示，还没试过动态删除，具体用法要结合xaml来看
            var main_grid_list = new List<MainGridData>();
            main_grid_list.Add(new MainGridData { ConFile = "ConFile1", Version = "v1.0", Time = "2017.10.13", Hash = "ff3e568a272ca876d331675fd3d5a6c2" });
            main_grid_list.Add(new MainGridData { ConFile = "ConFile2", Version = "v1.02", Time = "2017.11.13", Hash = "3431f59cbcae719ef1aa00d40ac166d8" });
            main_grid_list.Add(new MainGridData { ConFile = "ConFile3", Version = "v1.06", Time = "2017.11.29", Hash = "7889e750271c12eca0dfd8be3639c6b1" });
            main_grid_list.Add(new MainGridData { ConFile = "ConFile4", Version = "v1.2", Time = "2017.12.20", Hash = "cb0c2ee777054e54c51420dfc2e2cf82" });

           
            

        }

        // 主界面中的数据结构
        public class MainGridData
        {
            public string ConFile { get; set; } // 配置文件名
            public string Version { get; set; } 
            public string Time { get; set; }
            public string Hash { get; set; }
            
        }

        // 编辑界面中的数据结构
        public class ConfigureGridData
        {
            public bool Picked { get; set; } // 是否勾选
            public string File { get; set; } // 文件名
            public string Version { get; set; }
            public string Time { get; set; }
            public string Hash { get; set; }

            public string UpdateType { get; set; }


    }

        // 窗口加载完之后的函数，在xaml中进行了绑定，关键字async异步进行调用，否则无法得到想要的效果
        // 窗口加载完后还未进行渲染，如果是渲染完之后，可以在xaml中使用ContentRendered进行绑定
        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // 主窗口加载完后，延迟1秒打开更新提示框
            await Task.Delay(1000);
            FileStream fs_url = new FileStream(@"url.ini", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader m_streamReader = new StreamReader(fs_url);
            m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            string url = m_streamReader.ReadLine();
            ConFile con_install = new ConFile(url);
            if (con_install.GetVersion() > 1.1)
            {
                UpdateWindow update_window = new UpdateWindow();
                // 以对话框的形式打开
                update_window.ShowDialog();
            }
            
        }

        private void ShowMainPage(object sender, RoutedEventArgs e)
        {
            main_page.Visibility = Visibility.Visible;
            config_page.Visibility = Visibility.Hidden;
        }

        private void ShowConfigurePage(object sender, RoutedEventArgs e)
        {
            main_page.Visibility = Visibility.Hidden;
            config_page.Visibility = Visibility.Visible;


            try
            {
                //    DriveInfo[] dir = DriveInfo.GetDrives();
                //    foreach (DriveInfo item in dir)
                //    {
                //        TreeViewItem newItem = new TreeViewItem();
                //        newItem.Header = item.Name;
                //        newItem.Tag = item.Name;
                //        treeView.Items.Add(item);
                //    }
                TreeViewItem tvi = new TreeViewItem();
                tvi.Header = AppDomain.CurrentDomain.BaseDirectory;
                tvi.Tag = AppDomain.CurrentDomain.BaseDirectory;
                treeView.Items.Add(tvi);
            }
            catch
            {

            }


        }

        private void ShowUrlWindow(object sender, RoutedEventArgs e)
        {
            UrlWindow urlWindow = new UrlWindow();
            urlWindow.ShowDialog();
        }

        private void OpenFileWindow(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            // 设置这个对话框的打开路径  
            sfd.InitialDirectory = @"D:\";
            // 设置需要打开的文件类型，注意过滤器的语法  
            sfd.Filter = "CONF配置文件|*.conf";
            // 调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮  
            sfd.ShowDialog();

            ShowConfigurePage(sender, e);
        }

        private void ShowVersionWindow(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            // 设置这个对话框的起始保存路径  
            sfd.InitialDirectory = @"D:\";
            // 设置保存的文件的类型，注意过滤器的语法  
            sfd.Filter = "CONF配置文件|*.conf";
            // 调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮  
            sfd.ShowDialog();

            VersionWindow versionWindow = new VersionWindow();
            versionWindow.ShowDialog();
            main_page.Visibility = Visibility.Visible;
            config_page.Visibility = Visibility.Hidden;
        }

        // 关闭程序
        private void Shutdown(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void treeView_Selected(object sender, RoutedEventArgs e)
        {

            try
            {
                //遍历目录
                TreeViewItem treeViewItem = e.OriginalSource as TreeViewItem;
                treeViewItem.Items.Clear();
                DirectoryInfo dic = new DirectoryInfo(treeViewItem.Tag.ToString());
                DirectoryInfo[] info = dic.GetDirectories();
                foreach (DirectoryInfo item in info)
                {
                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = item.Name;
                    tvi.Tag = item.FullName;
                    treeViewItem.Items.Add(tvi);
                }


                var config_grid_list = new List<ConfigureGridData>();
                config_grid_list.Clear();


                //获取目录下文件列表
                System.IO.FileInfo[] fileinfo = dic.GetFiles();
                //遍历文件列表
                foreach (System.IO.FileInfo it in fileinfo)
                {
                    config_grid_list.Add(new ConfigureGridData { Picked = false, File = it.Name, Version = "1.0", Time = it.LastWriteTime.ToString(), Hash = "26205fa396afae7e698346556c23f256" });
                }
                config_data_grid.ItemsSource = config_grid_list;



            }
            catch
            {

            }


        }

        private void config_data_grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
