using System;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;
using ConsoleApp1.Etw.Platform.Win32;

namespace ConsoleApp1.Etw
{
	[StructLayout(LayoutKind.Explicit, Size = /*sizeof(int) + */NativeConstants.MaxSessionNameSize * sizeof(char))]
	internal readonly struct CharBlob1024
	{
		[FieldOffset((NativeConstants.MaxSessionNameSize - sizeof(int)) * sizeof(char))]
		public readonly int Length;

		public CharBlob1024(int length) => Length = length;

		public void Clear() => Ptr.OfReadOnlyRef(this).As<char>().Clear(Length);

		public void CopyFrom(in ReadOnlySpan<char> span)
		{
			Ptr.OfReadOnlyRef(Length).Value = span.Length;
			span.CopyTo(Ptr.OfReadOnlyRef(this).As<char>());
		}

#if DEBUG
		public override string ToString() => new string(this.AsReadOnlySpan());
#endif
	}
}