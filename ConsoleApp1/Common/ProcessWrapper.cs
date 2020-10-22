using System;
using System.IO;
using System.Runtime.CompilerServices;
using ConsoleApp1.ClrTracer;
using ConsoleApp1.Common.Platform.Win32;

namespace ConsoleApp1.Common
{
	public readonly struct ProcessWrapper
	{
		public readonly Ptr Handle;
		public readonly int Id;

		internal ProcessWrapper(Ptr handle, int id)
		{
			Handle = handle;
			Id = id;
		}

		public bool TryFindModuleByName(ReadOnlySpan<char> moduleName, ref Span<char> fileNameBuffer, out Ptr handle, out ushort machineType)
		{
			handle = default;
			machineType = default;
			var queryHandle = NativeMethods.OpenProcess(NativeConstants.ProcessVmRead | NativeConstants.ProcessQueryInformation, false, Id);
			NativeMethods.EnumProcessModules(queryHandle, IntPtr.Zero, 0, out var needed);
			var moduleCount = needed / Unsafe.SizeOf<Ptr>();
			var moduleHandles = Ptr.OfRef(stackalloc Ptr[(int)needed]);
			NativeMethods.EnumProcessModules(queryHandle, moduleHandles, needed, out _);

			for (var i = 0; i < moduleCount; i++)
			{
				NativeMethods.GetModuleFileNameEx(queryHandle, moduleHandles[i], Ptr.OfRef(ref fileNameBuffer[0]), NativeConstants.MaxFileNameSize);
				var fileName = fileNameBuffer.Slice(0, FindStringLength(Ptr.OfRef(ref fileNameBuffer[0])));
				if (!Path.GetFileNameWithoutExtension(fileName).SequenceEqual(moduleName)) continue;
				fileNameBuffer = fileName;
				handle = moduleHandles[i];
				Read<PeFormat.ImageDosHeader>(handle, out var dosHeader);
				Read<PeFormat.ImageFileHeader>(handle.As<byte>() + dosHeader.LfaNew + sizeof(int), out var fileHeader);
				machineType = fileHeader.Machine;
				return true;
			}
			NativeMethods.CloseHandle(queryHandle);
			return false;

			static int FindStringLength(Ptr<char> start)
			{
				var pos = 0;
				while (pos < NativeConstants.MaxFileNameSize - 1)
				{
					if (pos > 0 && start[pos] == 0)
					{
						return pos;
					}

					pos++;
				}

				return NativeConstants.MaxFileNameSize;
			}
		}
		//
		// public bool TryFindModuleByName(ReadOnlySpan<char> moduleName, out ModuleInfo moduleInfo)
		// {
		// 	moduleInfo = default;
		// 	var queryHandle = NativeMethods.OpenProcess(NativeConstants.ProcessVmRead | NativeConstants.ProcessQueryInformation, false, Id);
		// 	NativeMethods.EnumProcessModules(queryHandle, IntPtr.Zero, 0, out var needed);
		// 	var moduleCount = needed / Unsafe.SizeOf<Ptr>();
		// 	var moduleHandles = Ptr.OfRef(stackalloc Ptr[(int)needed]);
		// 	NativeMethods.EnumProcessModules(queryHandle, moduleHandles, needed, out _);
		//
		// 	for (var i = 0; i < moduleCount; i++)
		// 	{
		// 		ModuleFileNameStorage.Clear();
		// 		NativeMethods.GetModuleFileNameEx(queryHandle, moduleHandles[i], ref Ptr.OfReadOnlyRef(ModuleFileNameStorage).Value, NativeConstants.MaxFileNameSize);
		// 		var fileName = ModuleFileNameStorage.AsReadOnlySpan().GetString();
		// 		if (!Path.GetFileNameWithoutExtension(fileName).SequenceEqual(moduleName)) continue;
		// 		moduleInfo = new ModuleInfo(this, fileName, moduleHandles[i]);
		// 		return true;
		// 	}
		// 	NativeMethods.CloseHandle(queryHandle);
		// 	return false;
		// }

		public bool Read<T>(Ptr address, out T t)
			where T : unmanaged
		{
			t = default;
			return NativeMethods.ReadProcessMemory(Handle, address, Ptr.OfRef(ref t), Unsafe.SizeOf<T>(), out _);
		}

		public bool Read(Ptr address, Ptr buffer, int count, out int read)
		{
			return NativeMethods.ReadProcessMemory(Handle, address, buffer, count, out read);
		}
	}
}