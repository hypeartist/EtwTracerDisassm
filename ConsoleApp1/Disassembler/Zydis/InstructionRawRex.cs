namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Detailed info about the `REX` prefix.
	/// </summary>
	public readonly struct InstructionRawRex
	{
		/// <summary>
		/// 64-bit operand-size promotion.
		/// </summary>
		public readonly byte W;

		/// <summary>
		/// Extension of the `ModRM.reg` field.
		/// </summary>
		public readonly byte R;

		/// <summary>
		/// Extension of the `SIB.index` field.
		/// </summary>
		public readonly byte X;

		/// <summary>
		/// Extension of the `ModRM.rm`, `SIB.base`, or `opcode.reg` field.
		/// </summary>
		public readonly byte B;

		/// <summary>
		/// The offset of the effective `REX` byte, relative to the beginning of the
		/// instruction, in bytes.
		/// This offset always points to the "effective" `REX` prefix (the one closest to the
		/// instruction opcode), if multiple `REX` prefixes are present.
		/// Note that the `REX` byte can be the first byte of the instruction, which would lead
		/// to an offset of `0`. Please refer to the instruction attributes to check for the
		/// presence of the `REX` prefix.
		/// </summary>
		public readonly byte Offset;
	}
}