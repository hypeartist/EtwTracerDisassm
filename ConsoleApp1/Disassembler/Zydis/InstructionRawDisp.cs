namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Detailed info about displacement-bytes.
	/// </summary>
	public readonly struct InstructionRawDisp
	{
		/// <summary>
		/// The displacement value
		/// </summary>
		public readonly long Value;

		/// <summary>
		/// The physical displacement size, in bits.
		/// </summary>
		public readonly byte Size;

		// TODO: publish cd8 scale
		/// <summary>
		/// The offset of the displacement data, relative to the beginning of the
		/// instruction, in bytes.
		/// </summary>
		public readonly byte Offset;
	}
}