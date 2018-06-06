using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace AutoUpdate
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Window
    {
        AppInfo app_info = AppInfo.GetInstance();
        string install_ini;

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UpdateWindow()
        {
            InitializeComponent();
            updating_page.Visibility = Visibility.Hidden;
            running_update_page.Visibility = Visibility.Hidden;
            install_ini = app_info.GetInstallName();
            label_info.Content = "名称：自动更新系统\n来源：" + app_info.GetUrl();
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            if (File.Exists(install_ini))
            {
                FileDownload file_download = new FileDownload(app_info.GetUrl(), app_info.GetInstallName());
                if (file_download.URLExists())
                {
                    bool reboot = false;

                    file_download.DownloadPackage();
                    ConFile install_confile = new ConFile(install_ini);
                    ConFile version_confile = new ConFile("version.ini");
                    foreach (var item_install in install_confile.GetList())
                    {
                        if (reboot)
                            break;
                        if (item_install.GetUpdateMethod() == ProjectFile.UpdateMethod.REBOOT)
                        {
                            bool existed = false;
                            foreach (var item_version in version_confile.GetList())
                            {
                                if (item_install.GetName() == item_version.GetName())
                                {
                                    existed = true;
                                    if (existed)
                                    {
                                        if (item_install.GetHash() != item_version.GetHash())
                                            reboot = true;
                                    }
                                }
                            }
                            if (!existed)
                            {
                                reboot = true;
                            }
                        }
                    }
                    
                    string package_name = install_confile.GetPackageName();
                    string package_zip_name = package_name + ".zip";

                    if (File.Exists(package_zip_name))
                    {
                        // 解压程序包
                        ZipFile.ExtractToDirectory(package_zip_name, AppDomain.CurrentDomain.BaseDirectory + "\\" + package_name);
                        // 删除程序包
                        File.Delete(package_zip_name);

                        if (File.Exists(package_name + "\\UpdateAssistant.exe"))
                        {
                            if (File.Exists("UpdateAssistant.exe"))
                                File.Delete("UpdateAssistant.exe");
                            FileInfo file = new FileInfo(package_name + "\\UpdateAssistant.exe");
                            file.MoveTo("UpdateAssistant.exe");
                        }
                    }

                    if (reboot)
                    {
                        prompt_page.Visibility = Visibility.Hidden;
                        running_update_page.Visibility = Visibility.Hidden;
                        updating_page.Visibility = Visibility.Visible;
                        Process.Start("UpdateAssistant", install_ini + " " + package_name + " " + "REBOOT");
                        log.Info("Update Method: REBOOT");
                    }
                    else
                    {
                        Process.Start("UpdateAssistant", install_ini + " " + package_name + " " + "RUNNING");
                        log.Info("Update Method: RUNNING");
                        bool flag = true;
                        while (flag)
                        {
                            flag = false;
                            foreach (var item in Process.GetProcesses())
                            {
                                if (item.ProcessName == "UpdateAssistant")
                                {
                                    flag = true;
                                }
                            }
                        }
                        prompt_page.Visibility = Visibility.Hidden;
                        updating_page.Visibility = Visibility.Hidden;
                        running_update_page.Visibility = Visibility.Visible;
                    }
                    
                }
                
            }
            
        }

        
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AppShutdown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
