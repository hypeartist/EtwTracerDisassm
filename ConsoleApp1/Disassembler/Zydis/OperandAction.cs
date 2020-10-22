namespace ConsoleApp1.Disassembler.Zydis
{
	public enum OperandAction
	{
		/// <summary>
		/// The operand is read by the instruction.
		/// </summary>
		Read = 0x01,

		/// <summary>
		/// The operand is written by the instruction (must write).
		/// </summary>
		Write = 0x02,

		/// <summary>
		/// The operand is conditionally read by the instruction.
		/// </summary>
		Condread = 0x04,

		/// <summary>
		/// The operand is conditionally written by the instruction (may write).
		/// </summary>
		Condwrite = 0x08,

		/// <summary>
		/// The operand is read (must read) and written by the instruction (must write).
		/// </summary>
		Readwrite = Read | Write,

		/// <summary>
		/// The operand is conditionally read (may read) and conditionally written by the
		/// instruction (may write).
		/// </summary>
		CondreadCondwrite = Condread | Condwrite,

		/// <summary>
		/// The operand is read (must read) and conditionally written by the instruction
		/// (may write).
		/// </summary>
		ReadCondwrite = Read | Condwrite,

		/// <summary>
		/// The operand is written (must write) and conditionally read by the instruction
		/// (may read).
		/// </summary>
		CondreadWrite = Condread | Write,

		/// <summary>
		/// Mask combining all reading access flags.
		/// </summary>
		MaskRead = Read | Condread,

		/// <summary>
		/// Mask combining all writing access flags.
		/// </summary>
		MaskWrite = Write | Condwrite,
	}
}