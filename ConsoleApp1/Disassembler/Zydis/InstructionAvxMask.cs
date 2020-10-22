namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Info about the embedded writemask-register (`AVX-512` and `KNC` only).
	/// </summary>
	public readonly struct InstructionAvxMask
	{
		/// <summary>
		/// The masking mode.
		/// </summary>
		public readonly MaskMode Mode;

		/// <summary>
		/// The mask register.
		/// </summary>
		public readonly Register Reg;
	}
}