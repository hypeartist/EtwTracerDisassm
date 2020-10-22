using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw.Packets
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "NotAccessedField.Global")]
	public unsafe readonly ref struct MethodLoadVerbosePacket
	{
		private const byte OptimizationTierShift = 7;
		private const uint OptimizationTierLowMask = 0x7;
		private const byte MethodExtentShift = 28;
		private const uint MethodExtentLowMask = 0xf;

		private const uint OptimizationTierMask = OptimizationTierLowMask << OptimizationTierShift;
		private const uint MethodExtentMask = MethodExtentLowMask << MethodExtentShift;

		private const uint MethodFlagsMask = ~0u ^ (OptimizationTierMask | MethodExtentMask);

		public enum OptimizationTier : byte
		{
			Unknown,
			MinOptJitted,
			Optimized,
			QuickJitted,
			OptimizedTier1,
			ReadyToRun,
		}

		[Flags]
		public enum MethodFlags
		{
			None = 0,
			Dynamic = 0x1,
			Generic = 0x2,
			HasSharedGenericCode = 0x4,
			Jitted = 0x8,
			JitHelper = 0x10,
			ProfilerRejectedPrecompiledCode = 0x20,
			ReadyToRunRejectedPrecompiledCode = 0x40,
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private readonly struct MethodLoadVerboseNativeData
		{
			public readonly ulong MethodId;
			public readonly ulong ModuleId;
			public readonly ulong MethodStartAddress;
			public readonly int MethodSize;
			public readonly int MethodToken;
			public readonly int Flags;
			public readonly char TextBlobStart;
		}

		public readonly int ProcessId;
		public readonly Ptr MethodId;
		public readonly Ptr ModuleId;
		public readonly Ptr MethodStartAddress;
		public readonly int MethodSize;
		public readonly int MethodToken;
		public readonly MethodFlags Flags;
		public readonly OptimizationTier Tier;
		public readonly ReadOnlySpan<char> MethodNamespace;
		public readonly ReadOnlySpan<char> MethodName;
		public readonly ReadOnlySpan<char> MethodSignature;

		internal MethodLoadVerbosePacket(int processId, Ptr<byte> userData, ushort userDataLength)
		{
			ProcessId = processId;

			var data = userData.As<MethodLoadVerboseNativeData>();
			MethodId = (Ptr) data.Value.MethodId;
			ModuleId = (Ptr) data.Value.ModuleId;
			MethodStartAddress = (Ptr) data.Value.MethodStartAddress;
			MethodSize = data.Value.MethodSize;
			MethodToken = data.Value.MethodToken;
			Flags = (MethodFlags) (data.Value.Flags & MethodFlagsMask);
			Tier = ((MethodFlags)data.Value.Flags & MethodFlags.Jitted) == MethodFlags.None ? OptimizationTier.Unknown : (OptimizationTier)(((uint)data.Value.Flags >> OptimizationTierShift) & OptimizationTierLowMask);

			var start = Ptr.OfReadOnlyRef(data.Value.TextBlobStart).As<byte>();
			var end = Ptr.OfReadOnlyRef(data.Value.MethodId).As<byte>() + userDataLength;
			var textBlob = start.As<char>().AsSpan((int)(end - start) / sizeof(char));

			MethodNamespace = textBlob.GetNextString();
			MethodName = textBlob.GetNextString();
			MethodSignature = textBlob.GetNextString();
		}
	}
}