using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace AutoUpdate
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Window
    {
        AppInfo app_info = AppInfo.GetInstance();
        string install_ini;

        public UpdateWindow()
        {
            InitializeComponent();
            updating_page.Visibility = Visibility.Hidden;
            install_ini = app_info.GetInstallName();
            label_info.Content = "名称：自动更新系统\n来源：" + app_info.GetUrl();
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            prompt_page.Visibility = Visibility.Hidden;
            updating_page.Visibility = Visibility.Visible;

            
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
                                        if (reboot)
                                        {
                                            Trace.WriteLine(item_install.GetHash());
                                            Trace.WriteLine(item_version.GetHash());
                                        }
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
                    if (reboot)
                    {
                        Process.Start("UpdateAssistant", install_ini + " " + package_name + " " + ProjectFile.UpdateMethod.REBOOT);
                    }
                    else
                    {
                        Process.Start("UpdateAssistant", install_ini + " " + package_name + " " + ProjectFile.UpdateMethod.RUNNING);
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
