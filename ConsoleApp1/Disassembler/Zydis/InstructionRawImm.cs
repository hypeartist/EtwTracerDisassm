namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Detailed info about immediate-bytes.
	/// </summary>
	public readonly struct InstructionRawImm
	{
		/// <summary>
		/// Signals, if the immediate value is signed.
		/// </summary>
		public readonly bool IsSigned;

		/// <summary>
		/// Signals, if the immediate value contains a relative offset. You can use
		/// `ZydisCalcAbsoluteAddress` to determine the absolute address value.
		/// </summary>
		public readonly bool IsRelative;

		public readonly InstructionRawImmValue Value;

		/// <summary>
		/// The physical immediate size, in bits.
		/// </summary>
		public readonly byte Size;

		/// <summary>
		/// The offset of the immediate data, relative to the beginning of the
		/// instruction, in bytes.
		/// </summary>
		public readonly byte Offset;
	}
}