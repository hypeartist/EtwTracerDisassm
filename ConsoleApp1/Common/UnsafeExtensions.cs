using System;
using System.Runtime.CompilerServices;

namespace ConsoleApp1.Common
{
	public static unsafe class UnsafeExtensions
	{
		public static bool TryFormat(this Guid guid, Ptr<char> destination, int length, ReadOnlySpan<char> format = default)
		{
			return guid.TryFormat(new Span<char>(destination.AsIntPtr().ToPointer(), length), out _, format);
		}

		public static void CopyTo(this ReadOnlySpan<char> span, Ptr<char> p)
		{
			if(span.Length == 0) return;
			Ptr.OfReadOnlyRef(span).CopyTo(p, span.Length);
		}

		public static ReadOnlySpan<char> GetNextString(this ref ReadOnlySpan<char> span)
		{
			var pos = span.IndexOf('\x0');
			if (pos > -1)
			{
				var part = span.Slice(0, pos);
				span = span.Slice(pos + 1);
				return part;
			}
			else
			{
				var part = span;
				span = span.Slice(span.Length);
				return part;
			}
		}

		public static ReadOnlySpan<char> GetString(this ReadOnlySpan<char> span)
		{
			var pos = span.IndexOf('\x0');
			if (pos > -1)
			{
				var part = span.Slice(0, pos);
				return part;
			}
			else
			{
				var part = span;
				return part;
			}
		}

		public static int GetStringLength(this in ReadOnlySpan<char> span)
		{
			var pos = span.IndexOf('\x0');
			return pos < 0 ? span.Length : pos;
		}
	}
}