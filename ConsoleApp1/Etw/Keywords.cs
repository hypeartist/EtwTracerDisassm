using System;

namespace ConsoleApp1.Etw
{
	[Flags]
	public enum Keywords : long
	{
		Runtime = 0x00,
		AssemblyLoader = 0x04,
		Loader = 0x08,
		Jit = 0x10,
		JitTracing = 0x1000,
		Interop = 0x2000,
		JittedMethodIlToNativeMap = 0x20000,
		MethodDiagnostic = 0x4000000000,
	}
}