namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Detailed info about the `ModRM` byte.
	/// </summary>
	public readonly struct InstructionModRm
	{
		/// <summary>
		/// The addressing mode.
		/// </summary>
		public readonly byte Mod;

		/// <summary>
		/// Register specifier or opcode-extension.
		/// </summary>
		public readonly byte Reg;

		/// <summary>
		/// Register specifier or opcode-extension.
		/// </summary>
		public readonly byte Rm;

		/// <summary>
		/// The offset of the `ModRM` byte, relative to the beginning of the
		/// instruction, in bytes.
		/// </summary>
		public readonly byte Offset;
	}
}