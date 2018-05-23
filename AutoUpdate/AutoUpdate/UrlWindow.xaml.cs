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
        public UrlWindow()
        {
            InitializeComponent();
            // 初始化url窗口中的网址
            FileStream fs = new FileStream(@"url.ini", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader m_streamReader = new StreamReader(fs);
            m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            string strLine = m_streamReader.ReadLine();
            url_text.Text = strLine;
            m_streamReader.Close();
        }

        private void SaveUrl(object sender, RoutedEventArgs e)
        {
            FileStream fs = new FileStream(@"url.ini", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.Flush();
            //设置当前流的位置
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
            //写入内容
            m_streamWriter.Write(url_text.Text);
            //关闭此文件
            m_streamWriter.Flush();
            m_streamWriter.Close();
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
