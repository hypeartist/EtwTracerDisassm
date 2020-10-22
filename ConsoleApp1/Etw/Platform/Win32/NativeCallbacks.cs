using System;
using System.Runtime.InteropServices;

namespace ConsoleApp1.Etw.Platform.Win32
{
	internal static class NativeCallbacks
	{
		public delegate bool EventTraceBufferCallback([In] IntPtr logfile);

		public delegate void EventTraceEventCallback([In] in NativeStructs.EventRecord rawData);
	}
}
