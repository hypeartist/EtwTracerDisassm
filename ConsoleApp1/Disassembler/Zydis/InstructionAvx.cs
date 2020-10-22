namespace ConsoleApp1.Disassembler.Zydis
{
	public readonly struct InstructionAvx
	{
		/// <summary>
		/// The `AVX` vector-length.
		/// </summary>
		public readonly ushort VectorLength;

		public readonly InstructionAvxMask Mask;

		public readonly InstructionAvxBroadcast Broadcast;

		public readonly InstructionAvxRounding Rounding;

		public readonly InstructionAvxSwizzle Swizzle;

		public readonly InstructionAvxConversion Conversion;

		/// <summary>
		/// Signals, if the `SAE` (suppress-all-exceptions) functionality is enabled for
		/// the instruction.
		/// </summary>
		public readonly bool HasSae;

		/// <summary>
		/// Signals, if the instruction has a memory-eviction-hint (`KNC` only).
		/// </summary>
		public readonly bool HasEvictionHint;

		// TODO: publish EVEX tuple-type and MVEX functionality
	}
}