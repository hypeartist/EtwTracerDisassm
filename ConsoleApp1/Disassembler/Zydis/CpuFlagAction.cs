namespace ConsoleApp1.Disassembler.Zydis
{
	public enum CpuFlagAction
	{
		/// <summary>
		/// The CPU flag is not touched by the instruction.
		/// </summary>
		None,

		/// <summary>
		/// The CPU flag is tested (read).
		/// </summary>
		Tested,

		/// <summary>
		/// The CPU flag is tested and modified afterwards (read-write).
		/// </summary>
		TestedModified,

		/// <summary>
		/// The CPU flag is modified (write).
		/// </summary>
		Modified,

		/// <summary>
		/// The CPU flag is set to 0 (write).
		/// </summary>
		Set0,

		/// <summary>
		/// The CPU flag is set to 1 (write).
		/// </summary>
		Set1,

		/// <summary>
		/// The CPU flag is undefined (write).
		/// </summary>
		Undefined,
	}
}