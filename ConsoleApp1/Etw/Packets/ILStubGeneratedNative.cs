using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw.Packets
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "NotAccessedField.Global")]
	public unsafe readonly ref struct ILStubGeneratedPacket
	{
		[Flags]
		public enum ILStubGeneratedFlags 
		{
			None = 0,
			ReverseInterop = 0x1,
			ComInterop = 0x2,
			NGenedStub = 0x4,
			Delegate = 0x8,
			VarArg = 0x10,
			UnmanagedCallee = 0x20,
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private readonly struct ILStubGeneratedNativeData
		{
			public readonly ushort ClrInstanceID;
			public readonly ulong ModuleId;
			public readonly ulong StubMethodId;
			public readonly ILStubGeneratedFlags StubFlags;
			public readonly int ManagedInteropMethodToken;
			public readonly char TextBlobStart;
		}

		public readonly int ProcessId;
		public readonly ulong ModuleId;
		public readonly ulong StubMethodId;
		public readonly ILStubGeneratedFlags StubFlags;
		public readonly int ManagedInteropMethodToken;
		public readonly ReadOnlySpan<char> ManagedInteropMethodNamespace;
		public readonly ReadOnlySpan<char> ManagedInteropMethodName;
		public readonly ReadOnlySpan<char> ManagedInteropMethodSignature;
		public readonly ReadOnlySpan<char> NativeMethodSignature;
		public readonly ReadOnlySpan<char> StubMethodSignature;
		public readonly ReadOnlySpan<char> StubMethodILCode;

		internal ILStubGeneratedPacket(int processId, Ptr<byte> userData, ushort userDataLength)
		{
			ProcessId = processId;

			var data = userData.As<ILStubGeneratedNativeData>();
			ModuleId = data.Value.ModuleId;
			StubMethodId = data.Value.StubMethodId;
			StubFlags = data.Value.StubFlags;
			ManagedInteropMethodToken = data.Value.ManagedInteropMethodToken;

			var start = Ptr.OfReadOnlyRef(data.Value.TextBlobStart).As<byte>();
			var end = Ptr.OfReadOnlyRef(data.Value.ClrInstanceID).As<byte>() + userDataLength;
			var textBlob = start.As<char>().AsSpan((int)(end - start) / sizeof(char));

			ManagedInteropMethodNamespace = textBlob.GetNextString();
			ManagedInteropMethodName = textBlob.GetNextString();
			ManagedInteropMethodSignature = textBlob.GetNextString();
			NativeMethodSignature = textBlob.GetNextString();
			StubMethodSignature = textBlob.GetNextString();
			StubMethodILCode = textBlob.GetNextString();
		}
	}
}