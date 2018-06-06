using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdate
{
    class FileDownload
    {
        string url;
        string install_ini;
        string package_name;
        bool existed;

        public FileDownload(string url, string install_ini)
        {
            this.url = url;
            this.install_ini = install_ini;
            this.existed = false;
        }

        public bool URLExists()
        {
            if (url.IndexOf("http://") == 0)
            {
                // 判断install.ini是否存在
                WebRequest webRequest = WebRequest.Create(url + "/" + install_ini);
                webRequest.Timeout = 1200; // miliseconds
                webRequest.Method = "HEAD";

                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)webRequest.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                        existed = true;
                }
                catch (WebException webException)
                {

                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                }
            }
            else if (url.IndexOf("file://") == 0)
            {
                if (File.Exists(url.Substring(7) + "/" + install_ini))
                    existed = true;
            }
            return existed;
        }

        public void DownloadPackage()
        {
            WebClient web_client = new WebClient();
            string web_source = url + "/" + install_ini;
            web_client.DownloadFile(web_source, install_ini);
            if (File.Exists(install_ini))
            {
                ConFile install_confile = new ConFile(install_ini);
                package_name = install_confile.GetPackageName() + ".zip"; // 默认生成的名字不带.zip后缀
                web_source = url + "/" + package_name;
                web_client.DownloadFile(web_source, package_name);
            }
        }

        public float GetVersion()
        {
            if (existed)
            {
                WebClient web_client = new WebClient();
                string web_source = url + "/" + install_ini;
                web_client.DownloadFile(web_source, install_ini);
                ConFile install_confile = new ConFile(install_ini);
                return install_confile.GetVersion();
            }
            else
                return 0;
        }
    }
}
