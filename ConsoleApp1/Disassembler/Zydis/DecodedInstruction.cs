using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.Disassembler.Zydis
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct DecodedInstruction
	{
		/// <summary>
		/// The machine mode used to decode this instruction.
		/// </summary>
		public readonly MachineMode MachineMode;

		/// <summary>
		/// The instruction-mnemonic.
		/// </summary>
		public readonly InstructionMnemonic Mnemonic;

		/// <summary>
		/// The length of the decoded instruction.
		/// </summary>
		public readonly byte Length;

		/// <summary>
		/// The instruction-encoding (`LEGACY`, `3DNOW`, `VEX`, `EVEX`, `XOP`).
		/// </summary>
		public readonly InstructionEncoding Encoding;

		/// <summary>
		/// The opcode-map.
		/// </summary>
		public readonly OpcodeMap OpcodeMap;

		/// <summary>
		/// The instruction-opcode.
		/// </summary>
		public readonly byte Opcode;

		/// <summary>
		/// The stack width.
		/// </summary>
		public readonly byte StackWidth;

		/// <summary>
		/// The effective operand width.
		/// </summary>
		public readonly byte OperandWidth;

		/// <summary>
		/// The effective address width.
		/// </summary>
		public readonly AddressWidth AddressWidth;

		/// <summary>
		/// The number of instruction-operands.
		/// </summary>
		public readonly byte OperandCount;

		/// <summary>
		/// Detailed info for all instruction operands.
		/// Explicit operands are guaranteed to be in the front and ordered as they are printed
		/// by the formatter in Intel mode. No assumptions can be made about the order of hidden
		/// operands, except that they always located behind the explicit operands.
		/// </summary>
		public readonly FixedBuf10<DecodedOperand> Operands;

		/// <summary>
		/// Instruction attributes.
		/// </summary>
		public readonly InstructionAttributes Attributes;

		/// <summary>
		/// Information about accessed CPU flags.
		/// </summary>
		public readonly FixedBuf21<AccessedFlags> AccessedFlags;

		/// <summary>
		/// Extended info for `AVX` instructions.
		/// </summary>
		public readonly InstructionAvx Avx;

		/// <summary>
		/// Meta info.
		/// </summary>
		public readonly InstructionMeta Meta;

		/// <summary>
		/// Detailed info about different instruction-parts like `ModRM`, `SIB` or  encoding-prefixes.
		/// </summary>
		public readonly InstructionRaw Raw;
	}
}