using System;

namespace ConsoleApp1.Disassembler.Platform.Win32
{
	public static class NativeEnums
	{
		[Flags]
		public enum SymbolOptions : uint
		{
			None = 0,
			AllowAbsoluteSymbols = 0x800,
			AllowZeroAddress = 0x1000000,
			AutoPublics = 0x10000,
			CaseInsensitive = 1,
			Debug = 0x80000000,
			DeferredLoads = 0x4,
			DisableSymSrvAutoDetect = 0x2000000,
			ExactSymbols = 0x400,
			FailCriticalErrors = 0x200,
			FavorCompressed = 0x800000,
			FlatDirectory = 0x400000,
			IgnoreCodeViewRecord = 0x80,
			IgnoreImageDir = 0x200000,
			IgnoreNTSymbolPath = 0x1000,
			Include32BitModules = 0x2000,
			LoadAnything = 0x40,
			LoadLines = 0x10,
			NoCPP = 0x8,
			NoImageSearch = 0x20000,
			NoPrompts = 0x80000,
			NoPublics = 0x8000,
			NoUnqualifiedLoads = 0x100,
			Overwrite = 0x100000,
			PublicsOnly = 0x4000,
			Secure = 0x40000,
			UndecorateNames = 0x2
		}

		[Flags]
		public enum SymbolFlags : uint
		{
			None = 0,
			ClrToken = 0x40000,
			Constant = 0x100,
			Export = 0x200,
			Forwarder = 0x400,
			FrameRelative = 0x20,
			Function = 0x800,
			ILRelative = 0x10000,
			Local = 0x80,
			Metadata = 0x20000,
			Parameter = 0x40,
			Register = 0x8,
			RegisterRelative = 0x10,
			Slot = 0x8000,
			Thunk = 0x2000,
			TLSRelative = 0x4000,
			ValuePresent = 1,
			Virtual = 0x1000,
			Null = 0x80000,
			FunctionNoReturn = 0x100000,
			SyntheticZeroBase = 0x200000,
			PublicCode = 0x400000
		}

		public enum SymbolTag
		{
			Null,
			Exe,
			Compiland,
			CompilandDetails,
			CompilandEnv,
			Function,
			Block,
			Data,
			Annotation,
			Label,
			PublicSymbol,
			UDT,
			Enum,
			FunctionType,
			PointerType,
			ArrayType,
			BaseType,
			Typedef,
			BaseClass,
			Friend,
			FunctionArgType,
			FuncDebugStart,
			FuncDebugEnd,
			UsingNamespace,
			VTableShape,
			VTable,
			Custom,
			Thunk,
			CustomType,
			ManagedType,
			Dimension,
			CallSite,
			InlineSite,
			BaseInterface,
			VectorType,
			MatrixType,
			HLSLType,
			Caller,
			Callee,
			Export,
			HeapAllocationSite,
			CoffGroup,
			Max
		}
	}
}