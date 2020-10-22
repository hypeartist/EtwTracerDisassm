namespace ConsoleApp1.Disassembler.Zydis
{
	public enum OperandVisibility
	{
		Invalid,

		/// <summary>
		/// The operand is explicitly encoded in the instruction.
		/// </summary>
		Explicit,

		/// <summary>
		/// The operand is part of the opcode, but listed as an operand.
		/// </summary>
		Implicit,

		/// <summary>
		/// The operand is part of the opcode, and not typically listed as an operand.
		/// </summary>
		Hidden,
	}
}