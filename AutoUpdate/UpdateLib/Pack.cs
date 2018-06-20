using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace UpdateLib
{
    class Pack
    {
        List<string> source_file_list;

        public Pack()
        {
            source_file_list = new List<string>();
        }

        public void AddSourceFile(string source_file)
        {
            source_file_list.Add(source_file);
        }

        public void PackFile(string target_dir) // 传入目标文件夹
        {
            foreach (var source_file in source_file_list)
            {
                string source_dir = "";
                string target_file = Path.Combine(target_dir, source_file);
                if (source_file.LastIndexOf("\\") != -1)
                {
                    source_dir = source_file.Substring(0, source_file.LastIndexOf("\\"));
                }

                if (!System.IO.Directory.Exists(Path.Combine(target_dir, source_dir)))
                {
                    System.IO.Directory.CreateDirectory(Path.Combine(target_dir, source_dir));
                }

                File.Copy(source_file, target_file, true);
            }
            ZipFile.CreateFromDirectory(target_dir, target_dir + ".zip");
            if (System.IO.Directory.Exists(target_dir))
            {
                Directory.Delete(target_dir, true);
            }
        }

        public void ExtractFile(string zip_file)
        {
            ZipFile.ExtractToDirectory(zip_file, ".\\");
        }
    }
}
