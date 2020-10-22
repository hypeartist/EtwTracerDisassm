using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw.Packets
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "NotAccessedField.Global")]
	public unsafe readonly ref struct MethodILToNativeMapPacket
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private readonly struct MethodILToNativeMapNativeData
		{
			public readonly ulong MethodId;
			public readonly ulong ReJitId;
			public readonly byte MethodExtent;
			public readonly ushort CountOfMapEntries;
			public readonly int OffsetsBlob;
		}

		public readonly int ProcessId;
		public readonly ulong MethodId;
		public readonly ulong ReJitId;
		public readonly byte MethodExtent;
		public readonly ReadOnlySpan<int> ILOffsets;
		public readonly ReadOnlySpan<int> NativeOffsets;

		internal MethodILToNativeMapPacket(int processId, Ptr<byte> userData)
		{
			ProcessId = processId;

			var data = userData.As<MethodILToNativeMapNativeData>();
			MethodId = data.Value.MethodId;
			ReJitId = data.Value.ReJitId;
			MethodExtent = data.Value.MethodExtent;

			// var ilOffsetsStart = (byte*)Unsafe.AsPointer(ref Unsafe.AsRef(data->OffsetsBlob));
			// var nativeOffsetsStart = ilOffsetsStart + data->CountOfMapEntries * sizeof(int);
			var ilOffsetsStart = Ptr.OfReadOnlyRef(data.Value.OffsetsBlob).As<byte>();
			var nativeOffsetsStart = ilOffsetsStart + data.Value.CountOfMapEntries * sizeof(int);


			ILOffsets = new ReadOnlySpan<int>((void*) ilOffsetsStart.AsIntPtr(), data.Value.CountOfMapEntries);
			NativeOffsets = new ReadOnlySpan<int>((void*) nativeOffsetsStart.AsIntPtr(), data.Value.CountOfMapEntries);
		}
	}
}