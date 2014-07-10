using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ConstructTcpServer
{
    /// <summary>
    /// Object used to control a Windows Form.
    /// </summary>
    public class Window
    {
        public const uint PROCESS_KILLRIGHTS = 0x00000001;

        /// <summary>
        /// Win32 API Constants for ShowWindowAsync()
        /// </summary>
        private const uint TH32CS_SNAPPROCESS = 0x00000002;
        private const UInt32 INFINITE = 0xFFFFFFFF;
        private const UInt32 WAIT_ABANDONED = 0x00000080;
        private const UInt32 WAIT_OBJECT_0 = 0x00000000;
        private const UInt32 WAIT_TIMEOUT = 0x00000102;

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;
        /// <summary>
        /// The WM_CLOSE message is sent as a signal that a window or an application should terminate.
        /// </summary>
        private const uint WM_CLOSE = 0x0010;

        private IntPtr m_hPreviousActiveWnd;
        private bool m_Visible = true;
        private bool m_WasMax = false;

        /// <summary>
        /// Constructs a Window Object
        /// </summary>
        /// <param name="Title">Title Caption</param>
        /// <param name="hWnd">Handle</param>
        /// <param name="Process">Owning Process</param>
        public Window(string Title, IntPtr hWnd, string Process)
        {
            Title = Title;
            hWnd = hWnd;
            Process = Process;
        }

        //public delegate bool EnumWindowsProc(IntPtr hWnd, uint lParam);
        //private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
        private delegate bool EnumWindowsProc(int hWnd, int lParam);

        /// <summary>
        /// Window Object's Public Properties
        /// </summary>
        public IntPtr hWnd { get; private set; }
        public string Title { get; private set; }
        public string Process { get; private set; }

        /// <summary>
        /// Sets this Window Object's visibility
        /// </summary>
        public bool Visible
        {
            get
            {
                return m_Visible;
            }
            set
            {
                //show the window
                if (value == true)
                {
                    if (m_WasMax)
                    {
                        if (ShowWindowAsync(hWnd, SW_SHOWMAXIMIZED))
                        {
                            m_Visible = true;
                        }
                    }
                    else
                    {
                        if (ShowWindowAsync(hWnd, SW_SHOWNORMAL))
                        {
                            m_Visible = true;
                        }
                    }
                }
                //hide the window

                if (value == false)
                {
                    m_WasMax = IsZoomed(hWnd);
                    if (ShowWindowAsync(hWnd, SW_HIDE))
                    {
                        m_Visible = false;
                    }
                }
            }
        }

        public static Window GetWindowByTitle(string strTitle)
        {
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName(strTitle);
            if (p.Length > 0)
            {
                return new Window(strTitle, p[0].MainWindowHandle, string.Empty);
            }
            else
            {
                return null;
            }
        }

        public static bool KillProcess(string applicationName)
        {
            bool bResult = true;
            foreach (uint processId in GetProcessesByName(applicationName))
            {
                IntPtr hProcess = OpenProcess(PROCESS_KILLRIGHTS, false, processId);

                EnumWindowsProc ewp = new EnumWindowsProc(TerminateAppEnum);

                EnumWindows(ewp, processId);
                // Wait on the handle. If it signals, great. If it times out, then you kill it.
                if (WaitForSingleObject(hProcess, 5000) != WAIT_OBJECT_0)
                {
                    bResult = bResult && TerminateProcess(hProcess, 0);
                }
                CloseHandle(hProcess);
            }
            return bResult;
        }

        public static List<uint> GetProcessesByName(string applicationName)
        {
            // allocate one element in the list because we assume there's only one process
            List<uint> processIds = new List<uint>(1);
            //if (applicationName.Length > 15)
            //{
            //    applicationName = applicationName.Substring(0, 15);
            //}

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
                    //if (strProcessName.Length > 15)
                    //{
                    //    strProcessName = strProcessName.Substring(0, 15);
                    //}

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

        //Override ToString() 
        public override string ToString()
        {
            //return the title if it has one, if not return the process name
            if (Title.Length > 0)
            {
                return Title;
            }
            else
            {
                return Process;
            }
        }

        /// <summary>
        /// Sets focus to the window that had focus prior to requesting focus for this window
        /// </summary>
        public void Deactivate()
        {
            if (m_hPreviousActiveWnd != null)
            {
                Activate(m_hPreviousActiveWnd);
            }
        }
		
        /// <summary>
        /// Sets focus to this Window Object
        /// </summary>
        public void Activate()
        {
            m_hPreviousActiveWnd = GetForegroundWindow();

            Activate(hWnd);
        }

        /// <summary>
        /// Win32 API Imports
        /// </summary>
        [DllImport("user32.dll")]
        private static extern 
        bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern 
        bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern 
        bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern 
        bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern 
        IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        private static extern
        int EnumWindows(EnumWindowsProc ewp, uint lParam);

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //private static extern int PostMessage(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);
		
        [DllImport("kernel32", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr handle, Int32 milliseconds);

        [DllImport("kernel32.dll")]
        private static extern 
        IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern 
        IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        private static extern 
        int Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        private static extern 
        int Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern 
        bool CloseHandle(IntPtr hSnapshot);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        private static bool TerminateAppEnum(int hWnd, int lParam)
        //private static bool TerminateAppEnum(IntPtr hWnd, int lParam)
        {
            uint processId;
            GetWindowThreadProcessId((IntPtr)hWnd, out processId);

            if (processId == lParam)
            {
                PostMessage((IntPtr)hWnd, WM_CLOSE, 0, 0);
            }

            return true;
        }

        /// <summary>
        /// Sets focus to this Window Object
        /// </summary>
        private void Activate(IntPtr hWndToActivate)
        {
            if (hWndToActivate == GetForegroundWindow())
            {
                return;
            }

            IntPtr ThreadID1 = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
            IntPtr ThreadID2 = GetWindowThreadProcessId(hWndToActivate, IntPtr.Zero);

            //AttachThreadInput(ThreadID1,ThreadID2,1);
            SetForegroundWindow(hWndToActivate);

            if (IsIconic(hWndToActivate))
            {
                ShowWindowAsync(hWndToActivate, SW_RESTORE);
            }
            else
            {
                ShowWindowAsync(hWndToActivate, SW_SHOWNORMAL);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESSENTRY32
        {
            public readonly uint th32ProcessID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public readonly string szExeFile;
            public uint dwSize;
        }
    }
}