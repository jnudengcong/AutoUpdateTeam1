using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace AutoUpdate
{
    class AppInfo
    {
        // 实例，用于单例模式
        private static AppInfo instance;
        

        // 当前版本的信息
        private string name;
        private float current_version;
        private string time;
        private string hash;
        private List<ProjectFile> file_list;

        // 配置文件历史
        private string history_file;    // history.ini，记录生成的配置文件的绝对路径
        private List<string> history_file_list;     // 记录history.ini中能找到的文件路径，找不到的丢弃，生成新的history.ini文件

        // url
        private string url;

        // install.ini
        private string install_ini;

        private static readonly object locker = new object();

        private AppInfo()
        {
            // 读取当前版本的配置信息
            name = "version.ini";
            file_list = new List<ProjectFile>();
            FileStream fs = new FileStream(this.name, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader stream_reader = new StreamReader(fs);
            stream_reader.BaseStream.Seek(0, SeekOrigin.Begin);
            // 读取配置文件自身的信息
            this.current_version = float.Parse(stream_reader.ReadLine());
            this.time = stream_reader.ReadLine();
            this.hash = stream_reader.ReadLine();
            int file_count = int.Parse(stream_reader.ReadLine());

            for (int i = 0; i < file_count; i++)
            {
                // 读取配置文件列表中的文件信息
                string name = stream_reader.ReadLine();
                float version = float.Parse(stream_reader.ReadLine());
                string hash = stream_reader.ReadLine();
                ProjectFile.UpdateMethod update_method = (ProjectFile.UpdateMethod)Enum.Parse(typeof(ProjectFile.UpdateMethod), stream_reader.ReadLine());
                file_list.Add(new ProjectFile(name, version, hash, update_method));
            }
            stream_reader.Close();


            // 从history.ini中读取配置文件历史
            history_file = "history.ini";
            history_file_list = new List<string>();
            fs = new FileStream(history_file, FileMode.OpenOrCreate, FileAccess.Read);
            stream_reader = new StreamReader(fs);
            stream_reader.BaseStream.Seek(0, SeekOrigin.Begin);
            string file;
            while ((file = stream_reader.ReadLine()) != null)
            {
                if (System.IO.File.Exists(file))
                {
                    history_file_list.Add(file);    // 能找到的文件才添加到列表中
                }
            }
            stream_reader.Close();
            SaveHistory();  // 读取完后把数据重新写回history.ini


            // 从url.ini获取设置的网址
            fs = new FileStream(@"url.ini", FileMode.OpenOrCreate, FileAccess.Read);
            stream_reader = new StreamReader(fs);
            stream_reader.BaseStream.Seek(0, SeekOrigin.Begin);
            url = stream_reader.ReadLine();
            stream_reader.Close();

            // install.ini
            install_ini = "install.ini";
        }

        public static AppInfo GetInstance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new AppInfo();
                    }
                }
            }
            return instance;
        }

        public void RefreshVersion()
        {
            file_list = new List<ProjectFile>();
            FileStream fs = new FileStream(this.name, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader stream_reader = new StreamReader(fs);
            stream_reader.BaseStream.Seek(0, SeekOrigin.Begin);
            // 读取配置文件自身的信息
            this.current_version = float.Parse(stream_reader.ReadLine());
            this.time = stream_reader.ReadLine();
            this.hash = stream_reader.ReadLine();
            int file_count = int.Parse(stream_reader.ReadLine());

            for (int i = 0; i < file_count; i++)
            {
                // 读取配置文件列表中的文件信息
                string name = stream_reader.ReadLine();
                float version = float.Parse(stream_reader.ReadLine());
                string hash = stream_reader.ReadLine();
                ProjectFile.UpdateMethod update_method = (ProjectFile.UpdateMethod)Enum.Parse(typeof(ProjectFile.UpdateMethod), stream_reader.ReadLine());
                file_list.Add(new ProjectFile(name, version, hash, update_method));
            }
            stream_reader.Close();
        }

        public void AddHistory(string file_path)
        {
            history_file_list.Add(file_path);
            SaveHistory();
        }

        public void RemoveHistory(string file_path)
        {
            if (File.Exists(file_path))
                File.Delete(file_path);
            for (int i = history_file_list.Count - 1; i >= 0; i--)
            {
                if (history_file_list[i] == file_path)
                {
                    history_file_list.Remove(file_path);
                }
            }
            SaveHistory();
        }

        public void SaveHistory()
        {
            FileStream fs = new FileStream(history_file, FileMode.Create, FileAccess.Write);
            StreamWriter stream_writer = new StreamWriter(fs);
            stream_writer.Flush();
            stream_writer.BaseStream.Seek(0, SeekOrigin.Begin);
            foreach (var item in history_file_list)
            {
                stream_writer.WriteLine(item);
            }
            stream_writer.Flush();
            stream_writer.Close();
        }

        public List<string> GetHistoryList()
        {
            return history_file_list;
        }

        public string GetUrl()
        {
            return url;
        }

        public string GetInstallName()
        {
            return install_ini;
        }

        public void SaveUrl(string url)
        {
            this.url = url;
            FileStream fs = new FileStream(@"url.ini", FileMode.Create, FileAccess.Write);
            StreamWriter stream_writer = new StreamWriter(fs);
            stream_writer.Flush();
            //设置当前流的位置
            stream_writer.BaseStream.Seek(0, SeekOrigin.Begin);
            //写入内容
            stream_writer.Write(this.url);
            //关闭此文件
            stream_writer.Flush();
            stream_writer.Close();
        }

        public string CreateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public void SaveVersion(ConFile con_file)
        {
        }

        public float GetVersion()
        {
            return current_version;
        }

        public string GetTime()
        {
            return time;
        }

        public string GetHash()
        {
            return hash;
        }

        public int GetFileCount()
        {
            return file_list.Count;
        }

        public List<ProjectFile> GetList()
        {
            return file_list;
        }
    }
}
