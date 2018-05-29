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
                    file_download.DownloadPackage();
                    ConFile install_confile = new ConFile(install_ini);
                    string package_name = install_confile.GetPackageName() + ".zip";
                    Process.Start("UpdateAssistant", install_ini + " " + package_name);
                }
                
            }
            
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            if (File.Exists(install_ini))
                File.Delete(install_ini);
            this.Close();
        }
    }
}
