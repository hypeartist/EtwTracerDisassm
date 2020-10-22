using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Unicode;
using ConsoleApp1.ClrTracer.Com;
using ConsoleApp1.Common;
using ConsoleApp1.Common.Platform.Win32;

namespace ConsoleApp1.ClrData
{
	internal sealed class DacDistpatcher
	{
		private static readonly Guid DacInstanceGuid = new Guid("5c552ab6-fc09-4cb3-8e36-22fa03c798b7");

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate int CreateDacInstanceCallback([In, ComAliasName("REFIID")] in Guid riid, [In, MarshalAs(UnmanagedType.Interface)] IDacDataTarget data, [Out, MarshalAs(UnmanagedType.Interface)] out IUnknown ppObj);

		private readonly ISOSDac _sosDac;
		[SuppressMessage("ReSharper", "NotAccessedField.Local")]
		private readonly DacDataTarget _dacDataTarget;
		[SuppressMessage("ReSharper", "NotAccessedField.Local")]
		private readonly IXCLRDataProcess _clrDataProcess;

		private DacDistpatcher(DacDataTarget dacDataTarget, IXCLRDataProcess clrDataProcess, ISOSDac sosDac)
		{
			_dacDataTarget = dacDataTarget;
			_clrDataProcess = clrDataProcess;
			_sosDac = sosDac;
		}

		public unsafe static bool Create(ProcessWrapper process, out DacDistpatcher dacDistpatcher)
		{
			const string dacFileName = "mscordaccore.dll";
			const string engineName = "coreclr";
			const string dacInitProcName = "CLRDataCreateInstance";

			dacDistpatcher = null;
			var path = Ptr.OfRef(stackalloc char[NativeConstants.MaxFileNameSize]).AsWritableSpan(NativeConstants.MaxFileNameSize);
			if (!process.TryFindModuleByName(engineName, ref path, out var moduleHandle, out var machineType))
			{
				return false;
			}

			var dotnetHome = Path.GetDirectoryName(path);
			var dacPath = Ptr.OfRef(stackalloc char[dotnetHome.Length + dacFileName.Length + 1]);
			dotnetHome.CopyTo(dacPath);
			dacPath[dotnetHome.Length] = '\\';
			dacFileName.AsSpan().CopyTo(dacPath + dotnetHome.Length + 1);
			var dacDll = NativeMethods.LoadLibrary(dacPath);

			var createInstFn = NativeMethods.GetProcAddress(dacDll, dacInitProcName);
			var createInst = Marshal.GetDelegateForFunctionPointer<CreateDacInstanceCallback>(createInstFn.AsIntPtr());

			var dacDataTarget = new DacDataTarget(moduleHandle, machineType, process);
			if (createInst(DacInstanceGuid, dacDataTarget, out var iUnknown) != 0)
			{
				return false;
			}
			var clrDataProcess = Unsafe.As<IUnknown, IXCLRDataProcess>(ref iUnknown);
			var version = 0;
			const uint dacRequestVersion = 0xe0000000U;
			clrDataProcess.Request(dacRequestVersion, 0, Ptr.Null, sizeof(int), Ptr.OfRef(ref version));
			if (version != 9)
			{
				return false;
			}
			var sosDac = Unsafe.As<IXCLRDataProcess, ISOSDac>(ref clrDataProcess);
			dacDistpatcher = new DacDistpatcher(dacDataTarget, clrDataProcess, sosDac);
			return true;
		}

		public bool TryGetJitHelperFunctionName(Ptr ip, ref Span<char> buffer)
		{
			if (_sosDac.GetJitHelperFunctionName(ip, 0, Ptr.Null, out var needed) != 0)
			{
				buffer = Span<char>.Empty;
				return false;
			}

			Ptr.OfRef(buffer).Clear(buffer.Length);
			var tmpIn = Ptr.OfRef(stackalloc byte[needed]);
			if (_sosDac.GetJitHelperFunctionName(ip, needed, tmpIn, out _) != 0)
			{
				buffer = Span<char>.Empty;
				return false;
			}
			Utf8.ToUtf16(tmpIn.AsSpan(needed), buffer, out _, out _);
			buffer = buffer.Slice(0, needed - 1);
			return true;
		}

		public bool TryGetMethodByHandle(Ptr ppMd, ref Span<char> buffer)
		{
			if (_sosDac.GetMethodDescData(ppMd, Ptr.Null, out var methodDescData, 0, Ptr.Null, out _) != 0)
			{
				buffer = Span<char>.Empty;
				return false;
			}

			if (_sosDac.GetMethodTableData(methodDescData.MethodTable, out var methodTableData) != 0)
			{
				buffer = Span<char>.Empty;
				return false;
			}

			if (_sosDac.GetMethodTableName(methodDescData.MethodTable, 0, Ptr.Null, out var n1) != 0)
			{
				buffer = Span<char>.Empty;
				return false;
			}
			if (methodTableData.ComponentSize == 0)
			{
				for (var i = 0; i < methodTableData.NumMethods; i++)
				{
					if (_sosDac.GetMethodTableSlot(methodDescData.MethodTable, i, out var slot) != 0) continue;
					if (_sosDac.GetCodeHeaderData(slot, out var codeHeaderData) != 0) continue;
					if (codeHeaderData.MethodDesc != methodDescData.MethodDesc) continue;

					if (_sosDac.GetMethodDescData(codeHeaderData.MethodDesc, Ptr.Null, out var methodDescData2, 0, Ptr.Null, out _) != 0) continue;
					Ptr.OfRef(buffer).Clear(buffer.Length);
					if (_sosDac.GetMethodDescName(methodDescData2.MethodDesc, 0, Ptr.Null, out var needed) != 0) continue;
					if (_sosDac.GetMethodDescName(methodDescData2.MethodDesc, needed, Ptr.OfRef(buffer), out _) != 0) continue;
					buffer = buffer.Slice(0, needed - 1);
					return true;
				}
			}

			buffer = Span<char>.Empty;
			return false;
		}

		public bool TryGetMethodByInstructionPointer(Ptr ip, ref Span<char> buffer)
		{
			return _sosDac.GetMethodDescPtrFromIP(ip, out var ppMd) == 0 && TryGetMethodByHandle(ppMd, ref buffer);
		}
	}

	internal static class ComStructs
	{
		[StructLayout(LayoutKind.Sequential)]
		public readonly struct MethodDescData
		{
			public readonly uint HasNativeCode;
			public readonly uint IsDynamic;
			public readonly ushort SlotNumber;
			public readonly Ptr NativeCodeAddr;

			// Useful for breaking when a method is jitted.
			public readonly Ptr AddressOfNativeCodeSlot;

			public readonly Ptr MethodDesc;
			public readonly Ptr MethodTable;
			public readonly Ptr Module;

			public readonly uint MDToken;
			public readonly Ptr GCInfo;
			public readonly Ptr GCStressCodeCopy;

			// This is only valid if bIsDynamic is true
			public readonly Ptr ManagedDynamicMethodObject;

			public readonly Ptr RequestedIP;

			// Gives info for the single currently active version of a method
			public readonly RejitData RejitDataCurrent;

			// Gives info corresponding to requestedIP (for !ip2md)
			public readonly RejitData RejitDataRequested;

			// Total number of rejit versions that have been jitted
			public readonly uint JittedRejitVersions;
		}

		[StructLayout(LayoutKind.Sequential)]
		public readonly struct RejitData
		{
			private readonly Ptr RejitID;
			private readonly uint Flags;
			private readonly Ptr NativeCodeAddr;
		}

		[StructLayout(LayoutKind.Sequential)]
		public readonly struct MethodTableData
		{
			public readonly uint IsFree; // everything else is NULL if this is true.
			public readonly Ptr Module;
			public readonly Ptr EEClass;
			public readonly Ptr ParentMethodTable;
			public readonly ushort NumInterfaces;
			public readonly ushort NumMethods;
			public readonly ushort NumVtableSlots;
			public readonly ushort NumVirtuals;
			public readonly uint BaseSize;
			public readonly uint ComponentSize;
			public readonly uint Token;
			public readonly uint AttrClass;
			public readonly uint Shared; // flags & enum_flag_DomainNeutral
			public readonly uint Dynamic;
			public readonly uint ContainsPointers;
		}

		[StructLayout(LayoutKind.Sequential)]
		public readonly struct CodeHeaderData
		{
			public readonly Ptr GCInfo;
			public readonly uint JITType;
			public readonly Ptr MethodDesc;
			public readonly Ptr MethodStart;
			public readonly uint MethodSize;
			public readonly Ptr ColdRegionStart;
			public readonly uint ColdRegionSize;
			public readonly uint HotRegionSize;
		}
	}
}