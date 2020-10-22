namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Extended info for immediate-operands.
	/// </summary>
	public readonly struct OperandImm
	{
		/// <summary>
		/// Signals, if the immediate value is signed.
		/// </summary>
		public readonly bool IsSigned;

		/// <summary>
		/// Signals, if the immediate value contains a relative offset. You can use
		/// `ZydisCalcAbsoluteAddress` to determine the absolute address value.
		/// </summary>
		public readonly bool IsRelative;

		/// <summary>
		/// The immediate value.
		/// </summary>
		public readonly OperandImmValue Value;
	}
}