namespace ConsoleApp1.Disassembler.Zydis
{
	public enum FormatterStyle
	{
		/// <summary>
		/// Generates `AT&T`-style disassembly.
		/// </summary>
		Att,

		/// <summary>
		/// Generates `Intel`-style disassembly.
		/// </summary>
		Intel,

		/// <summary>
		/// Generates `MASM`-style disassembly that is directly accepted as input for the
		/// `MASM` assembler.
		/// The runtime-address is ignored in this mode.
		/// </summary>
		IntelMasm,
	}
}