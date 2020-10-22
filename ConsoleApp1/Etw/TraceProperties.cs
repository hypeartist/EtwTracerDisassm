using ConsoleApp1.Etw.Platform.Win32;

namespace ConsoleApp1.Etw
{
	internal struct TraceProperties
	{
		public NativeStructs.EventTraceLogFileW LogFile;
		public ulong TraceHandle;
	}
}