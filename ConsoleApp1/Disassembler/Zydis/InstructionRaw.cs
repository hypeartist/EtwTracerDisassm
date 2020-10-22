using ConsoleApp1.Common;

namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Detailed info about different instruction-parts like `ModRM`, `SIB` or encoding-prefixes.
	/// </summary>
	public readonly struct InstructionRaw
	{
		/// <summary>
		/// The number of legacy prefixes.
		/// </summary>
		public readonly byte PrefixCount;

		/// <summary>
		/// Detailed info about the legacy prefixes (including `REX`).
		/// </summary>
		public readonly FixedBuf15<InstructionRawPrefixes> Prefixes;

		/// <summary>
		/// Detailed info about the `REX` prefix.
		/// </summary>
		public readonly InstructionRawRex Rex;

		/// <summary>
		/// Detailed info about the `XOP` prefix.
		/// </summary>
		public readonly InstructionRawXop Xop;

		/// <summary>
		/// Detailed info about the `VEX` prefix.
		/// </summary>
		public readonly InstructionRawVex Vex;

		/// <summary>
		/// Detailed info about the `EVEX` prefix.
		/// </summary>
		public readonly InstructionRawEvex Evex;

		/// <summary>
		/// Detailed info about the `MVEX` prefix.
		/// </summary>
		public readonly InstructionRawMvex Mvex;

		/// <summary>
		/// Detailed info about the `ModRM` byte.
		/// </summary>
		public readonly InstructionModRm ModRm;

		/// <summary>
		/// Detailed info about the `SIB` byte.
		/// </summary>
		public readonly InstructionRawSib Sib;

		/// <summary>
		/// Detailed info about displacement-bytes.
		/// </summary>
		public readonly InstructionRawDisp Disp;

		/// <summary>
		/// Detailed info about immediate-bytes.
		/// </summary>
		public readonly FixedBuf2<InstructionRawImm> Imm;
	}
}