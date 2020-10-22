using System.Runtime.InteropServices;

namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// The immediate value.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public readonly struct OperandImmValue
	{
		[FieldOffset(0)]
		public readonly ulong U64;

		[FieldOffset(0)]
		public readonly long I64;
	}
}