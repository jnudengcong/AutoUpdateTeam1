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
            if (args.Count() == 0)
                return;
            string install_ini = args[0];
            string package_name = args[1];
            if (!File.Exists(install_ini) || !File.Exists(package_name))
            {
                if (File.Exists(install_ini))
                    File.Delete(install_ini);
                if (File.Exists(package_name))
                    File.Delete(package_name);
                return;
            }
            if (package_name.IndexOf(".zip") == -1)
                package_name = package_name + ".zip";

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
                // 删除旧的文件
                File.Delete(item.Name);
            }
            DirectoryInfo[] dirs = dic.GetDirectories();
            foreach (var item in dirs)
            {
                Directory.Delete(item.Name, true);
            }
            
            if (File.Exists(package_name))
            {
                // 解压程序包
                ZipFile.ExtractToDirectory(package_name, AppDomain.CurrentDomain.BaseDirectory);
                // 解压完成后删除程序包
                File.Delete(package_name);
            }

            if (File.Exists("version.ini"))
                File.Delete("version.ini");
            FileInfo file = new FileInfo(install_ini);
            file.MoveTo("version.ini");
            
            if (File.Exists("AutoUpdate.exe"))
                Process.Start("AutoUpdate");
        }
    }
}
