namespace ConsoleApp1.Disassembler.Zydis
{
	public enum PrefixType
	{
		/// <summary>
		/// The prefix is ignored by the instruction.
		/// This applies to all prefixes that are not accepted by the instruction in general or the
		/// ones that are overwritten by a prefix of the same group closer to the instruction opcode.
		/// </summary>
		Ignored,

		/// <summary>
		/// The prefix is effectively used by the instruction.
		/// </summary>
		Effective,

		/// <summary>
		/// The prefix is used as a mandatory prefix.
		/// A mandatory prefix is interpreted as an opcode extension and has no further effect on the
		/// instruction.
		/// </summary>
		Mandatory,
	}
}