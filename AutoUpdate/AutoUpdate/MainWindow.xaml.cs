using System;
using System.Collections.Generic;
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

            config_page.Visibility = Visibility.Hidden;

            var main_grid_list = new List<MainGridData>();
            main_grid_list.Add(new MainGridData { ConFile = "ConFile1", Version = "v1.0", Time = "2017.10.13", Hash = "ff3e568a272ca876d331675fd3d5a6c2" });
            main_grid_list.Add(new MainGridData { ConFile = "ConFile2", Version = "v1.02", Time = "2017.11.13", Hash = "3431f59cbcae719ef1aa00d40ac166d8" });
            main_grid_list.Add(new MainGridData { ConFile = "ConFile3", Version = "v1.06", Time = "2017.11.29", Hash = "7889e750271c12eca0dfd8be3639c6b1" });
            main_grid_list.Add(new MainGridData { ConFile = "ConFile4", Version = "v1.2", Time = "2017.12.20", Hash = "cb0c2ee777054e54c51420dfc2e2cf82" });

            main_data_grid.ItemsSource = main_grid_list;

            var config_grid_list = new List<ConfigureGridData>();
            config_grid_list.Add(new ConfigureGridData { Picked = true, File = "README.md", Version = "1.03", Time = "2017.11.13", Hash = "26205fa396afae7e698346556c23f256" });
            config_grid_list.Add(new ConfigureGridData { Picked = false, File = "aus.exe", Version = "2.11", Time = "2018.4.24", Hash = "36b4fa0153d9fe18c77dc124dc593143" });

            config_data_grid.ItemsSource = config_grid_list;
        }

        public class MainGridData
        {
            public string ConFile { get; set; }
            public string Version { get; set; }
            public string Time { get; set; }
            public string Hash { get; set; }
        }

        public class ConfigureGridData
        {
            public bool Picked { get; set; }
            public string File { get; set; }
            public string Version { get; set; }
            public string Time { get; set; }
            public string Hash { get; set; }
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);
            UpdateWindow update_window = new UpdateWindow();
            update_window.ShowDialog();
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
        }

        private void ShowUrlWindow(object sender, RoutedEventArgs e)
        {
            UrlWindow urlWindow = new UrlWindow();
            urlWindow.ShowDialog();
        }

        private void ShowVersionWindow(object sender, RoutedEventArgs e)
        {
            VersionWindow versionWindow = new VersionWindow();
            versionWindow.ShowDialog();
            main_page.Visibility = Visibility.Visible;
            config_page.Visibility = Visibility.Hidden;
        }

        private void Shutdown(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

    }
}
