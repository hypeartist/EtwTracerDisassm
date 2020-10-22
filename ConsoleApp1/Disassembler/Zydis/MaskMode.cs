namespace ConsoleApp1.Disassembler.Zydis
{
	public enum MaskMode
	{
		Invalid,

		/// <summary>
		/// Masking is disabled for the current instruction (`K0` register is used).
		/// </summary>
		Disabled,

		/// <summary>
		/// The embedded mask register is used as a merge-mask.
		/// </summary>
		Merging,

		/// <summary>
		/// The embedded mask register is used as a zero-mask.
		/// </summary>
		Zeroing,

		/// <summary>
		/// The embedded mask register is used as a control-mask (element selector).
		/// </summary>
		Control,

		/// <summary>
		/// The embedded mask register is used as a zeroing control-mask (element selector).
		/// </summary>
		ControlZeroing,
	}
}