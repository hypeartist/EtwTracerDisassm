using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw.Packets
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "NotAccessedField.Global")]
	public readonly ref struct MethodJittingStartedPacket
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private readonly struct MethodJittingStartedNativeData
		{
			public readonly ulong MethodId;
			public readonly ulong ModuleId;
			public readonly int MethodToken;
			public readonly int MethodILSize;
			public readonly char TextBlobStart;
		}

		public readonly int ProcessId;
		public readonly ulong MethodId;
		public readonly ulong ModuleId;
		public readonly int MethodToken;
		public readonly int MethodILSize;
		public readonly ReadOnlySpan<char> MethodNamespace;
		public readonly ReadOnlySpan<char> MethodName;
		public readonly ReadOnlySpan<char> MethodSignature;

		internal MethodJittingStartedPacket(int processId, Ptr<byte> userData, ushort userDataLength)
		{
			ProcessId = processId;

			var data = userData.As<MethodJittingStartedNativeData>();
			MethodId = data.Value.MethodId;
			ModuleId = data.Value.ModuleId;
			MethodToken = data.Value.MethodToken;
			MethodILSize = data.Value.MethodILSize;

			var start = Ptr.OfReadOnlyRef(data.Value.TextBlobStart).As<byte>();
			var end = Ptr.OfReadOnlyRef(data.Value.MethodId).As<byte>() + userDataLength;
			var textBlob = start.As<char>().AsSpan((int)(end - start) / sizeof(char));

			MethodNamespace = textBlob.GetNextString();
			MethodName = textBlob.GetNextString();
			MethodSignature = textBlob.GetNextString();
		}
	}

	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "NotAccessedField.Global")]
	public readonly ref struct MethodDetailsPacket
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private readonly struct MethodDetailsNativeData
		{
			public readonly ulong MethodId;
			public readonly ulong TypeId;
			public readonly int MethodToken;
			public readonly int TypeParameterCount;
			public readonly ulong LoaderModuleId;
			// public readonly char TextBlobStart;
		}

		public readonly int ProcessId;
		public readonly ulong MethodId;
		public readonly ulong TypeId;
		public readonly int MethodToken;
		public readonly int TypeParameterCount;
		public readonly ulong LoaderModuleId;
		// public readonly ReadOnlySpan<char> MethodNamespace;
		// public readonly ReadOnlySpan<char> MethodName;
		// public readonly ReadOnlySpan<char> MethodSignature;

		internal MethodDetailsPacket(int processId, Ptr<byte> userData, ushort userDataLength)
		{
			ProcessId = processId;

			var data = userData.As<MethodDetailsNativeData>();
			MethodId = data.Value.MethodId;
			TypeId = data.Value.TypeId;
			MethodToken = data.Value.MethodToken;
			TypeParameterCount = data.Value.TypeParameterCount;
			LoaderModuleId = data.Value.LoaderModuleId;

			// var start = Ptr.OfReadOnlyRef(data.Value.TextBlobStart).As<byte>();
			// var end = Ptr.OfReadOnlyRef(data.Value.MethodId).As<byte>() + userDataLength;
			// var textBlob = start.As<char>().AsSpan((int)(end - start) / sizeof(char));

			// MethodNamespace = textBlob.GetNextString();
			// MethodName = textBlob.GetNextString();
			// MethodSignature = textBlob.GetNextString();
		}
	}
}