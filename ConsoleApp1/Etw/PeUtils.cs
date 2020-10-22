// using System;
// using System.Runtime.InteropServices;
// using ConsoleApp1.ClrTracer.Com;
//
// namespace ConsoleApp1.ClrTracer
// {
// 	internal static unsafe class PeUtils
// 	{
// 		public enum MachineType : ushort
// 		{
// 			I386 = 0x014c,
// 			Amd64 = 0x8664
// 		}
//
// 		[StructLayout(LayoutKind.Explicit, Size = 0x40)]
// 		public readonly struct ImageDosHeader
// 		{
// 			[FieldOffset(0x00)]
// 			public readonly ushort Magic;
// 			[FieldOffset(0x3c)]
// 			public readonly int LfaNew;
// 		}
//
// 		[StructLayout(LayoutKind.Explicit)]
// 		public struct ImageNtHeaders
// 		{
// 			[FieldOffset(0x00)]
// 			public readonly uint Signature;
// 			[FieldOffset(0x04)]
// 			public readonly ImageFileHeader FileHeader;
// 			[FieldOffset(0x18)]
// 			public ImageOptionalHeader OptionalHeader;
// 		}
//
// 		[StructLayout(LayoutKind.Explicit, Size = 0x14)]
// 		public readonly struct ImageFileHeader
// 		{
// 			[FieldOffset(0x00)]
// 			public readonly MachineType Machine;
// 			[FieldOffset(0x02)]
// 			public readonly ushort NumberOfSections;
// 			[FieldOffset(0x10)]
// 			public readonly ushort SizeOfOptionalHeader;
// 			[FieldOffset(0x12)]
// 			public readonly ushort Characteristics;
// 		}
//
// 		[StructLayout(LayoutKind.Explicit)]
// 		public struct ImageOptionalHeader
// 		{
// 			[FieldOffset(0x08)]
// 			public readonly uint SizeOfInitializedData;
// 			[FieldOffset(0x0c)]
// 			public readonly uint SizeOfUninitializedData;
// 			[FieldOffset(0x10)]
// 			public readonly uint AddressOfEntryPoint;
// 			[FieldOffset(0x1c)]
// 			public uint ImageBase32;
// 			[FieldOffset(0x18)]
// 			public ulong ImageBase64;
// 			[FieldOffset(0x20)]
// 			public readonly uint SectionAlignment;
// 			[FieldOffset(0x38)]
// 			public readonly uint SizeOfImage;
// 			[FieldOffset(0x3c)]
// 			public readonly uint SizeOfHeaders;
// 			[FieldOffset(0x60)]
// 			public readonly ImageDataDirectory ExportTable32;
// 			[FieldOffset(0x70)]
// 			public readonly ImageDataDirectory ExportTable64;
// 			[FieldOffset(0x68)]
// 			public readonly ImageDataDirectory ImportTable32;
// 			[FieldOffset(0x78)]
// 			public readonly ImageDataDirectory ImportTable64;
// 			[FieldOffset(0xa0)]
// 			public readonly ImageDataDirectory BaseRelocationTable32;
// 			[FieldOffset(0xb0)]
// 			public readonly ImageDataDirectory BaseRelocationTable64;
// 			[FieldOffset(0xd8)]
// 			public readonly ImageDataDirectory TLSTable32;
// 			[FieldOffset(0xe8)]
// 			public readonly ImageDataDirectory TLSTable64;
// 		}
//
// 		[StructLayout(LayoutKind.Explicit, Size = 0x08)]
// 		public readonly struct ImageDataDirectory
// 		{
// 			[FieldOffset(0x00)]
// 			public readonly uint VirtualAddress;
// 			[FieldOffset(0x04)]
// 			public readonly uint Size;
// 		}
//
// 		private const ushort SignatureDos = 0x5A4D;
// 		private const uint SignatureNt = 0x00004550;
//
// 		public static bool GetHeaders(IntPtr imageBase, ProcessMemoryReader memoryReader, out ImageNtHeaders headers)
// 		{
// 			headers = default;
// 			// var dosHeader = *(ImageDosHeader*)imageBase;
// 			memoryReader.Read<ImageDosHeader>(imageBase, out var dosHeader);
// 			if (dosHeader.Magic != SignatureDos)
// 			{
// 				return false;
// 			}
//
// 			// var ntHeaders = *(ImageNtHeaders*)(imageBase + dosHeader.LfaNew);
// 			memoryReader.Read<ImageNtHeaders>(imageBase + dosHeader.LfaNew, out var ntHeaders);
// 			if (ntHeaders.Signature != SignatureNt)
// 			{
// 				return false;
// 			}
//
// 			headers = ntHeaders;
// 			return true;
// 		}
// 	}
// }