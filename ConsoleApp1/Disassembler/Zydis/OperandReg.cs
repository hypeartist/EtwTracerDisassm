namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Extended info for register-operands.
	/// </summary>
	public readonly struct OperandReg
	{
		/// <summary>
		/// The register value.
		/// </summary>
		public readonly Register Value;

		// TODO: AVX512_4VNNIW MULTISOURCE registers
	}
}