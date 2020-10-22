namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Contains info about the `AVX` broadcast.
	/// </summary>
	public readonly struct InstructionAvxBroadcast
	{
		/// <summary>
		/// Signals, if the broadcast is a static broadcast.
		/// This is the case for instructions with inbuilt broadcast functionality, which is
		/// always active and not controlled by the `EVEX/MVEX.RC` bits.
		/// </summary>
		public readonly bool IsStatic;

		/// <summary>
		/// The `AVX` broadcast-mode.
		/// </summary>
		public readonly BroadcastMode Mode;
	}
}