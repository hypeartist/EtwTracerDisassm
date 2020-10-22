namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Extended info for memory-operands with displacement.
	/// </summary>
	public readonly struct OperandMemDisp
	{
		/// <summary>
		/// Signals, if the displacement value is used.
		/// </summary>
		public readonly bool HasDisplacement;

		/// <summary>
		/// The displacement value
		/// </summary>
		public readonly long Value;
	}
}