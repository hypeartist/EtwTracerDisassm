namespace ConsoleApp1.Disassembler.Zydis
{
	public readonly struct InstructionMeta
	{
		/// <summary>
		/// The instruction category.
		/// </summary>
		public readonly InstructionCategory Category;

		/// <summary>
		/// The ISA-set.
		/// </summary>
		public readonly IsaSet IsaSet;

		/// <summary>
		/// The ISA-set extension.
		/// </summary>
		public readonly IsaExt IsaExt;

		/// <summary>
		/// The branch type.
		/// </summary>
		public readonly BranchType BranchType;

		/// <summary>
		/// The exception class.
		/// </summary>
		public readonly ExceptionClass ExceptionClass;
	}
}