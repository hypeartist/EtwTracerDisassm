using System;
using System.Diagnostics.CodeAnalysis;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw.Packets
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "NotAccessedField.Global")]
	public unsafe readonly ref struct AssemblyLoadStartPacket
	{
		public readonly int ProcessId;

		public readonly ReadOnlySpan<char> AssemblyName;
		public readonly ReadOnlySpan<char> AssemblyPath;
		public readonly ReadOnlySpan<char> RequestingAssembly;
		public readonly ReadOnlySpan<char> AssemblyLoadContext;
		public readonly ReadOnlySpan<char> RequestingAssemblyLoadContext;

		public AssemblyLoadStartPacket(int processId, byte* userData, ushort userDataLength)
		{
			ProcessId = processId;

			var textBlob = new ReadOnlySpan<char>(userData + sizeof(ushort), userDataLength / sizeof(char));

			AssemblyName = textBlob.GetNextString();//Utils.GetNextString(ref textBlob);
			AssemblyPath = textBlob.GetNextString();//Utils.GetNextString(ref textBlob);
			RequestingAssembly = textBlob.GetNextString();//Utils.GetNextString(ref textBlob);
			AssemblyLoadContext = textBlob.GetNextString();//Utils.GetNextString(ref textBlob);
			RequestingAssemblyLoadContext = textBlob.GetNextString();//Utils.GetNextString(ref textBlob);
		}
	}
}