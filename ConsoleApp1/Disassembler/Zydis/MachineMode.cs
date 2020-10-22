namespace ConsoleApp1.Disassembler.Zydis
{
	public enum MachineMode
	{
		/// <summary>
		/// 64 bit mode.
		/// </summary>
		Long64,

		/// <summary>
		/// 32 bit protected mode.
		/// </summary>
		LongCompat32,

		/// <summary>
		/// 16 bit protected mode.
		/// </summary>
		LongCompat16,

		/// <summary>
		/// 32 bit protected mode.
		/// </summary>
		Legacy32,

		/// <summary>
		/// 16 bit protected mode.
		/// </summary>
		Legacy16,

		/// <summary>
		/// 16 bit real mode.
		/// </summary>
		Real16
	}
}