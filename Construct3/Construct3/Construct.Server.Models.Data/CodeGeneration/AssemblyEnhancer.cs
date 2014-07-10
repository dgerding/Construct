using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public class AssemblyEnhancer
    {
        /// <summary>
        /// Gets or sets the target assembly.
        /// </summary>
        /// <value>The target assembly.</value>
        public string TargetAssembly { get; set; }

        public string TargetAssemblyDirectory { get; set; }

        public string vEnhanceDirectory { get; set; }

        /// <summary>
        /// Enhances this instance.
        /// </summary>
        public bool Enhance()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            Process p = new Process();

            //string location = Assembly.GetExecutingAssembly().Location;
            //for (int i = 0; i < 4; i++)
            //{
            //    int index = location.LastIndexOf("\\", location.Length);
            //    if (index != -1)
            //        location = location.Remove(index);
            //}
            GenerateStrongNameKeyFile.Generate(Path.Combine(TargetAssemblyDirectory, "key.snk"), 1024);

            
            startInfo.FileName = Path.Combine(vEnhanceDirectory, "venhance.exe");
            startInfo.Arguments = String.Format("-assembly:\"{0}\" -keyFile:\"{1}\" -signassembly+",
                Path.Combine(TargetAssemblyDirectory, TargetAssembly),
                Path.Combine(TargetAssemblyDirectory, @"key.snk"));

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            p.EnableRaisingEvents = true;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo = startInfo;

            string standardOutput = "";
            string errorOutput = "";

            try
            {
                if (p.Start())
                {
                    var waitForExitTask = Task.Factory.StartNew(() =>
                    {
                        p.WaitForExit();
                    });

                    var readOutputTask = Task.Factory.StartNew(() =>
                    {
                        int c;
                        while ((c = p.StandardOutput.Read()) >= 0)
                        {
                            Debug.Write((char)c);
                            standardOutput += (char)c;
                        }
                    });

                    var readErrortTask = Task.Factory.StartNew(() =>
                    {
                        int c;

                        while ((c = p.StandardError.Read()) >= 0)
                        {
                            Debug.Write((char)c);
                            errorOutput += (char)c;
                        }
                    });

                    Task.WaitAll(readOutputTask, readErrortTask, waitForExitTask);

                    Debug.Write(standardOutput);
                    Debug.Write(errorOutput);

                    if (String.IsNullOrEmpty(errorOutput))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("venhance.exe was most likely not found in the requested path:" + vEnhanceDirectory);
                throw e;
            }
            return false;
        }
    }
}