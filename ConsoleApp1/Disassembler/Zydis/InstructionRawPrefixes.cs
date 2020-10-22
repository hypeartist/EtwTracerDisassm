using System.Runtime.InteropServices;

namespace ConsoleApp1.Disassembler.Zydis
{
	/// <summary>
	/// Detailed info about the legacy prefixes (including `REX`).
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct InstructionRawPrefixes
	{
		/// <summary>
		/// The prefix type.
		/// </summary>
		public readonly PrefixType Type;

		/// <summary>
		/// The prefix byte.
		/// </summary>
		public readonly byte Value;
	}
}