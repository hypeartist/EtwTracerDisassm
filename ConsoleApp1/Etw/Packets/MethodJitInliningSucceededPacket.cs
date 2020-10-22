using System;
using System.Diagnostics.CodeAnalysis;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw.Packets
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "NotAccessedField.Global")]
	public unsafe readonly ref struct MethodJitInliningSucceededPacket
	{
		public readonly int ProcessId;

		public readonly ReadOnlySpan<char> MethodBeingCompiledNamespace;
		public readonly ReadOnlySpan<char> MethodBeingCompiledName;
		public readonly ReadOnlySpan<char> MethodBeingCompiledNameSignature;
		public readonly ReadOnlySpan<char> InlinerNamespace;
		public readonly ReadOnlySpan<char> InlinerName;
		public readonly ReadOnlySpan<char> InlinerNameSignature;
		public readonly ReadOnlySpan<char> InlineeNamespace;
		public readonly ReadOnlySpan<char> InlineeName;
		public readonly ReadOnlySpan<char> InlineeNameSignature;

		public MethodJitInliningSucceededPacket(int processId, Ptr<byte> userData, ushort userDataLength)
		{
			ProcessId = processId;

			var textBlob = new ReadOnlySpan<char>(userData.AsIntPtr().ToPointer(), userDataLength / sizeof(char));

			MethodBeingCompiledNamespace = textBlob.GetNextString();
			MethodBeingCompiledName = textBlob.GetNextString();
			MethodBeingCompiledNameSignature = textBlob.GetNextString();
			InlinerNamespace = textBlob.GetNextString();
			InlinerName = textBlob.GetNextString();
			InlinerNameSignature = textBlob.GetNextString();
			InlineeNamespace = textBlob.GetNextString();
			InlineeName = textBlob.GetNextString();
			InlineeNameSignature = textBlob.GetNextString();
		}
	}
}