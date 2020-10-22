using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw.Packets
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "NotAccessedField.Global")]
	public unsafe readonly ref struct AppDomainLoadPacket
	{
		[Flags]
		public enum AppDomainFlags
		{
			None = 0,
			Default = 0x1,
			Executable = 0x2,
			Shared = 0x4,
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private readonly struct AppDomainLoadNativeData
		{
			public readonly long AppDomainId;
			public readonly int Flags;
			public readonly char TextBlobStart;
		}

		public readonly int ProcessId;
		public readonly long AppDomainId;
		public readonly AppDomainFlags Flags;
		public readonly ReadOnlySpan<char> AppDomainName;

		internal AppDomainLoadPacket(int processId, Ptr<byte> userData, ushort userDataLength)
		{
			ProcessId = processId;

			var data = userData.As<AppDomainLoadNativeData>();
			AppDomainId = data.Value.AppDomainId;
			Flags = (AppDomainFlags) data.Value.Flags;

			var start = Ptr.OfReadOnlyRef(data.Value.TextBlobStart).As<byte>();
			var end = Ptr.OfReadOnlyRef(data.Value.AppDomainId).As<byte>() + userDataLength;
			var textBlob = new ReadOnlySpan<char>(start.AsIntPtr().ToPointer(), (int)(end - start) / sizeof(char));

			AppDomainName = textBlob.GetNextString();
		}
	}
}