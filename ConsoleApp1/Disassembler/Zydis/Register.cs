﻿namespace ConsoleApp1.Disassembler.Zydis
{
	public enum Register
	{
		None,

		// General purpose registers  8-bit
		Al,
		Cl,
		Dl,
		Bl,
		Ah,
		Ch,
		Dh,
		Bh,
		Spl,
		Bpl,
		Sil,
		Dil,
		R8B,
		R9B,
		R10B,
		R11B,
		R12B,
		R13B,
		R14B,
		R15B,

		// General purpose registers 16-bit
		Ax,
		Cx,
		Dx,
		Bx,
		Sp,
		Bp,
		Si,
		Di,
		R8W,
		R9W,
		R10W,
		R11W,
		R12W,
		R13W,
		R14W,
		R15W,

		// General purpose registers 32-bit
		Eax,
		Ecx,
		Edx,
		Ebx,
		Esp,
		Ebp,
		Esi,
		Edi,
		R8D,
		R9D,
		R10D,
		R11D,
		R12D,
		R13D,
		R14D,
		R15D,

		// General purpose registers 64-bit
		Rax,
		Rcx,
		Rdx,
		Rbx,
		Rsp,
		Rbp,
		Rsi,
		Rdi,
		R8,
		R9,
		R10,
		R11,
		R12,
		R13,
		R14,
		R15,

		// Floating point legacy registers
		St0,
		St1,
		St2,
		St3,
		St4,
		St5,
		St6,
		St7,
		X87Control,
		X87Status,
		X87Tag,

		// Floating point multimedia registers
		Mm0,
		Mm1,
		Mm2,
		Mm3,
		Mm4,
		Mm5,
		Mm6,
		Mm7,

		// Floating point vector registers 128-bit
		Xmm0,
		Xmm1,
		Xmm2,
		Xmm3,
		Xmm4,
		Xmm5,
		Xmm6,
		Xmm7,
		Xmm8,
		Xmm9,
		Xmm10,
		Xmm11,
		Xmm12,
		Xmm13,
		Xmm14,
		Xmm15,
		Xmm16,
		Xmm17,
		Xmm18,
		Xmm19,
		Xmm20,
		Xmm21,
		Xmm22,
		Xmm23,
		Xmm24,
		Xmm25,
		Xmm26,
		Xmm27,
		Xmm28,
		Xmm29,
		Xmm30,
		Xmm31,

		// Floating point vector registers 256-bit
		Ymm0,
		Ymm1,
		Ymm2,
		Ymm3,
		Ymm4,
		Ymm5,
		Ymm6,
		Ymm7,
		Ymm8,
		Ymm9,
		Ymm10,
		Ymm11,
		Ymm12,
		Ymm13,
		Ymm14,
		Ymm15,
		Ymm16,
		Ymm17,
		Ymm18,
		Ymm19,
		Ymm20,
		Ymm21,
		Ymm22,
		Ymm23,
		Ymm24,
		Ymm25,
		Ymm26,
		Ymm27,
		Ymm28,
		Ymm29,
		Ymm30,
		Ymm31,

		// Floating point vector registers 512-bit
		Zmm0,
		Zmm1,
		Zmm2,
		Zmm3,
		Zmm4,
		Zmm5,
		Zmm6,
		Zmm7,
		Zmm8,
		Zmm9,
		Zmm10,
		Zmm11,
		Zmm12,
		Zmm13,
		Zmm14,
		Zmm15,
		Zmm16,
		Zmm17,
		Zmm18,
		Zmm19,
		Zmm20,
		Zmm21,
		Zmm22,
		Zmm23,
		Zmm24,
		Zmm25,
		Zmm26,
		Zmm27,
		Zmm28,
		Zmm29,
		Zmm30,
		Zmm31,

		// Flags registers
		Flags,
		Eflags,
		Rflags,

		// Instruction-pointer registers
		Ip,
		Eip,
		Rip,

		// Segment registers
		Es,
		Cs,
		Ss,
		Ds,
		Fs,
		Gs,

		// Table registers
		Gdtr,
		Ldtr,
		Idtr,
		Tr,

		// Test registers
		Tr0,
		Tr1,
		Tr2,
		Tr3,
		Tr4,
		Tr5,
		Tr6,
		Tr7,

		// Control registers
		Cr0,
		Cr1,
		Cr2,
		Cr3,
		Cr4,
		Cr5,
		Cr6,
		Cr7,
		Cr8,
		Cr9,
		Cr10,
		Cr11,
		Cr12,
		Cr13,
		Cr14,
		Cr15,

		// Debug registers
		Dr0,
		Dr1,
		Dr2,
		Dr3,
		Dr4,
		Dr5,
		Dr6,
		Dr7,
		Dr8,
		Dr9,
		Dr10,
		Dr11,
		Dr12,
		Dr13,
		Dr14,
		Dr15,

		// Mask registers
		K0,
		K1,
		K2,
		K3,
		K4,
		K5,
		K6,
		K7,

		// Bound registers
		Bnd0,
		Bnd1,
		Bnd2,
		Bnd3,
		Bndcfg,
		Bndstatus,

		// Uncategorized
		Mxcsr,
		Pkru,
		Xcr0,
	}
}