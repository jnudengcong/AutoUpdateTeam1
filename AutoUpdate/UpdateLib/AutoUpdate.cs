using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UpdateLib
{
    public class AutoUpdate
    {
        AppInfo app_info = AppInfo.GetInstance();

        private string main_app;
        private string url;
        private string install_ini;

        public AutoUpdate(string main_app, string url, string install_ini)
        {
            this.main_app = main_app;
            this.url = url;
            this.install_ini = install_ini;
        }

        public bool NewVersionExists()
        {
            FileDownload file_download = new FileDownload(app_info.GetUrl(), install_ini);
            if (file_download.URLExists() && file_download.GetVersion() > app_info.GetVersion())
            {
                return true;
            }
            else
            {
                if (File.Exists(install_ini))
                    File.Delete(install_ini);
                return false;
            }
        }

        public void Update()
        {
            UpdateClass update_class = new UpdateClass(main_app, install_ini, url);
            update_class.LauchAssistant();
        }
    }
}
