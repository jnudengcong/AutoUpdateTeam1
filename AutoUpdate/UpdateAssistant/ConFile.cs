using System;
using System.Collections.Generic;
using System.IO;


namespace UpdateAssistant
{
    public class ConFile
    {
        private string name;
        private float version;
        private string time;
        private string hash;
        private List<ProjectFile> file_list;

        public ConFile(string name)
        {
            this.name = name;
            file_list = new List<ProjectFile>();
            ReadConFile();
        }


        public void AddFile(ProjectFile project_info)
        {
            file_list.Add(project_info);
        }

        // 生成配置文件
        public void SaveConFile()
        {
            FileStream fs = new FileStream(this.name, FileMode.Create, FileAccess.Write);
            StreamWriter stream_writer = new StreamWriter(fs);
            stream_writer.Flush();
            // 设置当前流的位置
            stream_writer.BaseStream.Seek(0, SeekOrigin.Begin);
            // 写入配置文件自身的信息
            stream_writer.Write(this.version.ToString() + "\n");
            stream_writer.Write(this.time.ToString() + "\n");
            stream_writer.Write(this.hash.ToString() + "\n");
            stream_writer.Write(this.file_list.Count.ToString() + "\n");

            foreach (var item in file_list)
            {
                // 写入配置文件列表中的文件信息
                stream_writer.Write(item.GetName().ToString() + "\n");
                stream_writer.Write(item.GetVersion().ToString() + "\n");
                stream_writer.Write(item.GetHash().ToString() + "\n");
                stream_writer.Write(item.GetUpdateMethod().ToString() + "\n");
            }
            //关闭此文件
            stream_writer.Flush();
            stream_writer.Close();

        }

        // 读取配置文件
        public void ReadConFile()
        {
            FileStream fs = new FileStream(this.name, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader stream_reader = new StreamReader(fs);
            stream_reader.BaseStream.Seek(0, SeekOrigin.Begin);
            // 读取配置文件自身的信息
            this.version = float.Parse(stream_reader.ReadLine());
            this.time = stream_reader.ReadLine();
            this.hash = stream_reader.ReadLine();
            int file_count = int.Parse(stream_reader.ReadLine());

            for (int i = 0; i < file_count; i++)
            {
                // 读取配置文件列表中的文件信息
                string name = stream_reader.ReadLine();
                float version = float.Parse(stream_reader.ReadLine());
                string hash = stream_reader.ReadLine();
                // 先转为int，再强制转换为enum类型
                ProjectFile.UpdateMethod update_method = (ProjectFile.UpdateMethod)Enum.Parse(typeof(ProjectFile.UpdateMethod), stream_reader.ReadLine());
                file_list.Add(new ProjectFile(name, version, hash, update_method));
            }

            //string strLine = stream_reader.ReadLine();
            stream_reader.Close();
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return this.name;
        }

        public string GetPackageName()
        {
            return "aus_v" + version.ToString();
        }

        public void SetVersion(float version)
        {
            this.version = version;
        }

        public float GetVersion()
        {
            return this.version;
        }

        public void SetTime(string time)
        {
            this.time = time;
        }

        public string GetTime()
        {
            return this.time;
        }

        public void SetHash(string hash)
        {
            this.hash = hash;
        }

        public string GetHash()
        {
            return this.hash;
        }

        public int GetFileCount()
        {
            return this.file_list.Count;
        }

        public List<ProjectFile> GetList()
        {
            return file_list;
        }
    }
}
