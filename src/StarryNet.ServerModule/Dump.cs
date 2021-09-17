using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StarryNet.ServerModule
{
    public static class Dump
    {
        private static string serverName;
        private static string dumpPath;

        private static int dumpError;
        private static MiniDumpType dumpType;
        private static MinidumpExceptionInfo exceptionInfo;

        public static void Initialize(string serverName, string path)
        {
            Dump.serverName = serverName;
            dumpPath = path;

            System.AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Write();
            };
        }

        public static bool Write()
        {
            return Write(MiniDumpType.WithFullMemory);
        }

        public static bool Write(MiniDumpType dumpType)
        {
            Dump.dumpType = dumpType;

            exceptionInfo.ThreadId = GetCurrentThreadId();
            exceptionInfo.ExceptionPointers = Marshal.GetExceptionPointers();
            exceptionInfo.ClientPointers = false;
            Thread t = new Thread(new ThreadStart(MakeDump));
            t.Start();
            t.Join();
            return dumpError == 0;
        }

        private static void MakeDump()
        {
            using (FileStream stream = new FileStream($"{dumpPath}{serverName} {DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss")}.dmp", FileMode.Create))
            {
                Process process = Process.GetCurrentProcess();

                IntPtr mem = Marshal.AllocHGlobal(Marshal.SizeOf(exceptionInfo));
                Marshal.StructureToPtr(exceptionInfo, mem, false);

                Boolean res = MiniDumpWriteDump(process.Handle,
                                                process.Id,
                                                stream.SafeFileHandle.DangerousGetHandle(),
                                                dumpType,
                                                exceptionInfo.ClientPointers ? mem : IntPtr.Zero,
                                                IntPtr.Zero,
                                                IntPtr.Zero);

                dumpError = res ? 0 : Marshal.GetLastWin32Error();
                Marshal.FreeHGlobal(mem);
            }
        }

        [DllImport("DbgHelp.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        private static extern Boolean MiniDumpWriteDump(
                                    IntPtr hProcess,
                                    Int32 processId,
                                    IntPtr fileHandle,
                                    MiniDumpType dumpType,
                                    IntPtr excepInfo,
                                    IntPtr userInfo,
                                    IntPtr extInfo);

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        [StructLayout(LayoutKind.Sequential)]
        struct MinidumpExceptionInfo
        {
            public Int32 ThreadId;
            public IntPtr ExceptionPointers;
            public bool ClientPointers;
        }
    }

    public enum MiniDumpType
    {
        Normal = 0x00000000,
        WithDataSegs = 0x00000001,
        WithFullMemory = 0x00000002,
        WithHandleData = 0x00000004,
        FilterMemory = 0x00000008,
        ScanMemory = 0x00000010,
        WithUnloadedModules = 0x00000020,
        WithIndirectlyReferencedMemory = 0x00000040,
        FilterModulePaths = 0x00000080,
        WithProcessThreadData = 0x00000100,
        WithPrivateReadWriteMemory = 0x00000200,
        WithoutOptionalData = 0x00000400,
        WithFullMemoryInfo = 0x00000800,
        WithThreadInfo = 0x00001000,
        WithCodeSegs = 0x00002000,
        WithoutAuxiliaryState = 0x00004000,
        WithFullAuxiliaryState = 0x00008000
    }
}