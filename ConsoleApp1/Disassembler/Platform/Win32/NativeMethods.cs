using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.Disassembler.Platform.Win32
{
	internal static class NativeMethods
	{
		[DllImport("dbghelp", SetLastError = true)]
		public static extern NativeEnums.SymbolOptions SymSetOptions(NativeEnums.SymbolOptions options);

		[DllImport("dbghelp", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SymInitializeW", ExactSpelling = true)]
		public static extern int SymInitialize(Ptr hProcess, Ptr searchPath, bool invadeProcess);

		[DllImport("dbghelp", SetLastError = true, EntryPoint = "SymLoadModuleExW", ExactSpelling = true, CharSet = CharSet.Unicode)]
		public static extern Ptr SymLoadModuleEx(Ptr hProcess, Ptr hFile, Ptr imageName, Ptr moduleName, ulong baseOfDll, uint dllSize, Ptr data, uint flags);

		[DllImport("dbghelp", SetLastError = true)]
		public static extern int SymFromAddr(Ptr hProcess, Ptr address, out ulong displacement, ref NativeStructs.SymbolInfo symbol);
	}
}