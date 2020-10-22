namespace ConsoleApp1.Disassembler.Zydis
{
	public enum BranchType
	{
		/// <summary>
		/// The instruction is not a branch instruction.
		/// </summary>
		None,

		/// <summary>
		/// The instruction is a short (8-bit) branch instruction.
		/// </summary>
		Short,

		/// <summary>
		/// The instruction is a near (16-bit or 32-bit) branch instruction.
		/// </summary>
		Near,

		/// <summary>
		/// The instruction is a far (inter-segment) branch instruction.
		/// </summary>
		Far
	}
}