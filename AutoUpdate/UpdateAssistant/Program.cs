using System;
using System.Collections.Generic;
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
            if (File.Exists(args[0]))
                ZipFile.ExtractToDirectory(args[0], ".\\");
        }
    }
}
