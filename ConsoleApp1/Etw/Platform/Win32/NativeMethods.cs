using System;
using System.Runtime.InteropServices;
using System.Security;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw.Platform.Win32
{
	internal static class NativeMethods
	{
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int StartTraceW([Out] out ulong sessionHandle, [In] in CharBlob1024 sessionName, ref NativeStructs.EventTraceProperties properties);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int QueryAllTraces([In] Ptr propertyArray, [In] int propertyArrayCount, [In] [Out] ref int sessionCount);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int ControlTrace(ulong sessionHandle, in CharBlob1024 sessionName, ref NativeStructs.EventTraceProperties properties, uint controlCode);

		[DllImport("advapi32.dll", EntryPoint = "OpenTraceW", CharSet = CharSet.Unicode, SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		public static extern ulong OpenTrace([In] [Out] ref NativeStructs.EventTraceLogFileW logfile);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int ProcessTrace([In] Ptr handleArray, [In] uint handleCount, [In] ulong startTime, [In] ulong endTime);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int EnableTraceEx2([In] ulong traceHandle, [In] in Guid providerId, [In] uint controlCode, [In] byte level, [In] ulong matchAnyKeyword, [In] ulong matchAllKeyword, [In] int timeout, [In] ref NativeStructs.EnableTraceParameters enableParameters);
	}
}