using System;
using System.IO;
using System.Linq;

namespace Construct.Server.Models.Data.CodeGeneration
{
    /// <summary>
    /// 
    /// </summary>
    public class DirectoryService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(string path)
        {
            string[] names = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            string directory = names.FirstOrDefault();

            if (!String.IsNullOrWhiteSpace(directory) && !directory.Contains(":"))
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            foreach (var name in names.Skip(1))
            {
                directory = String.Concat(directory, "\\", name);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }
    }
}