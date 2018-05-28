using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateAssistant
{
    class Program
    {
        static void Main(string[] args)
        {
            string package_name = args[0];
            string version_file = args[1];

            // 主程序退出后进行替换
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (var item in Process.GetProcesses())
                {
                    if (item.ProcessName == "AutoUpdate")
                    {
                        flag = true;
                    }   
                }
            }
            
            DirectoryInfo dic = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            FileInfo[] files = dic.GetFiles();
            foreach (var item in files)
            {
                if (Path.GetExtension(item.Name) == ".ini" || item.Name == "UpdateAssistant.exe" || item.Name == package_name)
                    continue;
                Console.WriteLine(item.Name);
                // 删除旧的文件
                File.Delete(item.Name);
            }
            DirectoryInfo[] dirs = dic.GetDirectories();
            Console.WriteLine();
            foreach (var item in dirs)
            {
                Console.WriteLine(item.Name);
                // 删除旧的文件夹
                Directory.Delete(item.Name, true);
            }

            Console.Read();
            if (File.Exists(package_name))
            {
                // 解压程序包
                ZipFile.ExtractToDirectory(package_name, AppDomain.CurrentDomain.BaseDirectory);
                // 解压完成后删除程序包
                // 暂时保留File.Delete(package_name);
            }

            if (File.Exists("version.ini"))
                File.Delete("version.ini");
            FileInfo file = new FileInfo(version_file);
            file.MoveTo("version.ini");

            Console.Read();
            
        }
    }
}
