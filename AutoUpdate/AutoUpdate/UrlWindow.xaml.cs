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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace AutoUpdate
{
    /// <summary>
    /// UrlWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UrlWindow : Window
    {
        AppInfo info = AppInfo.GetInstance();

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UrlWindow()
        {
            InitializeComponent();
            // 初始化url窗口中的网址
            url_text.Text = info.GetUrl();
        }

        private void SaveUrl(object sender, RoutedEventArgs e)
        {
            info.SaveUrl(url_text.Text);
            log.Info("New Url: " + url_text);
            this.Close();
        }

        // 重载函数，允许回车进行保存
        private void SaveUrl(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 这里的e之所以进行类型转换是为了第一个SaveUrl函数，否则会死循环
                SaveUrl(sender, (RoutedEventArgs)e);
            }
        }
    }
}
