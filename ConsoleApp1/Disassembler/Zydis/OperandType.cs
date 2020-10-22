namespace ConsoleApp1.Disassembler.Zydis
{
	public enum OperandType
	{
		/// <summary>
		/// The operand is not used.
		/// </summary>
		Unused,

		/// <summary>
		/// The operand is a register operand.
		/// </summary>
		Register,

		/// <summary>
		/// The operand is a memory operand.
		/// </summary>
		Memory,

		/// <summary>
		/// The operand is a pointer operand with a segment:offset lvalue.
		/// </summary>
		Pointer,

		/// <summary>
		/// The operand is an immediate operand.
		/// </summary>
		Immediate,
	}
}