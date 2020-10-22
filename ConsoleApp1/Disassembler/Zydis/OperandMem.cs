namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Extended info for memory-operands.
	/// </summary>
	public readonly struct OperandMem
	{
		/// <summary>
		/// The type of the memory operand.
		/// </summary>
		public readonly MemoryOperandType Type;

		/// <summary>
		/// The segment register.
		/// </summary>
		public readonly Register Segment;

		/// <summary>
		/// The base register.
		/// </summary>
		public readonly Register Base;

		/// <summary>
		/// The index register.
		/// </summary>
		public readonly Register Index;

		/// <summary>
		/// The scale factor.
		/// </summary>
		public readonly byte Scale;

		/// <summary>
		/// Extended info for memory-operands with displacement.
		/// </summary>
		public readonly OperandMemDisp Disp;
	}
}