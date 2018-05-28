using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdate
{

    public class ProjectFile
    {
        public enum UpdateMethod { PARTIAL, WHOLE, REBOOT }
        private string name;
        private float version;
        private string hash;
        private UpdateMethod update_method;

        public ProjectFile(string name, float version, string hash, UpdateMethod update_method)
        {
            this.name = name;
            this.version = version;
            this.hash = hash;
            this.update_method = update_method;
        }

        public string GetName()
        {
            return this.name;
        }

        public float GetVersion()
        {
            return this.version;
        }

        public string GetHash()
        {
            return this.hash;
        }

        public void SetUpdateMethod(UpdateMethod update_method)
        {
            this.update_method = update_method;
        }

        public UpdateMethod GetUpdateMethod()
        {
            return this.update_method;
        }

    }
}
