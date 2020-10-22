using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;
using ConsoleApp1.Disassembler.Platform.Win32;

namespace ConsoleApp1.Disassembler
{
	internal readonly struct SymbolHelper
	{
		private readonly ProcessWrapper _process;

		private SymbolHelper(ProcessWrapper process)
		{
			_process = process;
		}

		public static bool Create(ProcessWrapper process, NativeEnums.SymbolOptions options, ReadOnlySpan<char> localSymbolCache, ReadOnlySpan<char> searchPath, out SymbolHelper symbolHelper)
		{
			const string cacheToServerPrefix = @"srv*";
			const string microsoftSymbolStore = @"https://msdl.microsoft.com/download/symbols";
			var defaultPath = Environment.GetEnvironmentVariable("_NT_SYMBOL_PATH").AsSpan();
			var tmp = Ptr.OfRef(stackalloc char[cacheToServerPrefix.Length + microsoftSymbolStore.Length + localSymbolCache.Length + searchPath.Length + 2 + (searchPath.Length != 0 ? 1 : 0) + defaultPath.Length]);
			cacheToServerPrefix.AsSpan().CopyTo(tmp);
			var offset = cacheToServerPrefix.Length;
			localSymbolCache.CopyTo(tmp + offset);
			offset += localSymbolCache.Length;
			tmp[offset++] = '*';
			microsoftSymbolStore.AsSpan().CopyTo(tmp + offset);
			offset += microsoftSymbolStore.Length;
			tmp[offset++] = ';';
			if (searchPath.Length != 0)
			{
				searchPath.CopyTo(tmp + offset);
				offset += searchPath.Length;
				tmp[offset++] = ';';
			}

			if (defaultPath.Length != 0)
			{
				defaultPath.CopyTo(tmp + offset);
			}

			NativeMethods.SymSetOptions(options);
			if (NativeMethods.SymInitialize(process.Handle, tmp, true) != 0)
			{
				symbolHelper = new SymbolHelper(process);
				return true;
			}
			else
			{
				var err = new Win32Exception(Marshal.GetLastWin32Error()).Message;
				symbolHelper = default;
				return false;
			}
		}

		public Ptr LoadSymbolsForModule(Ptr imageName, ulong dllBase, Ptr moduleName, Ptr hFile)
		{
			var address = NativeMethods.SymLoadModuleEx(_process.Handle, hFile, imageName, moduleName, dllBase, 0, Ptr.Null, 0);
			var error = Marshal.GetLastWin32Error();
			if (address.IsNull && error != 0) throw new Win32Exception(error);
			return address;
		}

		public int TryGetSymbolFromAddress(Ptr address, ref NativeStructs.SymbolInfo symbol, out ulong displacement)
		{
			symbol.SizeOfStruct = 88;
			symbol.MaxNameLen = 500;
			return NativeMethods.SymFromAddr(_process.Handle, address, out displacement, ref symbol);
		}
	}
}