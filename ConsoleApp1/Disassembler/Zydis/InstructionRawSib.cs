namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Detailed info about the `SIB` byte.
	/// </summary>
	public readonly struct InstructionRawSib
	{
		/// <summary>
		/// The scale factor.
		/// </summary>
		public readonly byte Scale;

		/// <summary>
		/// The index-register specifier.
		/// </summary>
		public readonly byte Index;

		/// <summary>
		/// The base-register specifier.
		/// </summary>
		public readonly byte Base;

		/// <summary>
		/// The offset of the `SIB` byte, relative to the beginning of the instruction,
		/// in bytes.
		/// </summary>
		public readonly byte Offset;
	}
}