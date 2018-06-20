using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace UpdateLib
{
    class UpdateClass
    {
        private string main_app;
        private string install_ini;
        private string url;

        public UpdateClass(string main_app, string install_ini, string url)
        {
            this.main_app = main_app;
            this.install_ini = install_ini;
            this.url = url;
        }

        public void LauchAssistant()
        {
            if (File.Exists(install_ini))
            {
                FileDownload file_download = new FileDownload(url, install_ini);
                if (file_download.URLExists())
                {
                    bool reboot = false;

                    file_download.DownloadPackage();
                    ConFile install_confile = new ConFile(install_ini);
                    ConFile version_confile = new ConFile("version.ini");
                    foreach (var item_install in install_confile.GetList())
                    {
                        if (reboot)
                            break;
                        if (item_install.GetUpdateMethod() == ProjectFile.UpdateMethod.REBOOT)
                        {
                            bool existed = false;
                            foreach (var item_version in version_confile.GetList())
                            {
                                if (item_install.GetName() == item_version.GetName())
                                {
                                    existed = true;
                                    if (existed)
                                    {
                                        if (item_install.GetHash() != item_version.GetHash())
                                            reboot = true;
                                    }
                                }
                            }
                            if (!existed)
                            {
                                reboot = true;
                            }
                        }
                    }

                    string package_name = install_confile.GetPackageName();
                    string package_zip_name = package_name + ".zip";

                    if (File.Exists(package_zip_name))
                    {
                        // 解压程序包
                        ZipFile.ExtractToDirectory(package_zip_name, AppDomain.CurrentDomain.BaseDirectory + "\\" + package_name);
                        // 删除程序包
                        File.Delete(package_zip_name);

                        if (File.Exists(package_name + "\\UpdateAssistant.exe"))
                        {
                            if (File.Exists("UpdateAssistant.exe"))
                                File.Delete("UpdateAssistant.exe");
                            FileInfo file = new FileInfo(package_name + "\\UpdateAssistant.exe");
                            file.MoveTo("UpdateAssistant.exe");
                        }
                    }

                    if (reboot)
                    {
                        Trace.WriteLine("reboottttttttt");
                        Process.Start("UpdateAssistant", main_app +  " " + install_ini + " " + package_name + " " + "REBOOT");
                    }
                    else
                    {
                        Trace.WriteLine("runninggggggggg");
                        
                        Process.Start("UpdateAssistant", main_app + " " + install_ini + " " + package_name + " " + "RUNNING");
                        Trace.WriteLine(main_app + install_ini + package_name);
                        bool flag = true;
                        while (flag)
                        {
                            flag = false;
                            foreach (var item in Process.GetProcesses())
                            {
                                if (item.ProcessName == "UpdateAssistant")
                                {
                                    flag = true;
                                }
                            }
                        }

                    }

                }

            }

        }
    }
}
