namespace ConsoleApp1.Disassembler.Zydis
{
	public enum MemoryOperandType
	{
		Invalid,

		/// <summary>
		/// Normal memory operand.
		/// </summary>
		Mem,

		/// <summary>
		/// The memory operand is only used for address-generation. No real memory-access is
		/// caused.
		/// </summary>
		Agen,

		/// <summary>
		/// A memory operand using `SIB` addressing form, where the index register is not used
		/// in address calculation and scale is ignored. No real memory-access is caused.
		/// </summary>
		Mib,
	}
}