using System.Runtime.InteropServices;

namespace ConsoleApp1.Disassembler.Zydis
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct DecodedOperand
	{
		/// <summary>
		/// The operand-id.
		/// </summary>
		public readonly byte Id;

		/// <summary>
		/// The type of the operand.
		/// </summary>
		public readonly OperandType Type;

		/// <summary>
		/// The visibility of the operand.
		/// </summary>
		public readonly OperandVisibility Visibility;

		/// <summary>
		/// The operand-actions.
		/// </summary>
		public readonly OperandAction Actions;

		/// <summary>
		/// The operand-encoding.
		/// </summary>
		public readonly OperandEncoding Encoding;

		/// <summary>
		/// The logical size of the operand (in bits).
		/// </summary>
		public readonly ushort Size;

		/// <summary>
		/// The element-type.
		/// </summary>
		public readonly ElementType ElementType;

		/// <summary>
		/// The size of a single element.
		/// </summary>
		public readonly ushort ElementSize;

		/// <summary>
		/// The number of elements.
		/// </summary>
		public readonly ushort ElementCount;

		/// <summary>
		/// Extended info for register-operands.
		/// </summary>
		public readonly OperandReg Reg;

		/// <summary>
		/// Extended info for memory-operands.
		/// </summary>
		public readonly OperandMem Mem;

		/// <summary>
		/// Extended info for pointer-operands.
		/// </summary>
		public readonly OperandPtr Ptr;

		/// <summary>
		/// Extended info for immediate-operands.
		/// </summary>
		public readonly OperandImm Imm;
	}
}