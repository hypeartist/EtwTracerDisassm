using System.Runtime.InteropServices;
using ConsoleApp1.Common;

// ReSharper disable UnusedMember.Global

namespace ConsoleApp1.ClrTracer.Com
{
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3E11CCEE-D08B-43e5-AF01-32717A64DA03")]
	internal interface IDacDataTarget
	{
		void GetMachineType(out uint machineType);
		void GetPointerSize(out uint pointerSize);
		void GetImageBase(Ptr imagePath, out Ptr baseAddress);

		[PreserveSig]
		int ReadVirtual(Ptr address, Ptr buffer, int bytesRequested, out int bytesRead);

		void WriteVirtual(Ptr address, Ptr buffer, uint bytesRequested, out uint bytesWritten);
		void GetTLSValue(uint threadId, uint index, out ulong value);
		void SetTLSValue(uint threadId, uint index, ulong value);
		void GetCurrentThreadID(out uint threadId);
		void GetThreadContext(uint threadId, uint contextFlags, uint contextSize, Ptr context);
		void SetThreadContext(uint threadId, uint contextSize, Ptr context);
		void Request(uint reqCode, uint inBufferSize, Ptr inBuffer, Ptr outBufferSize, out Ptr outBuffer);
	}
}