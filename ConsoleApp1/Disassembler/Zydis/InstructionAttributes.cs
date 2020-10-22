namespace ConsoleApp1.Disassembler.Zydis
{
	public enum InstructionAttributes : ulong
	{
		/// <summary>
		/// The instruction has the `ModRM` byte.
		/// </summary>
		HasModrm = 0x0000000000000001, // (1 <<  0)

		/// <summary>
		/// The instruction has the `SIB` byte.
		/// </summary>
		HasSib = 0x0000000000000002, // (1 <<  1)

		/// <summary>
		/// The instruction has the `REX` prefix.
		/// </summary>
		HasRex = 0x0000000000000004, // (1 <<  2)

		/// <summary>
		/// The instruction has the `XOP` prefix.
		/// </summary>
		HasXop = 0x0000000000000008, // (1 <<  3)

		/// <summary>
		/// The instruction has the `VEX` prefix.
		/// </summary>
		HasVex = 0x0000000000000010, // (1 <<  4)

		/// <summary>
		/// The instruction has the `EVEX` prefix.
		/// </summary>
		HasEvex = 0x0000000000000020, // (1 <<  5)

		/// <summary>
		/// The instruction has the `MVEX` prefix.
		/// </summary>
		HasMvex = 0x0000000000000040, // (1 <<  6)

		/// <summary>
		/// The instruction has one or more operands with position-relative offsets.
		/// </summary>
		IsRelative = 0x0000000000000080, // (1 <<  7)

		/// <summary>
		/// The instruction is privileged.
		/// Privileged instructions are any instructions that require a current ring level below 3.
		/// </summary>
		IsPrivileged = 0x0000000000000100, // (1 <<  8)

		/// <summary>
		/// The instruction accesses one or more CPU-flags.
		/// </summary>
		CpuflagAccess = 0x0000001000000000, // (1 << 36) // TODO: rebase

		/// <summary>
		/// The instruction may conditionally read the general CPU state.
		/// </summary>
		CpuStateCr = 0x0000002000000000, // (1 << 37) // TODO: rebase

		/// <summary>
		/// The instruction may conditionally write the general CPU state.
		/// </summary>
		CpuStateCw = 0x0000004000000000, // (1 << 38) // TODO: rebase

		/// <summary>
		/// The instruction may conditionally read the FPU state (X87, MMX).
		/// </summary>
		FpuStateCr = 0x0000008000000000, // (1 << 39) // TODO: rebase

		/// <summary>
		/// The instruction may conditionally write the FPU state (X87, MMX).
		/// </summary>
		FpuStateCw = 0x0000010000000000, // (1 << 40) // TODO: rebase

		/// <summary>
		/// The instruction may conditionally read the XMM state (AVX, AVX2, AVX-512).
		/// </summary>
		XmmStateCr = 0x0000020000000000, // (1 << 41) // TODO: rebase

		/// <summary>
		/// The instruction may conditionally write the XMM state (AVX, AVX2, AVX-512).
		/// </summary>
		XmmStateCw = 0x0000040000000000, // (1 << 42) // TODO: rebase

		/// <summary>
		/// The instruction accepts the `LOCK` prefix (`0xF0`).
		/// </summary>
		AcceptsLock = 0x0000000000000200, // (1 <<  9)

		/// <summary>
		/// The instruction accepts the `REP` prefix (`0xF3`).
		/// </summary>
		AcceptsRep = 0x0000000000000400, // (1 << 10)

		/// <summary>
		/// The instruction accepts the `REPE`/`REPZ` prefix (`0xF3`).
		/// </summary>
		AcceptsRepe = 0x0000000000000800, // (1 << 11)

		/// <summary>
		/// The instruction accepts the `REPE`/`REPZ` prefix (`0xF3`).
		/// </summary>
		AcceptsRepz = 0x0000000000000800, // (1 << 11)

		/// <summary>
		/// The instruction accepts the `REPNE`/`REPNZ` prefix (`0xF2`).
		/// </summary>
		AcceptsRepne = 0x0000000000001000, // (1 << 12)

		/// <summary>
		/// The instruction accepts the `REPNE`/`REPNZ` prefix (`0xF2`).
		/// </summary>
		AcceptsRepnz = 0x0000000000001000, // (1 << 12)

		/// <summary>
		/// The instruction accepts the `BND` prefix (`0xF2`).
		/// </summary>
		AcceptsBnd = 0x0000000000002000, // (1 << 13)

		/// <summary>
		/// The instruction accepts the `XACQUIRE` prefix (`0xF2`).
		/// </summary>
		AcceptsXacquire = 0x0000000000004000, // (1 << 14)

		/// <summary>
		/// The instruction accepts the `XRELEASE` prefix (`0xF3`).
		/// </summary>
		AcceptsXrelease = 0x0000000000008000, // (1 << 15)

		/// <summary>
		/// The instruction accepts the `XACQUIRE`/`XRELEASE` prefixes (`0xF2`, `0xF3`) without
		/// the `LOCK` prefix (`0x0F`).
		/// </summary>
		AcceptsHleWithoutLock = 0x0000000000010000, // (1 << 16)

		/// <summary>
		/// The instruction accepts branch hints (0x2E, 0x3E).
		/// </summary>
		AcceptsBranchHints = 0x0000000000020000, // (1 << 17)

		/// <summary>
		/// The instruction accepts segment prefixes (`0x2E`, `0x36`, `0x3E`, `0x26`, `0x64`,
		/// `0x65`).
		/// </summary>
		AcceptsSegment = 0x0000000000040000, // (1 << 18)

		/// <summary>
		/// The instruction has the `LOCK` prefix (`0xF0`).
		/// </summary>
		HasLock = 0x0000000000080000, // (1 << 19)

		/// <summary>
		/// The instruction has the `REP` prefix (`0xF3`).
		/// </summary>
		HasRep = 0x0000000000100000, // (1 << 20)

		/// <summary>
		/// The instruction has the `REPE`/`REPZ` prefix (`0xF3`).
		/// </summary>
		HasRepe = 0x0000000000200000, // (1 << 21)

		/// <summary>
		/// The instruction has the `REPE`/`REPZ` prefix (`0xF3`).
		/// </summary>
		HasRepz = 0x0000000000200000, // (1 << 21)

		/// <summary>
		/// The instruction has the `REPNE`/`REPNZ` prefix (`0xF2`).
		/// </summary>
		HasRepne = 0x0000000000400000, // (1 << 22)

		/// <summary>
		/// The instruction has the `REPNE`/`REPNZ` prefix (`0xF2`).
		/// </summary>
		HasRepnz = 0x0000000000400000, // (1 << 22)

		/// <summary>
		/// The instruction has the `BND` prefix (`0xF2`).
		/// </summary>
		HasBnd = 0x0000000000800000, // (1 << 23)

		/// <summary>
		/// The instruction has the `XACQUIRE` prefix (`0xF2`).
		/// </summary>
		HasXacquire = 0x0000000001000000, // (1 << 24)

		/// <summary>
		/// The instruction has the `XRELEASE` prefix (`0xF3`).
		/// </summary>
		HasXrelease = 0x0000000002000000, // (1 << 25)

		/// <summary>
		/// The instruction has the branch-not-taken hint (`0x2E`).
		/// </summary>
		HasBranchNotTaken = 0x0000000004000000, // (1 << 26)

		/// <summary>
		/// The instruction has the branch-taken hint (`0x3E`).
		/// </summary>
		HasBranchTaken = 0x0000000008000000, // (1 << 27)

		/// <summary>
		/// The instruction has a segment modifier.
		/// </summary>
		HasSegment = 0x00000003F0000000,

		/// <summary>
		/// The instruction has the `CS` segment modifier (`0x2E`).
		/// </summary>
		HasSegmentCs = 0x0000000010000000, // (1 << 28)

		/// <summary>
		/// The instruction has the `SS` segment modifier (`0x36`).
		/// </summary>
		HasSegmentSs = 0x0000000020000000, // (1 << 29)

		/// <summary>
		/// The instruction has the `DS` segment modifier (`0x3E`).
		/// </summary>
		HasSegmentDs = 0x0000000040000000, // (1 << 30)

		/// <summary>
		/// The instruction has the `ES` segment modifier (`0x26`).
		/// </summary>
		HasSegmentEs = 0x0000000080000000, // (1 << 31)

		/// <summary>
		/// The instruction has the `FS` segment modifier (`0x64`).
		/// </summary>
		HasSegmentFs = 0x0000000100000000, // (1 << 32)

		/// <summary>
		/// The instruction has the `GS` segment modifier (`0x65`).
		/// </summary>
		HasSegmentGs = 0x0000000200000000, // (1 << 33)

		/// <summary>
		/// The instruction has the operand-size override prefix (`0x66`).
		/// </summary>
		HasOperandSize = 0x0000000400000000, // (1 << 34) // TODO: rename

		/// <summary>
		/// The instruction has the address-size override prefix (`0x67`).
		/// </summary>
		HasAddressSize = 0x0000000800000000, // (1 << 35) // TODO: rename
	}
}