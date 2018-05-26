﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Diagnostics;  // 用于调试的类,使用Trace.Write(str)输出信息

namespace AutoUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    
    public partial class MainWindow : Window
    {
        AppInfo app_info = AppInfo.GetInstance();
        public static ConFile con_file;
        List<ConFile> con_file_list;

        public MainWindow()
        {
            InitializeComponent();

            // 初始化窗口时先把配置界面隐藏，也可以在xaml中设置，但是xaml的效果是实时的，不方便调试
            config_page.Visibility = Visibility.Hidden;

            ShowMainPage();
            info_display(app_info.GetVersion().ToString(), app_info.GetFileCount().ToString(), app_info.GetTime(), app_info.GetHash());

            List<string> comboBoxItems = new List<string>();
            comboBoxItems.Add("部分更新");
            comboBoxItems.Add("整体更新");
            comboBoxItems.Add("更新后重启");
            dataGridComboBoxColumn.ItemsSource = comboBoxItems;
            
            set_version_box.Text = (app_info.GetVersion() + 0.1f).ToString();
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
            if (con_install.GetVersion() > 3.3)
            {
                UpdateWindow update_window = new UpdateWindow();
               
                // 以对话框的形式打开
                update_window.ShowDialog();
            }
            
        }

        private void ShowMainPage(object sender, RoutedEventArgs e)
        {
            ShowMainPage();
        }

        private void ShowMainPage()
        {
            
            // 写死在主界面的数据，通过数据列表的绑定进行显示，还没试过动态删除，具体用法要结合xaml来看
            var main_grid_list = new List<MainGridData>();

            DirectoryInfo dic = new DirectoryInfo("ConFile");
            System.IO.FileInfo[] fileinfo = dic.GetFiles();
            //遍历文件列表
            con_file_list = new List<ConFile>();
            foreach (System.IO.FileInfo it in fileinfo)
            {
                con_file_list.Add(new ConFile("ConFile\\" + it.Name));
            }

            foreach (var it in con_file_list)
            {
                main_grid_list.Add(new MainGridData
                {

                    ConFile = it.GetName().Substring(it.GetName().LastIndexOf("\\") + 1),
                    Version = it.GetVersion().ToString(),
                    Time = it.GetTime(),
                    Hash = it.GetHash()
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

                string relative_name = "ConFile\\" + fileNameExt;
                MainWindow.con_file.SetName(sfd.FileName);
                MainWindow.con_file.SetHash(app_info.CreateMD5(relative_name));
                MainWindow.con_file.SaveConFile();
            }
            ShowMainPage();

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
                    string update_type = "部分更新";
                    foreach (var item in con_file.GetList())
                    {
                        if (item.GetName() == it.Name)
                        {
                            exist = true;
                            update_type = item.GetUpdateMethod() == FileInfo.UpdateMethod.PARTIAL ? "部分更新"
                                : item.GetUpdateMethod() == FileInfo.UpdateMethod.WHOLE ? "整体更新"
                                : "更新后重启";
                        }
                    }
                    config_grid_list.Add(new ConfigureGridData {
                        Picked = exist,
                        File = it.Name,
                        Version = "1.0",
                        Time = it.LastWriteTime.ToString(),
                        Hash = app_info.CreateMD5(it.Name),
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
                Trace.WriteLine(selected_item.Picked);
                Trace.WriteLine(selected_item.File);
                Trace.WriteLine(selected_item.UpdateType);

                string name = selected_item.File;
                foreach (var item in con_file.GetList())
                {
                    if (item.GetName() == name)
                        return;
                }
                float version = float.Parse(selected_item.Version);
                string hash = selected_item.Hash;

                FileInfo.UpdateMethod u_method;
                if (selected_item.UpdateType == null)
                {
                    u_method = FileInfo.UpdateMethod.PARTIAL;
                    selected_item.UpdateType = "部分更新";
                }
                else
                {
                    u_method = selected_item.UpdateType == "部分更新" ? FileInfo.UpdateMethod.PARTIAL
                           : selected_item.UpdateType == "整体更新" ? FileInfo.UpdateMethod.WHOLE
                           : FileInfo.UpdateMethod.REBOOT;
                }

                FileInfo file_info = new FileInfo(name, version, hash, u_method);
                con_file.AddFile(file_info);
                
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
                Trace.WriteLine(a.Picked);
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
                    if (selected_item.File == item.GetName())
                        item.SetUpdateMethod(selected_item.UpdateType == "部分更新" ? FileInfo.UpdateMethod.PARTIAL
                               : selected_item.UpdateType == "整体更新" ? FileInfo.UpdateMethod.WHOLE
                               : FileInfo.UpdateMethod.REBOOT);
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
    }
}
