/*
Copyright (C) 2018-2019 de4dot@gmail.com

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

// ⚠️This file was generated by GENERATOR!🦹‍♂️

#nullable enable

#if INSTR_INFO
namespace Iced.Intel {
	/// <summary><c>CPUID</c> feature flags</summary>
	public enum CpuidFeature {
		/// <summary>8086 or later</summary>
		INTEL8086 = 0,
		/// <summary>8086 only</summary>
		INTEL8086_ONLY = 1,
		/// <summary>80186 or later</summary>
		INTEL186 = 2,
		/// <summary>80286 or later</summary>
		INTEL286 = 3,
		/// <summary>80286 only</summary>
		INTEL286_ONLY = 4,
		/// <summary>80386 or later</summary>
		INTEL386 = 5,
		/// <summary>80386 only</summary>
		INTEL386_ONLY = 6,
		/// <summary>80386 A0-B0 stepping only (<c>XBTS</c>, <c>IBTS</c> instructions)</summary>
		INTEL386_A0_ONLY = 7,
		/// <summary>Intel486 or later</summary>
		INTEL486 = 8,
		/// <summary>Intel486 A stepping only (<c>CMPXCHG</c>)</summary>
		INTEL486_A_ONLY = 9,
		/// <summary>80386 and Intel486 only</summary>
		INTEL386_486_ONLY = 10,
		/// <summary>IA-64</summary>
		IA64 = 11,
		/// <summary>CPUID.80000001H:EDX.LM[bit 29]</summary>
		X64 = 12,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.ADX[bit 19]</summary>
		ADX = 13,
		/// <summary>CPUID.01H:ECX.AES[bit 25]</summary>
		AES = 14,
		/// <summary>CPUID.01H:ECX.AVX[bit 28]</summary>
		AVX = 15,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.AVX2[bit 5]</summary>
		AVX2 = 16,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EDX.AVX512_4FMAPS[bit 3]</summary>
		AVX512_4FMAPS = 17,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EDX.AVX512_4VNNIW[bit 2]</summary>
		AVX512_4VNNIW = 18,
		/// <summary>CPUID.(EAX=07H, ECX=1H):EAX[bit 5]</summary>
		AVX512_BF16 = 19,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.AVX512_BITALG[bit 12]</summary>
		AVX512_BITALG = 20,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.AVX512_IFMA[bit 21]</summary>
		AVX512_IFMA = 21,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.AVX512_VBMI[bit 1]</summary>
		AVX512_VBMI = 22,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.AVX512_VBMI2[bit 6]</summary>
		AVX512_VBMI2 = 23,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.AVX512_VNNI[bit 11]</summary>
		AVX512_VNNI = 24,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EDX[bit 08]</summary>
		AVX512_VP2INTERSECT = 25,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.AVX512_VPOPCNTDQ[bit 14]</summary>
		AVX512_VPOPCNTDQ = 26,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.AVX512BW[bit 30]</summary>
		AVX512BW = 27,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.AVX512CD[bit 28]</summary>
		AVX512CD = 28,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.AVX512DQ[bit 17]</summary>
		AVX512DQ = 29,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.AVX512ER[bit 27]</summary>
		AVX512ER = 30,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.AVX512F[bit 16]</summary>
		AVX512F = 31,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.AVX512PF[bit 26]</summary>
		AVX512PF = 32,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.AVX512VL[bit 31]</summary>
		AVX512VL = 33,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.BMI1[bit 3]</summary>
		BMI1 = 34,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.BMI2[bit 8]</summary>
		BMI2 = 35,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EDX.CET_IBT[bit 20]</summary>
		CET_IBT = 36,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.CET_SS[bit 7]</summary>
		CET_SS = 37,
		/// <summary><c>CL1INVMB</c> instruction (Intel SCC = Single-Chip Computer)</summary>
		CL1INVMB = 38,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.CLDEMOTE[bit 25]</summary>
		CLDEMOTE = 39,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.CLFLUSHOPT[bit 23]</summary>
		CLFLUSHOPT = 40,
		/// <summary>CPUID.01H:EDX.CLFSH[bit 19]</summary>
		CLFSH = 41,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.CLWB[bit 24]</summary>
		CLWB = 42,
		/// <summary>CPUID.80000008H:EBX.CLZERO[bit 0]</summary>
		CLZERO = 43,
		/// <summary>CPUID.01H:EDX.CMOV[bit 15]</summary>
		CMOV = 44,
		/// <summary>CPUID.01H:ECX.CMPXCHG16B[bit 13]</summary>
		CMPXCHG16B = 45,
		/// <summary><c>RFLAGS.ID</c> can be toggled</summary>
		CPUID = 46,
		/// <summary>CPUID.01H:EDX.CX8[bit 8]</summary>
		CX8 = 47,
		/// <summary>CPUID.80000001H:EDX.3DNOW[bit 31]</summary>
		D3NOW = 48,
		/// <summary>CPUID.80000001H:EDX.3DNOWEXT[bit 30]</summary>
		D3NOWEXT = 49,
		/// <summary>CPUID.(EAX=12H, ECX=0H):EAX.OSS[bit 5]</summary>
		ENCLV = 50,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX[bit 29]</summary>
		ENQCMD = 51,
		/// <summary>CPUID.01H:ECX.F16C[bit 29]</summary>
		F16C = 52,
		/// <summary>CPUID.01H:ECX.FMA[bit 12]</summary>
		FMA = 53,
		/// <summary>CPUID.80000001H:ECX.FMA4[bit 16]</summary>
		FMA4 = 54,
		/// <summary>8087 or later (CPUID.01H:EDX.FPU[bit 0])</summary>
		FPU = 55,
		/// <summary>80287 or later</summary>
		FPU287 = 56,
		/// <summary>80287XL only</summary>
		FPU287XL_ONLY = 57,
		/// <summary>80387 or later</summary>
		FPU387 = 58,
		/// <summary>80387SL only</summary>
		FPU387SL_ONLY = 59,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.FSGSBASE[bit 0]</summary>
		FSGSBASE = 60,
		/// <summary>CPUID.01H:EDX.FXSR[bit 24]</summary>
		FXSR = 61,
		/// <summary>AMD Geode LX/GX CPU</summary>
		GEODE = 62,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.GFNI[bit 8]</summary>
		GFNI = 63,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.HLE[bit 4]</summary>
		HLE = 64,
		/// <summary><see cref="HLE"/> or <see cref="RTM"/></summary>
		HLE_or_RTM = 65,
		/// <summary><see cref="VMX"/> and IA32_VMX_EPT_VPID_CAP[bit 20]</summary>
		INVEPT = 66,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.INVPCID[bit 10]</summary>
		INVPCID = 67,
		/// <summary><see cref="VMX"/> and IA32_VMX_EPT_VPID_CAP[bit 32]</summary>
		INVVPID = 68,
		/// <summary>CPUID.80000001H:ECX.LWP[bit 15]</summary>
		LWP = 69,
		/// <summary>CPUID.80000001H:ECX.LZCNT[bit 5]</summary>
		LZCNT = 70,
		/// <summary>CPUID.80000008H:EBX.MCOMMIT[bit 8]</summary>
		MCOMMIT = 71,
		/// <summary>CPUID.01H:EDX.MMX[bit 23]</summary>
		MMX = 72,
		/// <summary>CPUID.01H:ECX.MONITOR[bit 3]</summary>
		MONITOR = 73,
		/// <summary>CPUID.80000001H:ECX.MONITORX[bit 29]</summary>
		MONITORX = 74,
		/// <summary>CPUID.01H:ECX.MOVBE[bit 22]</summary>
		MOVBE = 75,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.MOVDIR64B[bit 28]</summary>
		MOVDIR64B = 76,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.MOVDIRI[bit 27]</summary>
		MOVDIRI = 77,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.MPX[bit 14]</summary>
		MPX = 78,
		/// <summary>CPUID.01H:EDX.MSR[bit 5]</summary>
		MSR = 79,
		/// <summary>Multi-byte nops (<c>0F1F /0</c>): CPUID.01H.EAX[Bits 11:8] = 0110B or 1111B</summary>
		MULTIBYTENOP = 80,
		/// <summary>CPUID.0C0000000H:EAX &gt;= 0C0000001H AND CPUID.0C0000001H:EDX.ACE[Bits 7:6] = 11B ([6] = exists, [7] = enabled)</summary>
		PADLOCK_ACE = 81,
		/// <summary>CPUID.0C0000000H:EAX &gt;= 0C0000001H AND CPUID.0C0000001H:EDX.PHE[Bits 11:10] = 11B ([10] = exists, [11] = enabled)</summary>
		PADLOCK_PHE = 82,
		/// <summary>CPUID.0C0000000H:EAX &gt;= 0C0000001H AND CPUID.0C0000001H:EDX.PMM[Bits 13:12] = 11B ([12] = exists, [13] = enabled)</summary>
		PADLOCK_PMM = 83,
		/// <summary>CPUID.0C0000000H:EAX &gt;= 0C0000001H AND CPUID.0C0000001H:EDX.RNG[Bits 3:2] = 11B ([2] = exists, [3] = enabled)</summary>
		PADLOCK_RNG = 84,
		/// <summary><c>PAUSE</c> instruction (Pentium 4 or later)</summary>
		PAUSE = 85,
		/// <summary>CPUID.01H:ECX.PCLMULQDQ[bit 1]</summary>
		PCLMULQDQ = 86,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.PCOMMIT[bit 22]</summary>
		PCOMMIT = 87,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EDX.PCONFIG[bit 18]</summary>
		PCONFIG = 88,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.PKU[bit 3]</summary>
		PKU = 89,
		/// <summary>CPUID.01H:ECX.POPCNT[bit 23]</summary>
		POPCNT = 90,
		/// <summary>CPUID.80000001H:ECX.PREFETCHW[bit 8]</summary>
		PREFETCHW = 91,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.PREFETCHWT1[bit 0]</summary>
		PREFETCHWT1 = 92,
		/// <summary>CPUID.(EAX=14H, ECX=0H):EBX.PTWRITE[bit 4]</summary>
		PTWRITE = 93,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.RDPID[bit 22]</summary>
		RDPID = 94,
		/// <summary><c>RDPMC</c> instruction (Pentium MMX or later, or Pentium Pro or later)</summary>
		RDPMC = 95,
		/// <summary>CPUID.80000008H:EBX.RDPRU[bit 4]</summary>
		RDPRU = 96,
		/// <summary>CPUID.01H:ECX.RDRAND[bit 30]</summary>
		RDRAND = 97,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.RDSEED[bit 18]</summary>
		RDSEED = 98,
		/// <summary>CPUID.80000001H:EDX.RDTSCP[bit 27]</summary>
		RDTSCP = 99,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.RTM[bit 11]</summary>
		RTM = 100,
		/// <summary>CPUID.01H:EDX.SEP[bit 11]</summary>
		SEP = 101,
		/// <summary>CPUID.(EAX=12H, ECX=0H):EAX.SGX1[bit 0]</summary>
		SGX1 = 102,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.SHA[bit 29]</summary>
		SHA = 103,
		/// <summary>CPUID.80000001H:ECX.SKINIT[bit 12]</summary>
		SKINIT = 104,
		/// <summary><see cref="SKINIT"/> or <see cref="SVML"/></summary>
		SKINIT_or_SVML = 105,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EBX.SMAP[bit 20]</summary>
		SMAP = 106,
		/// <summary>CPUID.01H:ECX.SMX[bit 6]</summary>
		SMX = 107,
		/// <summary>CPUID.01H:EDX.SSE[bit 25]</summary>
		SSE = 108,
		/// <summary>CPUID.01H:EDX.SSE2[bit 26]</summary>
		SSE2 = 109,
		/// <summary>CPUID.01H:ECX.SSE3[bit 0]</summary>
		SSE3 = 110,
		/// <summary>CPUID.01H:ECX.SSE4_1[bit 19]</summary>
		SSE4_1 = 111,
		/// <summary>CPUID.01H:ECX.SSE4_2[bit 20]</summary>
		SSE4_2 = 112,
		/// <summary>CPUID.80000001H:ECX.SSE4A[bit 6]</summary>
		SSE4A = 113,
		/// <summary>CPUID.01H:ECX.SSSE3[bit 9]</summary>
		SSSE3 = 114,
		/// <summary>CPUID.80000001H:ECX.SVM[bit 2]</summary>
		SVM = 115,
		/// <summary>CPUID.8000000AH:EDX.SVML[bit 2]</summary>
		SVML = 116,
		/// <summary>CPUID.80000001H:EDX.SYSCALL[bit 11]</summary>
		SYSCALL = 117,
		/// <summary>CPUID.80000001H:ECX.TBM[bit 21]</summary>
		TBM = 118,
		/// <summary>CPUID.01H:EDX.TSC[bit 4]</summary>
		TSC = 119,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.VAES[bit 9]</summary>
		VAES = 120,
		/// <summary>CPUID.01H:ECX.VMX[bit 5]</summary>
		VMX = 121,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.VPCLMULQDQ[bit 10]</summary>
		VPCLMULQDQ = 122,
		/// <summary>CPUID.(EAX=07H, ECX=0H):ECX.WAITPKG[bit 5]</summary>
		WAITPKG = 123,
		/// <summary>CPUID.(EAX=80000008H, ECX=0H):EBX.WBNOINVD[bit 9]</summary>
		WBNOINVD = 124,
		/// <summary>CPUID.80000001H:ECX.XOP[bit 11]</summary>
		XOP = 125,
		/// <summary>CPUID.01H:ECX.XSAVE[bit 26]</summary>
		XSAVE = 126,
		/// <summary>CPUID.(EAX=0DH, ECX=1H):EAX.XSAVEC[bit 1]</summary>
		XSAVEC = 127,
		/// <summary>CPUID.(EAX=0DH, ECX=1H):EAX.XSAVEOPT[bit 0]</summary>
		XSAVEOPT = 128,
		/// <summary>CPUID.(EAX=0DH, ECX=1H):EAX.XSAVES[bit 3]</summary>
		XSAVES = 129,
		/// <summary>CPUID.8000001FH:EAX.SNP[bit 4]</summary>
		SNP = 130,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EDX.SERIALIZE[bit 14]</summary>
		SERIALIZE = 131,
		/// <summary>CPUID.(EAX=07H, ECX=0H):EDX.TSXLDTRK[bit 16]</summary>
		TSXLDTRK = 132,
		/// <summary>CPUID.80000001H:EDX.INVLPGB[bit ??]</summary>
		INVLPGB = 133,
	}
}
#endif