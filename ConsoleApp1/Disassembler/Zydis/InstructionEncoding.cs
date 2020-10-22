namespace ConsoleApp1.Disassembler.Zydis
{
	public enum InstructionEncoding
	{
		/// <summary>
		/// The instruction uses the legacy encoding.
		/// </summary>
		Legacy,

		/// <summary>
		/// The instruction uses the AMD 3DNow-encoding.
		/// </summary>
		Amd3Dnow,

		/// <summary>
		/// The instruction uses the AMD XOP-encoding.
		/// </summary>
		Xop,

		/// <summary>
		/// The instruction uses the VEX-encoding.
		/// </summary>
		Vex,

		/// <summary>
		/// The instruction uses the EVEX-encoding.
		/// </summary>
		Evex,

		/// <summary>
		/// The instruction uses the MVEX-encoding.
		/// </summary>
		Mvex,
	}
}