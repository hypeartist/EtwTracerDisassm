namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Detailed info about the `EVEX` prefix.
	/// </summary>
	public readonly struct InstructionRawEvex
	{
		/// <summary>
		/// Extension of the `ModRM.reg` field (inverted).
		/// </summary>
		public readonly byte R;

		/// <summary>
		/// Extension of the `SIB.index/vidx` field (inverted).
		/// </summary>
		public readonly byte X;

		/// <summary>
		/// Extension of the `ModRM.rm` or `SIB.base` field (inverted).
		/// </summary>
		public readonly byte B;

		/// <summary>
		/// High-16 register specifier modifier (inverted).
		/// </summary>
		public readonly byte R2;

		/// <summary>
		/// Opcode-map specifier.
		/// </summary>
		public readonly byte Mm;

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
		/// Compressed legacy prefix.
		/// </summary>
		public readonly byte Pp;

		/// <summary>
		/// Zeroing/Merging.
		/// </summary>
		public readonly byte Z;

		/// <summary>
		/// Vector-length specifier or rounding-control (most significant bit).
		/// </summary>
		public readonly byte L2;

		/// <summary>
		/// Vector-length specifier or rounding-control (least significant bit).
		/// </summary>
		public readonly byte L;

		/// <summary>
		/// Broadcast/RC/SAE context.
		/// </summary>
		public readonly byte Bc;

		/// <summary>
		/// High-16 `NDS`/`VIDX` register specifier.
		/// </summary>
		public readonly byte V2;

		/// <summary>
		/// Embedded opmask register specifier.
		/// </summary>
		public readonly byte Aaa;

		/// <summary>
		/// The offset of the first evex byte, relative to the beginning of the
		/// instruction, in bytes.
		/// </summary>
		public readonly byte Offset;
	}
}