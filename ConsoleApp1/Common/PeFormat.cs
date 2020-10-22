using System.Runtime.InteropServices;

namespace ConsoleApp1.Common
{
	internal static class PeFormat
	{
		public const ushort SignatureDos = 0x5A4D;
		public const uint SignatureNt = 0x00004550;

		[StructLayout(LayoutKind.Explicit, Size = 0x40)]
		public readonly struct ImageDosHeader
		{
			// [FieldOffset(0x00)]
			// public readonly ushort Magic;
			[FieldOffset(0x3c)]
			public readonly int LfaNew;
		}

		// [StructLayout(LayoutKind.Explicit, Size = 0xf8)]
		// public struct ImageNtHeaders32
		// {
		// 	// [FieldOffset(0x00)]
		// 	// public readonly uint Signature;
		// 	[FieldOffset(0x04)]
		// 	public readonly ImageFileHeader FileHeader;
		// 	[FieldOffset(0x18)]
		// 	public ImageOptionalHeader32 OptionalHeader;
		// }

		// [StructLayout(LayoutKind.Explicit, Size = 0x108)]
		// public struct ImageNtHeaders64
		// {
		// 	[FieldOffset(0x00)]
		// 	public readonly uint Signature;
		// 	[FieldOffset(0x04)]
		// 	public readonly ImageFileHeader FileHeader;
		// 	[FieldOffset(0x18)]
		// 	public ImageOptionalHeader64 OptionalHeader;
		// }

		[StructLayout(LayoutKind.Explicit, Size = 0x14)]
		public readonly struct ImageFileHeader
		{
			[FieldOffset(0x00)]
			public readonly ushort Machine;
			[FieldOffset(0x02)]
			public readonly ushort NumberOfSections;
			[FieldOffset(0x10)]
			public readonly ushort SizeOfOptionalHeader;
			// [FieldOffset(0x12)]
			// public readonly ushort Characteristics;
		}

		[StructLayout(LayoutKind.Explicit, Size = 0xe0)]
		public struct ImageOptionalHeader32
		{
			[FieldOffset(0x60)]
			public ImageDataDirectory DataDirectory;//[16]; //0x70
		}

		[StructLayout(LayoutKind.Explicit, Size = 0xf0)]
		public struct ImageOptionalHeader64
		{
			[FieldOffset(0x70)]
			public ImageDataDirectory DataDirectory;//[16]; //0x70
		}

		[StructLayout(LayoutKind.Explicit, Size = 0x28)]
		public readonly struct ImageSectionHeader
		{
			// [FieldOffset(0x08)]
			// public uint PhysicalAddress;
			[FieldOffset(0x0c)]
			public readonly uint VirtualAddress;
			[FieldOffset(0x10)]
			public readonly uint SizeOfRawData;
			[FieldOffset(0x14)]
			public readonly uint PointerToRawData;
			// [FieldOffset(0x24)]
			// public readonly uint Characteristics;
		}

		public readonly struct ImageCor20Header
		{
			public readonly int Cb;
			public readonly ushort MajorRuntimeVersion;
			public readonly ushort MinorRuntimeVersion;
			public readonly ImageDataDirectory MetadataDirectory;
		}

		[StructLayout(LayoutKind.Explicit, Size = 0x08)]
		public readonly struct ImageDataDirectory
		{
			[FieldOffset(0x00)]
			public readonly uint VirtualAddress;
			[FieldOffset(0x04)]
			public readonly uint Size;
		}
	}
}