namespace ConsoleApp1.Disassembler.Zydis
{
	public enum RoundingMode
	{
		Invalid,

		/// <summary>
/// Round to nearest.
         /// </summary>
		Rn,

		/// <summary>
/// Round down.
         /// </summary>
		Rd,

		/// <summary>
/// Round up.
         /// </summary>
		Ru,

		/// <summary>
/// Round towards zero.
         /// </summary>
		Rz,
	}
}