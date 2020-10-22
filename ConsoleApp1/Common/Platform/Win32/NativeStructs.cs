using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ConsoleApp1.Common.Platform.Win32
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	internal static class NativeStructs
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct SecurityAttributes
		{
			public int nLength;
			public IntPtr lpSecurityDescriptor;
			public int bInheritHandle;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct Startupinfo
		{
			public int cb;
			public IntPtr lpReserved;
			public IntPtr lpDesktop;
			public IntPtr lpTitle;
			public int dwX;
			public int dwY;
			public int dwXSize;
			public int dwYSize;
			public int dwXCountChars;
			public int dwYCountChars;
			public int dwFillAttribute;
			public int dwFlags;
			public short wShowWindow;
			public short cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ProcessInformation
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}
	}
}