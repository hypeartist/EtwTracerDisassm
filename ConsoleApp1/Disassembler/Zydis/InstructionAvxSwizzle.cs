namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Contains info about the `AVX` register-swizzle (`KNC` only).
	/// </summary>
	public readonly struct InstructionAvxSwizzle
	{
		/// <summary>
		/// The `AVX` register-swizzle mode.
		/// </summary>
		public readonly SwizzleMode Mode;
	}
}