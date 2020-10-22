namespace ConsoleApp1.Disassembler.Zydis
{
	public enum ElementType
	{
		Invalid,

		/// <summary>
		/// A struct type.
		/// </summary>
		Struct,

		/// <summary>
		/// Unsigned integer value.
		/// </summary>
		Uint,

		/// <summary>
		/// Signed integer value.
		/// </summary>
		Int,

		/// <summary>
		/// 16-bit floating point value (`half`).
		/// </summary>
		Float16,

		/// <summary>
		/// 32-bit floating point value (`single`).
		/// </summary>
		Float32,

		/// <summary>
		/// 64-bit floating point value (`double`).
		/// </summary>
		Float64,

		/// <summary>
		/// 80-bit floating point value (`extended`).
		/// </summary>
		Float80,

		/// <summary>
		/// Binary coded decimal value.
		/// </summary>
		Longbcd,

		/// <summary>
		/// A condition code (e.g. used by `CMPPD`, `VCMPPD`, ...).
		/// </summary>
		Cc,
	}
}