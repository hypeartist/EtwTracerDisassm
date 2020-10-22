namespace ConsoleApp1.Disassembler.Zydis
{
	public enum FormatterProperty
	{
		/// <summary>
		/// Controls the printing of effective operand-size suffixes (`ATT`) or operand-sizes
		/// of memory operands (`INTEL`).
		/// Pass `ZYAN_TRUE` as value to force the formatter to always print the size, or `ZYAN_FALSE`
		/// to only print it if needed.
		/// </summary>
		ForceSize,

		/// <summary>
		/// Controls the printing of segment prefixes.
		/// Pass `ZYAN_TRUE` as value to force the formatter to always print the segment register of
		/// memory-operands or `ZYAN_FALSE` to omit implicit `DS`/`SS` segments.
		/// </summary>
		ForceSegment,

		/// <summary>
		/// Controls the printing of branch addresses.
		/// Pass `ZYAN_TRUE` as value to force the formatter to always print relative branch addresses
		/// or `ZYAN_FALSE` to use absolute addresses, if a runtime-address different to
		/// `ZYDIS_RUNTIME_ADDRESS_NONE` was passed.
		/// </summary>
		ForceRelativeBranches,

		/// <summary>
		/// Controls the printing of `EIP`/`RIP`-relative addresses.
		/// Pass `ZYAN_TRUE` as value to force the formatter to always print relative addresses for
		/// `EIP`/`RIP`-relative operands or `ZYAN_FALSE` to use absolute addresses, if a runtime-
		/// address different to `ZYDIS_RUNTIME_ADDRESS_NONE` was passed.
		/// </summary>
		ForceRelativeRiprel,

		/// <summary>
		/// Controls the printing of branch-instructions sizes.
		/// Pass `ZYAN_TRUE` as value to print the size (`short`, `near`) of branch
		/// instructions or `ZYAN_FALSE` to hide it.
		/// Note that the `far`/`l` modifier is always printed.
		/// </summary>
		PrintBranchSize,

		/// <summary>
		/// Controls the printing of instruction prefixes.
		/// Pass `ZYAN_TRUE` as value to print all instruction-prefixes (even ignored or duplicate
		/// ones) or `ZYAN_FALSE` to only print prefixes that are effectively used by the instruction.
		/// </summary>
		DetailedPrefixes,

		/// <summary>
		/// Controls the base of address values.
		/// </summary>
		AddrBase,

		/// <summary>
		/// Controls the signedness of relative addresses. Absolute addresses are always
		/// unsigned.
		/// </summary>
		AddrSignedness,

		/// <summary>
		/// Controls the padding of absolute address values.
		/// Pass `ZYDIS_PADDING_DISABLED` to disable padding, `ZYDIS_PADDING_AUTO` to padd all
		/// addresses to the current stack width (hexadecimal only), or any other integer value for
		/// custom padding.
		/// </summary>
		AddrPaddingAbsolute,

		/// <summary>
		/// Controls the padding of relative address values.
		/// Pass `ZYDIS_PADDING_DISABLED` to disable padding, `ZYDIS_PADDING_AUTO` to padd all
		/// addresses to the current stack width (hexadecimal only), or any other integer value for
		/// custom padding.
		/// </summary>
		AddrPaddingRelative,

		/// <summary>
		/// Controls the base of displacement values.
		/// </summary>
		DispBase,

		/// <summary>
		/// Controls the signedness of displacement values.
		/// </summary>
		DispSignedness,

		/// <summary>
		/// Controls the padding of displacement values.
		/// Pass `ZYDIS_PADDING_DISABLED` to disable padding, or any other integer value for custom
		/// padding.
		/// </summary>
		DispPadding,

		/// <summary>
		/// Controls the base of immediate values.
		/// </summary>
		ImmBase,

		/// <summary>
		/// Controls the signedness of immediate values.
		/// Pass `ZYDIS_SIGNEDNESS_AUTO` to automatically choose the most suitable mode based on the
		/// operands `DecodedOperand.imm.is_signed` attribute.
		/// </summary>
		ImmSignedness,

		/// <summary>
		/// Controls the padding of immediate values.
		/// Pass `ZYDIS_PADDING_DISABLED` to disable padding, `ZYDIS_PADDING_AUTO` to padd all
		/// immediates to the operand-width (hexadecimal only), or any other integer value for custom
		/// padding.
		/// </summary>
		ImmPadding,

		/// <summary>
		/// Controls the letter-case for prefixes.
		/// Pass `ZYAN_TRUE` as value to format in uppercase or `ZYAN_FALSE` to format in lowercase.
		/// </summary>
		UppercasePrefixes,

		/// <summary>
		/// Controls the letter-case for the mnemonic.
		/// Pass `ZYAN_TRUE` as value to format in uppercase or `ZYAN_FALSE` to format in lowercase.
		/// </summary>
		UppercaseMnemonic,

		/// <summary>
		/// Controls the letter-case for registers.
		/// Pass `ZYAN_TRUE` as value to format in uppercase or `ZYAN_FALSE` to format in lowercase.
		/// </summary>
		UppercaseRegisters,

		/// <summary>
		/// Controls the letter-case for typecasts.
		/// Pass `ZYAN_TRUE` as value to format in uppercase or `ZYAN_FALSE` to format in lowercase.
		/// </summary>
		UppercaseTypecasts,

		/// <summary>
		/// Controls the letter-case for decorators.
		/// Pass `ZYAN_TRUE` as value to format in uppercase or `ZYAN_FALSE` to format in lowercase.
		/// </summary>
		UppercaseDecorators,

		/// <summary>
		/// Controls the prefix for decimal values.
		/// Pass a pointer to a null-terminated C-style string with a maximum length of 10 characters
		/// to set a custom prefix, or `ZYAN_NULL` to disable it.
		/// The string is deep-copied into an internal buffer.
		/// </summary>
		DecPrefix,

		/// <summary>
		/// Controls the suffix for decimal values.
		/// Pass a pointer to a null-terminated C-style string with a maximum length of 10 characters
		/// to set a custom suffix, or `ZYAN_NULL` to disable it.
		/// The string is deep-copied into an internal buffer.
		/// </summary>
		DecSuffix,

		/// <summary>
		/// Controls the letter-case of hexadecimal values.
		/// Pass `ZYAN_TRUE` as value to format in uppercase and `ZYAN_FALSE` to format in lowercase.
		/// The default value is `ZYAN_TRUE`.
		/// </summary>
		HexUppercase,

		/// <summary>
		/// Controls the prefix for hexadecimal values.
		/// Pass a pointer to a null-terminated C-style string with a maximum length of 10 characters
		/// to set a custom prefix, or `ZYAN_NULL` to disable it.
		/// The string is deep-copied into an internal buffer.
		/// </summary>
		HexPrefix,

		/// <summary>
		/// Controls the suffix for hexadecimal values.
		/// Pass a pointer to a null-terminated C-style string with a maximum length of 10 characters
		/// to set a custom suffix, or `ZYAN_NULL` to disable it.
		/// The string is deep-copied into an internal buffer.
		/// </summary>
		HexSuffix,
	}
}