namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Extended info for pointer-operands.
	/// </summary>
	public readonly struct OperandPtr
	{
		public readonly ushort Segment;
		public readonly uint Offset;
	}
}