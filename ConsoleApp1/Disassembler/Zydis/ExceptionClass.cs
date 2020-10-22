﻿namespace ConsoleApp1.Disassembler.Zydis
{
	public enum ExceptionClass
	{
		None,

		// TODO: FP Exceptions
		Sse1,
		Sse2,
		Sse3,
		Sse4,
		Sse5,
		Sse7,
		Avx1,
		Avx2,
		Avx3,
		Avx4,
		Avx5,
		Avx6,
		Avx7,
		Avx8,
		Avx11,
		Avx12,
		E1,
		E1Nf,
		E2,
		E2Nf,
		E3,
		E3Nf,
		E4,
		E4Nf,
		E5,
		E5Nf,
		E6,
		E6Nf,
		E7Nm,
		E7Nm128,
		E9Nf,
		E10,
		E10Nf,
		E11,
		E11Nf,
		E12,
		E12Np,
		K20,
		K21,
	}
}