namespace ConsoleApp1.Etw.Platform.Win32
{
	internal static class NativeConstants
	{
		public const int MaxSessionNameSize = 1024;
		public const int MaxExtensionSize = 256;
		public const int MaxSessions = 64;
		public const uint EnableTraceParametersVersion2 = 2;
		public const uint EventTraceControlQuery = 0;
		public const uint EventTraceControlStop = 1;
		public const uint EventTraceRealTimeMode = 0x00000100;
		public const uint WnodeFlagTracedGuid = 0x00020000;
		public const uint ProcessTraceModeEventRecord = 0x10000000;
		public const uint ProcessTraceModeRawTimestamp = 0x00001000;
		public const uint ErrorWmiInstanceNotFound = 4201;
		public const uint EventControlCodeEnableProvider = 1;
		public const int EventFilterTypePid = unchecked((int)0x80000004);
	}
}