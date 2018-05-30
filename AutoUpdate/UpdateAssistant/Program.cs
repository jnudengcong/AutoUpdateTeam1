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
            string package_zip_name = package_name + ".zip";
            ProjectFile.UpdateMethod update_method = (ProjectFile.UpdateMethod)Enum.Parse(typeof(ProjectFile.UpdateMethod), args[2]);
            Console.WriteLine(update_method);
            Console.Read();
            return;
            if (!File.Exists(install_ini) || !File.Exists(package_zip_name))
            {
                if (File.Exists(install_ini))
                    File.Delete(install_ini);
                if (File.Exists(package_zip_name))
                    File.Delete(package_zip_name);
                return;
            }

            Console.Read();
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

            if (File.Exists(package_zip_name))
            {
                Console.WriteLine(package_zip_name);
                Console.Read();
                // 解压程序包
                ZipFile.ExtractToDirectory(package_zip_name, AppDomain.CurrentDomain.BaseDirectory + "\\" + package_name);
                // 解压完成后删除程序包
                File.Delete(package_zip_name);
            }

            DirectoryInfo dic = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            FileInfo[] files = dic.GetFiles();
            foreach (var item in files)
            {
                if (Path.GetExtension(item.Name) == ".ini" || item.Name == "UpdateAssistant.exe" || item.Name == package_zip_name)
                    continue;
                // 删除旧的文件
                File.Delete(item.Name);
            }
            DirectoryInfo[] dirs = dic.GetDirectories();
            foreach (var item in dirs)
            {
                if (item.Name == package_name)
                    continue;
                Directory.Delete(item.Name, true);
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
