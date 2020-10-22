using System;
using System.Runtime.InteropServices;
using System.Security;
using ConsoleApp1.Common;

namespace ConsoleApp1.Disassembler.Zydis
{
	public static class Zydis
	{
		[DllImport(@"d:\Repos\zydis\msvc\bin\ReleaseX64\Zydis.dll", EntryPoint = "ZydisDecoderInit")]
		[SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		public static extern uint DecoderInit(ref Decoder pDecoder, MachineMode mode, AddressWidth addressWidth);

		[DllImport(@"d:\Repos\zydis\msvc\bin\ReleaseX64\Zydis.dll", EntryPoint = "ZydisDecoderDecodeBuffer")]
		[SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		public static extern uint DecoderDecodeBuffer(ref Decoder pDecoder, Ptr buffer, ulong befferLength, ref DecodedInstruction ins);

		[DllImport(@"d:\Repos\zydis\msvc\bin\ReleaseX64\Zydis.dll", EntryPoint = "ZydisFormatterInit")]
		[SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		public static extern uint FormatterInit(ref Formatter formatter, FormatterStyle style);

		[DllImport(@"d:\Repos\zydis\msvc\bin\ReleaseX64\Zydis.dll", EntryPoint = "ZydisFormatterSetProperty")]
		[SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		public static extern uint FormatterSetProperty(ref Formatter formatter, FormatterProperty property, ulong value);

		[DllImport(@"d:\Repos\zydis\msvc\bin\ReleaseX64\Zydis.dll", EntryPoint = "ZydisFormatterFormatInstruction")]
		[SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		public static extern uint FormatterFormatInstruction(ref Formatter formatter, ref DecodedInstruction instruction, Ptr buffer, uint length, Ptr runtimeAddress);
		
		[DllImport(@"d:\Repos\zydis\msvc\bin\ReleaseX64\Zydis.dll", EntryPoint = "ZydisFormatterFormatOperand")]
		[SuppressUnmanagedCodeSecurity]
		[SuppressGCTransition]
		public static extern uint FormatOperand(ref Formatter formatter, ref DecodedInstruction instruction, byte index, Ptr buffer, uint length, Ptr runtimeAddress);

		public static bool Success(uint v) => (v & 0x80000000) == 0;
	}
}