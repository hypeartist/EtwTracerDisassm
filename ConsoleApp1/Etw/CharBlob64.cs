using System;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw
{
	[StructLayout(LayoutKind.Explicit, Size = sizeof(int) + 64 * sizeof(char))]
	internal unsafe readonly struct CharBlob64
	{
		[FieldOffset((64 - sizeof(int)) * sizeof(char))]
		public readonly int Length;

		public CharBlob64(int length) => Length = length;

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