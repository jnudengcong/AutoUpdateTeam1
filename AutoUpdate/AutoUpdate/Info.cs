using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdate
{
    class Info
    {
        private static Info instance;

        private string name;
        private float version;
        private string time;
        private string hash;
        private List<FileInfo> file_list;

        private string url;

        private static readonly object locker = new object();

        private Info()
        {
            // 读取当前版本的配置信息
            name = "version.ini";
            file_list = new List<FileInfo>();
            FileStream fs = new FileStream(this.name, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader m_streamReader = new StreamReader(fs);
            m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            // 读取配置文件自身的信息
            this.version = float.Parse(m_streamReader.ReadLine());
            this.time = m_streamReader.ReadLine();
            this.hash = m_streamReader.ReadLine();
            int file_count = int.Parse(m_streamReader.ReadLine());

            for (int i = 0; i < file_count; i++)
            {
                // 读取配置文件列表中的文件信息
                string name = m_streamReader.ReadLine();
                float version = float.Parse(m_streamReader.ReadLine());
                string hash = m_streamReader.ReadLine();
                // 先转为int，再强制转换为enum类型
                FileInfo.UpdateMethod update_method = (FileInfo.UpdateMethod)Enum.Parse(typeof(FileInfo.UpdateMethod), m_streamReader.ReadLine());
                file_list.Add(new FileInfo(name, version, hash, update_method));
            }
            m_streamReader.Close();

            // 从url.ini获取设置的网址
            fs = new FileStream(@"url.ini", FileMode.OpenOrCreate, FileAccess.Read);
            m_streamReader = new StreamReader(fs);
            m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            url = m_streamReader.ReadLine();
            m_streamReader.Close();
        }

        public static Info GetInstance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new Info();
                    }
                }
            }
            return instance;
        }

        public string GetUrl()
        {
            return url;
        }

        public void SaveUrl(string url)
        {
            this.url = url;
            FileStream fs = new FileStream(@"url.ini", FileMode.Create, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.Flush();
            //设置当前流的位置
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
            //写入内容
            m_streamWriter.Write(this.url);
            //关闭此文件
            m_streamWriter.Flush();
            m_streamWriter.Close();
        }

        public float GetVersion()
        {
            return this.version;
        }

        public string GetTime()
        {
            return this.time;
        }

        public string GetHash()
        {
            return this.hash;
        }

        public int GetFileCount()
        {
            return file_list.Count;
        }

        public List<FileInfo> GetList()
        {
            return file_list;
        }
    }
}
