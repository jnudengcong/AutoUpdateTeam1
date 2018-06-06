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
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            
            string install_ini = args[0];
            string package_name = args[1];
            string update_method = args[2];

            ConFile install_confile = new ConFile(install_ini);
            ConFile version_confile = new ConFile("version.ini");
            if (update_method == "REBOOT")
            {
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
                    if (Path.GetExtension(item.Name) == ".ini" || Path.GetExtension(item.Name) == ".log" || item.Name == "UpdateAssistant.exe")
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

                
                foreach (var item in install_confile.GetList())
                {
                    if (item.GetName() == "UpdateAssistant.exe")
                        continue;
                    string target_dir = AppDomain.CurrentDomain.BaseDirectory;
                    if (item.GetName().LastIndexOf("\\") != -1)
                    {
                        target_dir += item.GetName().Substring(0, item.GetName().LastIndexOf("\\"));
                    }
                    if (!Directory.Exists(target_dir))
                        Directory.CreateDirectory(target_dir);
                    FileInfo f = new FileInfo(package_name + "\\" + item.GetName());
                    f.MoveTo(item.GetName());
                }

                if (Directory.Exists(package_name))
                    Directory.Delete(package_name, true);

                if (File.Exists("version.ini"))
                    File.Delete("version.ini");
                FileInfo file = new FileInfo(install_ini);
                file.MoveTo("version.ini");

                if (File.Exists("AutoUpdate.exe"))
                    Process.Start("AutoUpdate");

                
            }
            else
            {
                foreach (var item_version in version_confile.GetList())
                {
                    if (item_version.GetName() == "UpdateAssistant.exe")
                        continue;
                    if (item_version.GetUpdateMethod() == ProjectFile.UpdateMethod.REBOOT)
                        continue;
                    bool existed = false;
                    foreach (var item_install in install_confile.GetList())
                    {
                        if (item_version.GetName() == item_install.GetName())
                        {
                            existed = true;
                            if (item_version.GetHash() != item_install.GetHash())
                                if (File.Exists(item_version.GetName()))
                                    File.Delete(item_version.GetName());
                        }
                    }
                    if (!existed)
                        if (File.Exists(item_version.GetName()))
                            File.Delete(item_version.GetName());
                }

                foreach (var item in install_confile.GetList())
                {
                    if (!File.Exists(item.GetName()))
                    {
                        FileInfo file = new FileInfo(package_name + "\\" + item.GetName());
                        file.MoveTo(item.GetName());
                    }
                }

                if (Directory.Exists(package_name))
                    Directory.Delete(package_name, true);

                if (File.Exists("version.ini"))
                    File.Delete("version.ini");
                FileInfo f = new FileInfo(install_ini);
                f.MoveTo("version.ini");
            }

            log.Info("Update Success");
        }
    }
}
