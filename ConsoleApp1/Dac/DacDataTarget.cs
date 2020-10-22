using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.ClrTracer.Com
{
	internal sealed class DacDataTarget : IDacDataTarget, IMetadataLocator
	{
		private const int Ok = 0;
		private const int Fail = unchecked((int) 0x80004005);
		private const int InvalidArg = unchecked((int) 0x80070057);

		private readonly ProcessWrapper _process;
		private readonly Ptr _coreClrImageBase;
		private readonly ushort _coreClrImageMachineType;

		public DacDataTarget(Ptr coreClrImageBase, ushort coreClrImageMachineType, ProcessWrapper process)
		{
			_process = process;
			_coreClrImageMachineType = coreClrImageMachineType;
			_coreClrImageBase = coreClrImageBase;
		}

		public void GetMachineType(out uint machineType) => machineType = _coreClrImageMachineType;

		public void GetPointerSize(out uint pointerSize) => pointerSize = (uint) IntPtr.Size;

		public void GetImageBase(Ptr imagePath, out Ptr baseAddress) => baseAddress = _coreClrImageBase;

		public int ReadVirtual(Ptr address, Ptr buffer, int bytesRequested, out int bytesRead)
		{
			if (!_process.Read(address, buffer.As<byte>(), bytesRequested, out var read))
			{
				bytesRead = 0;
				return Fail;
			}

			bytesRead = read;
			return Ok;
		}

		public void WriteVirtual(Ptr address, Ptr buffer, uint bytesRequested, out uint bytesWritten) => throw new NotImplementedException();

		public void GetTLSValue(uint threadId, uint index, out ulong value) => throw new NotImplementedException();

		public void SetTLSValue(uint threadId, uint index, ulong value) => throw new NotImplementedException();

		public void GetCurrentThreadID(out uint threadId) => throw new NotImplementedException();

		public void GetThreadContext(uint threadId, uint contextFlags, uint contextSize, Ptr context) => throw new NotImplementedException();

		public void SetThreadContext(uint threadId, uint contextSize, Ptr context) => throw new NotImplementedException();

		public void Request(uint reqCode, uint inBufferSize, Ptr inBuffer, Ptr outBufferSize, out Ptr outBuffer) => throw new NotImplementedException();

		public unsafe int GetMetadata(string fileName, uint imageTimestamp, uint imageSize, Ptr mvid, uint mdRva, uint flags, uint bufferSize, Ptr buffer, Ptr dataSize)
		{
			if (buffer.IsNull) return InvalidArg;

			Debug.WriteLine(fileName);
			
			var fs= File.OpenRead(fileName);

			const int imageDirectoryEntryCOMDescriptor = 14;
  
            PeFormat.ImageDataDirectory clrDataDirectory;
            PeFormat.ImageDosHeader dosHeader = default;
			fs.Read(Ptr.OfRef(ref dosHeader).As<byte>().AsWritableSpan(Unsafe.SizeOf<PeFormat.ImageDosHeader>()));
			PeFormat.ImageFileHeader fileHeader = default;
			fs.Seek(dosHeader.LfaNew + sizeof(int), SeekOrigin.Begin);
			fs.Read(Ptr.OfRef(ref fileHeader).As<byte>().AsWritableSpan(Unsafe.SizeOf<PeFormat.ImageFileHeader>()));
			if (fileHeader.SizeOfOptionalHeader == Unsafe.SizeOf<PeFormat.ImageOptionalHeader64>())
			{
				PeFormat.ImageOptionalHeader64 optionalHeader = default;
				fs.Read(Ptr.OfRef(ref optionalHeader).As<byte>().AsWritableSpan(Unsafe.SizeOf<PeFormat.ImageOptionalHeader64>()));
				clrDataDirectory = Ptr.OfReadOnlyRef(optionalHeader.DataDirectory)[imageDirectoryEntryCOMDescriptor];
			}
			else if(fileHeader.SizeOfOptionalHeader == Unsafe.SizeOf<PeFormat.ImageOptionalHeader32>())
			{
				PeFormat.ImageOptionalHeader32 optionalHeader = default;
				fs.Read(Ptr.OfRef(ref optionalHeader).As<byte>().AsWritableSpan(Unsafe.SizeOf<PeFormat.ImageOptionalHeader32>()));
				clrDataDirectory = Ptr.OfReadOnlyRef(optionalHeader.DataDirectory)[imageDirectoryEntryCOMDescriptor];
			}
			else
			{
				fs.Dispose();
				return Fail;
			}
  
			var isManaged = clrDataDirectory.VirtualAddress != 0;
			if (!isManaged)
			{
				fs.Dispose();
				return Fail;
			}
  
			var sections = Ptr.OfRef(stackalloc PeFormat.ImageSectionHeader[fileHeader.NumberOfSections]);
			fs.Read(sections.As<byte>().AsWritableSpan(fileHeader.NumberOfSections * Unsafe.SizeOf<PeFormat.ImageSectionHeader>()));
  
			PeFormat.ImageSectionHeader clrSectionHeader = default;
			for (var i = 0; i < fileHeader.NumberOfSections; i++)
			{
				var sectionHeader = sections[i];
				if (sectionHeader.VirtualAddress > clrDataDirectory.VirtualAddress || sectionHeader.VirtualAddress + sectionHeader.SizeOfRawData <= clrDataDirectory.VirtualAddress) continue;
				clrSectionHeader = sectionHeader;
				break;
			}
  
			fs.Seek(clrSectionHeader.PointerToRawData + (clrDataDirectory.VirtualAddress - clrSectionHeader.VirtualAddress), SeekOrigin.Begin);
			PeFormat.ImageCor20Header cor20Header = default;
			fs.Read(Ptr.OfRef(ref cor20Header).As<byte>().AsWritableSpan(Unsafe.SizeOf<PeFormat.ImageCor20Header>()));
			var rva = mdRva;
			var size = bufferSize;
			if (rva == 0)
			{
				var metadata = cor20Header.MetadataDirectory;
				if (metadata.VirtualAddress == 0) return Fail;
  
				rva = metadata.VirtualAddress;
				size = Math.Min(size, metadata.Size);
			}
			for (var i = 0; i < fileHeader.NumberOfSections; i++)
			{
				var sectionHeader = sections[i];
				if (sectionHeader.VirtualAddress <= rva && sectionHeader.VirtualAddress + sectionHeader.SizeOfRawData > rva)
				{
					var off = (int)(sectionHeader.PointerToRawData + (rva - sectionHeader.VirtualAddress));
					fs.Seek(off, SeekOrigin.Begin);
					var read = fs.Read(buffer.As<byte>().AsWritableSpan((int) size));
					if (!dataSize.IsNull) dataSize.As<int>().Value = read;
					break;
				}
			}
			fs.Dispose();
			return Ok;
		}
	}
}