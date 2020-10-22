using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ConsoleApp1.Common.Platform.Win32
{
	internal static class NativeMethods
	{
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int CreateProcess(Ptr lpApplicationName, Ptr lpCommandLine, in NativeStructs.SecurityAttributes lpProcessAttributes, in NativeStructs.SecurityAttributes lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref NativeStructs.Startupinfo lpStartupInfo, out NativeStructs.ProcessInformation lpProcessInformation);

		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int ResumeThread(Ptr hThread);

		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int TerminateProcess(Ptr hProcess, uint uExitCode);

		[DllImport("kernel32.dll")]
		[SuppressUnmanagedCodeSecurity]
		public static extern int RegisterWaitForSingleObject(ref Ptr phNewWaitObject, Ptr hObject, Ptr callback, Ptr context, ulong dwMilliseconds, ulong dwFlags);

		[DllImport("kernel32.dll")]
		[SuppressUnmanagedCodeSecurity]
		public static extern int UnregisterWaitEx([In] Ptr hWait, Ptr completionEvent);

		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int CloseHandle(Ptr hHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true, EntryPoint = "K32EnumProcessModules")]
		[SuppressUnmanagedCodeSecurity]
		public static extern int EnumProcessModules(Ptr hProcess, [Out] Ptr lphModule, uint cb, [MarshalAs(UnmanagedType.U4)] out uint lpcbNeeded);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "K32GetModuleFileNameExW")]
		[SuppressUnmanagedCodeSecurity]
		public static extern int GetModuleFileNameEx(Ptr hProcess, Ptr hModule, Ptr lpFilename, uint nSize);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "LoadLibraryW")]
		[SuppressUnmanagedCodeSecurity]
		public static extern Ptr LoadLibrary(Ptr lpLibFileName);

		[DllImport("kernel32.dll")]
		[SuppressUnmanagedCodeSecurity]
		public static extern Ptr GetProcAddress(Ptr hModule, string procedureName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory(Ptr hProcess, Ptr lpBaseAddress, Ptr lpBuffer, int nSize, out int lpNumberOfBytesRead);
	}
}