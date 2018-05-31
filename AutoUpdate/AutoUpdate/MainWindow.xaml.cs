using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Diagnostics;  // 用于调试的类,使用Trace.Write(str)输出信息
using System.Net;

namespace AutoUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    
    public partial class MainWindow : Window
    {
        AppInfo app_info = AppInfo.GetInstance();
        public static ConFile con_file;
        string relative_path;   // 配置界面显示的文件所在的相对路径

        public MainWindow()
        {
            InitializeComponent();

            // 初始化窗口时先把配置界面隐藏，也可以在xaml中设置，但是xaml的效果是实时的，不方便调试
            config_page.Visibility = Visibility.Hidden;

            ShowMainPage();
            

            List<string> comboBoxItems = new List<string>();
            comboBoxItems.Add("重启后覆盖");
            comboBoxItems.Add("运行时覆盖");
            dataGridComboBoxColumn.ItemsSource = comboBoxItems;
            
            set_version_box.Text = (app_info.GetVersion() + 0.01f).ToString();
            label_warnning.Visibility = Visibility.Hidden;
            
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

        private async void WindowContentRendered(object sender, EventArgs e)
        {
            // 主窗口加载完后，延迟1秒打开更新提示框
            await Task.Delay(1000);

            string install_ini = app_info.GetInstallName();
            FileDownload file_download = new FileDownload(app_info.GetUrl(), install_ini);
            if (file_download.URLExists() && file_download.GetVersion() > app_info.GetVersion())
            {
                UpdateWindow update_window = new UpdateWindow();
                // 以对话框的形式打开
                update_window.ShowDialog();
                ShowMainPage();
            }
            else
            {
                if (File.Exists(app_info.GetInstallName()))
                    File.Delete(app_info.GetInstallName());
            }
        }

        private void ShowMainPage(object sender, RoutedEventArgs e)
        {
            ShowMainPage();
        }

        private void ShowMainPage()
        {
            app_info.RefreshVersion();
            info_display(app_info.GetVersion().ToString(), app_info.GetFileCount().ToString(), app_info.GetTime(), app_info.GetHash());
            var main_grid_list = new List<MainGridData>();

            // 遍历文件列表
            foreach (var item in app_info.GetHistoryList())
            {
                ConFile con = new ConFile(item);
                main_grid_list.Add(new MainGridData
                {
                    ConFile = con.GetName().Substring(con.GetName().LastIndexOf("\\") + 1),
                    Version = con.GetVersion().ToString(),
                    Time = con.GetTime(),
                    Hash = con.GetHash()
                });
            }

            main_data_grid.ItemsSource = main_grid_list;

            // 返回主界面时如果不加这一句，配置界面的数据不会消除，
            // 下次再进入配置界面时，由于目录树还未被点击，配置界面不会
            // 重置，会显示旧数据
            config_data_grid.ItemsSource = null;

            main_page.Visibility = Visibility.Visible;
            config_page.Visibility = Visibility.Hidden;
        }

        private void CreateConFile(object sender, RoutedEventArgs e)
        {
            con_file = new ConFile("version.ini");
            ShowConfigurePage();
        }

        private void OpenConFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            // 设置这个对话框的打开路径  
            sfd.InitialDirectory = @".\";
            // 设置需要打开的文件类型，注意过滤器的语法  
            sfd.Filter = "CONF配置文件|*.conf";
            // 调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮  

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                con_file = new ConFile(sfd.FileName);
            }
            ShowConfigurePage();
        }

        private void OpenConFile_Clicked(object sender, RoutedEventArgs e)
        {
            // 获取配置文件的索引
            var index = main_data_grid.SelectedIndex;
            if (index > -1)
            {
                string file_path = app_info.GetHistoryList()[index];
                con_file = new ConFile(file_path);
            }
            ShowConfigurePage();
        }

        private void RemoveConFile(object sender, RoutedEventArgs e)
        {
            var index = main_data_grid.SelectedIndex;
            if (index > -1)
            {
                string file_path = app_info.GetHistoryList()[index];
                app_info.RemoveHistory(file_path);
            }
            ShowMainPage();
        }

        private void ShowConfigurePage()
        {
            main_page.Visibility = Visibility.Hidden;
            config_page.Visibility = Visibility.Visible;

            try
            {
                treeView.Items.Clear();
                TreeViewItem tvi = new TreeViewItem();
                // 只获取当前所在的目录名
                tvi.Header = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Name;
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


        // 生成版本
        private void ShowVersionWindow(object sender, RoutedEventArgs e)
        {
            
            if (float.TryParse(set_version_box.Text, out float result) && result > app_info.GetVersion())
            {
                SaveFileDialog sfd = new SaveFileDialog();
                // 设置这个对话框的起始保存路径  
                sfd.InitialDirectory = @".\";
                // 设置保存的文件的类型，注意过滤器的语法  
                sfd.Filter = "CONF配置文件|*.conf";
                // 调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮  
                foreach (var item in con_file.GetList())
                    Trace.WriteLine(item.GetName());
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    MainWindow.con_file.SetName(sfd.FileName);
                    MainWindow.con_file.SetVersion(float.Parse(set_version_box.Text));
                    MainWindow.con_file.SetTime(DateTime.Now.ToString("yyyy/MM/dd"));
                    //MainWindow.con_file.SetHash(app_info.CreateMD5(sfd.FileName));  // TODO: 这里需要相对路径，暂时没有改
                    MainWindow.con_file.SaveConFile();
                    app_info.AddHistory(sfd.FileName);

                    // 对文件进行打包
                    string target_dir = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("\\"));
                    Pack pack = new Pack();
                    foreach (var item in con_file.GetList())
                    {
                        pack.AddSourceFile(item.GetName());
                    }
                    pack.PackFile(target_dir + "\\" + MainWindow.con_file.GetPackageName());
                }
                
                ShowMainPage();
            }
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
                relative_path = dic.FullName.Substring(AppDomain.CurrentDomain.BaseDirectory.Length);
                foreach (DirectoryInfo item in info)
                {
                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = item.Name;
                    tvi.Tag = item.FullName;
                    treeViewItem.Items.Add(tvi);
                }
                
                var config_list = new List<ConfigureGridData>();
                config_list.Clear();

                ObservableCollection<ConfigureGridData> config_grid_list = new ObservableCollection<ConfigureGridData>(config_list);
                config_grid_list.Clear();
                //获取目录下文件列表
                System.IO.FileInfo[] fileinfo = dic.GetFiles();
                //遍历文件列表
                foreach (System.IO.FileInfo it in fileinfo)
                {
                    bool exist = false;
                    string update_type = "重启后覆盖";
                    foreach (var item in con_file.GetList())
                    {
                        // 文件名是以相对路径的形式保存的，这里对相对路径名进行比较
                        string tmp_name = string.IsNullOrEmpty(relative_path) ? it.Name : relative_path + "\\" + it.Name;
                        if (item.GetName() == tmp_name)
                        {
                            exist = true;
                            item.SetHash(app_info.CreateMD5(string.IsNullOrEmpty(relative_path) ? it.Name : relative_path + "\\" + it.Name));
                            update_type = item.GetUpdateMethod() == ProjectFile.UpdateMethod.RUNNING ? "运行时覆盖" : "重启后覆盖";
                        }
                    }
                    config_grid_list.Add(new ConfigureGridData {
                        Picked = exist,
                        File = it.Name,
                        Version = "1.0",
                        Time = it.LastWriteTime.ToString(),
                        Hash = app_info.CreateMD5(string.IsNullOrEmpty(relative_path) ? it.Name : relative_path + "\\" + it.Name),
                        UpdateType = update_type});
                }
                config_data_grid.ItemsSource = config_grid_list;
            }
            catch
            {
            }

        }

        // checkbox选中事件
        public void per_row_checkbox_checked(object sender, RoutedEventArgs e)
        {
            //获取当前选中行
            var selected_item = config_data_grid.SelectedItem as ConfigureGridData;
            if (selected_item != null && selected_item.Picked == false)
            {
                selected_item.Picked = true;
                string name = string.IsNullOrEmpty(relative_path) ? selected_item.File : relative_path + "\\" + selected_item.File;
                foreach (var item in con_file.GetList())
                {
                    if (item.GetName() == name)
                        return;
                }
                float version = float.Parse(selected_item.Version);
                string hash = selected_item.Hash;

                ProjectFile.UpdateMethod u_method;
                if (selected_item.UpdateType == null)
                {
                    u_method = ProjectFile.UpdateMethod.REBOOT;
                    selected_item.UpdateType = "重启后覆盖";
                }
                else
                {
                    u_method = selected_item.UpdateType == "重启后覆盖" ? ProjectFile.UpdateMethod.REBOOT : ProjectFile.UpdateMethod.RUNNING;
                }

                ProjectFile project_info = new ProjectFile(name, version, hash, u_method);
                con_file.AddFile(project_info);
                
            }

        }

        // checkbox取消事件
        public void per_row_checkbox_unchecked(object sender, RoutedEventArgs e)
        {
            var a = config_data_grid.SelectedItem as ConfigureGridData;

            if (a.Picked == true)
            {
                a.Picked = false;
                a.UpdateType = null;
                for (int i = con_file.GetFileCount() - 1; i >= 0; i--)
                {
                    if (con_file.GetList()[i].GetName() == a.File)
                    {
                        con_file.GetList().Remove(con_file.GetList()[i]);
                    }
                }
            }
            
        }

        // combobox内容改变事件
        public void combobox_changed(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as System.Windows.Controls.ComboBox;
            var selected_item = this.config_data_grid.SelectedItem as ConfigureGridData;
            if (combobox.SelectedValue != null)
            {
                selected_item.UpdateType = combobox.SelectedValue.ToString();

                foreach (var item in con_file.GetList())
                {
                    // 比较相对路径名
                    string tmp_name = string.IsNullOrEmpty(relative_path) ? selected_item.File : relative_path + "\\" + selected_item.File;
                    if (item.GetName() == tmp_name)
                        item.SetUpdateMethod(selected_item.UpdateType == "重启后覆盖" ? ProjectFile.UpdateMethod.REBOOT : ProjectFile.UpdateMethod.RUNNING);
                }
            }
            
        }

        // 输入的版本不合法或不高于当前版本时，会进行提示
        private void version_TextChanged(object sender, TextChangedEventArgs e)
        {
            float result = 0;
            if (float.TryParse(set_version_box.Text, out result))
            {
                if (result > app_info.GetVersion())
                    label_warnning.Visibility = Visibility.Hidden;
                else
                    label_warnning.Visibility = Visibility.Visible;
            }
            else
            {
                label_warnning.Visibility = Visibility.Hidden;
            }
        }

        private void info_display(string version, string file_count, string time, string hash)
        {
            info_display_text.Text = "\n当前版本： " + version +
                                    "\n\n文件数量： " + file_count +
                                    "\n\n版本发布时间： " + time +
                                    "\n\n配置文件哈希：\n" + hash;
        }

        // 测试各种功能
        public void test(object sender, RoutedEventArgs e)
        {
            
        }

        
    }
}
