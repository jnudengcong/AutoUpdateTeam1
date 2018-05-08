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

            var item_list = new List<MainData>();
            item_list.Add(new MainData { ConFile = "ConFile1", Version = "v1.0", Time = "2017.10.13", Hash = "ff3e568a272ca876d331675fd3d5a6c2" });
            item_list.Add(new MainData { ConFile = "ConFile2", Version = "v1.02", Time = "2017.11.13", Hash = "3431f59cbcae719ef1aa00d40ac166d8" });
            item_list.Add(new MainData { ConFile = "ConFile3", Version = "v1.06", Time = "2017.11.29", Hash = "7889e750271c12eca0dfd8be3639c6b1" });
            item_list.Add(new MainData { ConFile = "ConFile4", Version = "v1.2", Time = "2017.12.20", Hash = "cb0c2ee777054e54c51420dfc2e2cf82" });


            data_grid.ItemsSource = item_list;
        }

        public class MainData
        {
            public string ConFile { get; set; }
            public string Version { get; set; }
            public string Time { get; set; }
            public string Hash { get; set; }
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //await Task.Delay(1000);
           // UpdateWindow update_window = new UpdateWindow();
            //update_window.ShowDialog();
        }

        private void ShowMainPage(object sender, RoutedEventArgs e)
        {
            main_page.Visibility = Visibility.Visible;
            configure_page.Visibility = Visibility.Hidden;
        }

        private void ShowConfigurePage(object sender, RoutedEventArgs e)
        {
            main_page.Visibility = Visibility.Hidden;
            configure_page.Visibility = Visibility.Visible;
        }

        private void ShowUrlWindow(object sender, RoutedEventArgs e)
        {
            UrlWindow urlWindow = new UrlWindow();
            urlWindow.ShowDialog();
        }

    }
}
