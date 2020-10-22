namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Contains info about the `AVX` data-conversion (`KNC` only).
	/// </summary>
	public readonly struct InstructionAvxConversion
	{
		/// <summary>
		/// The `AVX` data-conversion mode.
		/// </summary>
		public readonly ConversionMode Mode;
	}
}