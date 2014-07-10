using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace ConstructTcpServer
{
    public class Processes
    {
        public const uint PROCESS_KILLRIGHTS = 0x00000001;

        private const uint TH32CS_SNAPPROCESS = 0x00000002;		
        public Processes()
        {
            int x = 3;
            
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hSnapshot);

        [DllImport("Kernel32.dll")]
        public static extern int GetCurrentProcessId(); 
												
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        public static bool Kill(uint uiProcessId)
        {
            System.IntPtr handler = OpenProcess(PROCESS_KILLRIGHTS, false, uiProcessId);
            //System.Windows.Forms.MessageBox.Show("Handler is=" + handler.ToString());
            bool b = Processes.KillProcess(handler);
            Processes.CloseHandle(handler);
            return b;		
        }
		
        public static ArrayList GetProcessByName(string applicationName)
        {
            ArrayList processIds = new ArrayList();
            if (applicationName.Length > 15)
            { 
                applicationName = applicationName.Substring(0, 15);
            }

            IntPtr handle = IntPtr.Zero;
            try
            {
                // Create snapshot of the processes
                handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
                PROCESSENTRY32 info = new PROCESSENTRY32();
                info.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(PROCESSENTRY32));

                // Get the first process
                int first = Process32First(handle, ref info);
                // If we failed to get the first process, throw an exception
                if (first == 0)					
                {
                    return processIds;
                }
				
                do
                {
                    string strProcessName = System.IO.Path.GetFileNameWithoutExtension(info.szExeFile);
                    if (strProcessName.Length > 15)
                    {
                        strProcessName = strProcessName.Substring(0, 15);
                    }

                    if (string.Compare(strProcessName, applicationName, true) == 0)
                    {
                        processIds.Add(info.th32ProcessID);												
                    }
                }
                while (Process32Next(handle, ref info) != 0);
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseHandle(handle);
                handle = IntPtr.Zero;				
            }
            return processIds;
        }

        public static ArrayList GetProcess()
        {
            ArrayList processIds = new ArrayList();
											
            IntPtr handle = IntPtr.Zero;
            try
            {
                // Create snapshot of the processes
                handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
                PROCESSENTRY32 info = new PROCESSENTRY32();
                info.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(PROCESSENTRY32));

                // Get the first process
                int first = Process32First(handle, ref info);
                // If we failed to get the first process, throw an exception
                if (first == 0)					
                {
                    return processIds;
                }
				
                do
                {
                    string strProcessName = System.IO.Path.GetFileNameWithoutExtension(info.szExeFile);
					
                    processIds.Add(info.szExeFile);												
                }
                while (Process32Next(handle, ref info) != 0);
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseHandle(handle);
                handle = IntPtr.Zero;				
            }
            return processIds;
        }

        public static uint GetProcessHandler(string applicationName)
        {
            if (applicationName.Length > 15)
            {
                applicationName = applicationName.Substring(0, 15);
            }

            IntPtr handle = IntPtr.Zero;
            try
            {
                // Create snapshot of the processes
                handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
                PROCESSENTRY32 info = new PROCESSENTRY32();
                info.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(PROCESSENTRY32));

                // Get the first process
                int first = Process32First(handle, ref info);
                // If we failed to get the first process, throw an exception
                if (first == 0)					
                {
                    return 0;
                }
				
                do
                {
                    if (string.Compare(info.szExeFile,
                        applicationName, true) == 0)
                    {
                        return info.th32ModuleID;
                    }
                }
                while (Process32Next(handle, ref info) != 0);
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseHandle(handle);
                handle = IntPtr.Zero;				
            }
            return 0;
        }
		
        [DllImport("kernel32.dll")]
        private static extern int Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        private static extern int Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);
		
        private static bool KillProcess(System.IntPtr ProcessHandler)
        {
            return TerminateProcess(ProcessHandler, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESSENTRY32
        {
            public readonly uint th32ProcessID;
            public readonly uint th32ModuleID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public readonly string szExeFile;
            public uint dwSize;
        }
    }
}