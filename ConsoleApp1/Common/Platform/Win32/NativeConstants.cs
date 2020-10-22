namespace ConsoleApp1.Common.Platform.Win32
{
	internal static class NativeConstants
	{
		public const int MaxFileNameSize = 260;
		public const ulong InvalidHandleValue = unchecked((ulong)-1);
		public const uint CreateSuspended = 0x00000004;
		public const int ProcessVmRead = 0x10;
		public const int ProcessQueryInformation = 0x0400;
	}
}