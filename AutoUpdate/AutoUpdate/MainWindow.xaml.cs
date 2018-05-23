using System;
using System.Collections.Generic;
using System.Data;
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
        public static ConFile con_file;
        List<ConFile> con_file_list;
        public MainWindow()
        {
            InitializeComponent();
            
            // 初始化窗口时先把配置界面隐藏，也可以在xaml中设置，但是xaml的效果是实时的，不方便调试
            config_page.Visibility = Visibility.Hidden;

            // 写死在主界面的数据，通过数据列表的绑定进行显示，还没试过动态删除，具体用法要结合xaml来看
            var main_grid_list = new List<MainGridData>();
           
            DirectoryInfo dic = new DirectoryInfo(".\\ConFile");
            System.IO.FileInfo[] fileinfo = dic.GetFiles();
            //遍历文件列表
            con_file_list = new List<ConFile>();
            foreach (System.IO.FileInfo it in fileinfo)
            {
                //config_grid_list.Add(new ConfigureGridData { Picked = false, File = it.Name, Version = "1.0", Time = it.LastWriteTime.ToString(), Hash = "26205fa396afae7e698346556c23f256" });
                con_file_list.Add(new ConFile(it.Name));
            }

            foreach (var it in con_file_list)
            {
                main_grid_list.Add(new MainGridData
                {
                    ConFile = it.GetName(),
                    Version = it.GetVersion().ToString(),
                    Time = it.GetTime(),
                    Hash = it.GetHash()
                });
            }

            main_data_grid.ItemsSource = main_grid_list;

            List<string> comboBoxItems = new List<string>();
            comboBoxItems.Add("部分更新");
            comboBoxItems.Add("整体更新");
            comboBoxItems.Add("部分更新");
            dataGridComboBoxColumn.ItemsSource = comboBoxItems;

            con_file = new ConFile("confile1", 2.1f, "2018/5/23", "abcdefg");

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
                treeView.Items.Clear();
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
            sfd.InitialDirectory = @".\";
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
            sfd.InitialDirectory = @".\";
            // 设置保存的文件的类型，注意过滤器的语法  
            sfd.Filter = "CONF配置文件|*.conf";
            // 调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮  

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //获得文件路径
                string localFilePath = sfd.FileName.ToString();

                //获取文件名，不带路径
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);

                //获取文件路径，不带文件名
                string FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));

                MainWindow.con_file.SetName(FilePath + "ConFile\\" + fileNameExt);
                
                MainWindow.con_file.SaveConFile();
            }


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

        //checkbox选中事件
        public void per_row_checkbox_checked(object sender, RoutedEventArgs e)
        {
            //获取当前选中行
            var a = config_data_grid.SelectedItem as ConfigureGridData;

            string result = a.UpdateType;

            string name = a.File;
            float version = float.Parse(a.Version);
            string hash = a.Hash;
            FileInfo.UpdateMethod u_method = FileInfo.UpdateMethod.WHOLE;
            FileInfo file_info = new FileInfo(name, version, hash, u_method);
            con_file.AddFile(file_info);

            //treeView.Items.Add("1111");
            //var selectItem = this.config_data_grid.SelectedItem as DataGridRow;//!根据点击的item获取集合中的数据
            //treeView.Items.Add(selectItem);
            //DataGridRow mySelectedElement = (DataGridRow)config_data_grid.SelectedItem;
            //string result = mySelectedElement.ToString();
            //treeView.Items.Add(result);
        }

        //checkbox取消事件
        public void per_row_checkbox_unchecked(object sender, RoutedEventArgs e)
        {
            var a = config_data_grid.SelectedItem as ConfigureGridData;

            for (int i = con_file.GetFileCount() - 1; i >= 0; i--)
            {
                if (con_file.GetList()[i].GetName() == a.File)
                {
                    con_file.GetList().Remove(con_file.GetList()[i]);
                }
            }
        }

        private void config_data_grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
