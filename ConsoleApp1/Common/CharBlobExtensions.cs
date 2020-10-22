using System;
using System.Runtime.CompilerServices;
using ConsoleApp1.Etw;

namespace ConsoleApp1.Common
{
	internal unsafe static class CharBlobExtensions
	{
		public static ReadOnlySpan<char> AsReadOnlySpan(this CharBlob64 blob)
		{
			return new ReadOnlySpan<char>(&blob, blob.Length);
		}

		public static ReadOnlySpan<char> AsReadOnlySpan(this CharBlob1024 blob)
		{
			return new ReadOnlySpan<char>(&blob, blob.Length);
		}
		//
		// public static ReadOnlySpan<char> AsReadOnlySpan(this CharBlobFileMax blob)
		// {
		// 	return new ReadOnlySpan<char>(Unsafe.AsPointer(ref blob), blob.Length);
		// }
		
		// public static void AsStackBasedString(this ReadOnlySpan<char> span, Ptr<CharBlob64> blob)
		// {
		// 	Ptr.OfReadOnlyRef(blob.Value.Length).Value = span.Length;
		// 	Unsafe.CopyBlock(blob.AsIntPtr().ToPointer(), span.GetPointer().AsIntPtr().ToPointer(), (uint)(span.Length * sizeof(char)));
		// }
		//
		// public static void AsStackBasedString(this ReadOnlySpan<char> span, ref CharBlobSessionMax blob)
		// {
		// 	var pBlob = (byte*)Unsafe.AsPointer(ref blob);
		// 	*(int*)(pBlob + (NativeConstants.MaxSessionNameSize - sizeof(int)) * sizeof(char)) = span.Length;
		// 	Unsafe.CopyBlock(pBlob, Unsafe.AsPointer(ref Unsafe.AsRef(span[0])), (uint)(span.Length * sizeof(char)));
		// }
		//
		// public static CharBlobSessionMax AsStackBasedString(this ReadOnlySpan<char> span)
		// {
		// 	CharBlobSessionMax blob = default;
		// 	var pBlob = (byte*)&blob;
		// 	*(int*)(pBlob + (NativeConstants.MaxSessionNameSize - sizeof(int)) * sizeof(char)) = span.Length;
		// 	Unsafe.CopyBlock(pBlob, Unsafe.AsPointer(ref Unsafe.AsRef(span[0])), (uint)(span.Length * sizeof(char)));
		// 	return blob;
		// }
	}
}