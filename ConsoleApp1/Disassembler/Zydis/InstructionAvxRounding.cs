namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	///   Contains info about the `AVX` rounding.
	/// </summary>
	public readonly struct InstructionAvxRounding
	{
		/// <summary>
		/// The `AVX` rounding-mode.
		/// </summary>
		public readonly RoundingMode Mode;
	}
}