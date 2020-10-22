namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Detailed info about the `XOP` prefix.
	/// </summary>
	public readonly struct InstructionRawXop
	{
		/// <summary>
		/// Extension of the `ModRM.reg` field (inverted).
		/// </summary>
		public readonly byte R;

		/// <summary>
		/// Extension of the `SIB.index` field (inverted).
		/// </summary>
		public readonly byte X;

		/// <summary>
		/// Extension of the `ModRM.rm`, `SIB.base`, or `opcode.reg` field (inverted).
		/// </summary>
		public readonly byte B;

		/// <summary>
		/// Opcode-map specifier.
		/// </summary>
		public readonly byte Mmmm;

		/// <summary>
		/// 64-bit operand-size promotion or opcode-extension.
		/// </summary>
		public readonly byte W;

		/// <summary>
		/// `NDS`/`NDD` (non-destructive-source/destination) register specifier
		/// (inverted).
		/// </summary>
		public readonly byte Vvvv;

		/// <summary>
		/// Vector-length specifier.
		/// </summary>
		public readonly byte L;

		/// <summary>
		/// Compressed legacy prefix.
		/// </summary>
		public readonly byte Pp;

		/// <summary>
		/// The offset of the first xop byte, relative to the beginning of the
		/// instruction, in bytes.
		/// </summary>
		public readonly byte Offset;
	}
}