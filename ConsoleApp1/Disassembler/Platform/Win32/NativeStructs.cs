using System.Diagnostics.CodeAnalysis;

namespace ConsoleApp1.Disassembler.Platform.Win32
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	internal static class NativeStructs
	{
		public struct SymbolInfo
		{
			public int SizeOfStruct;
			public int TypeIndex;
			private readonly ulong Reserved1;
			private readonly ulong Reserved2;
			public int Index;
			public int Size;
			public ulong ModuleBase;
			public NativeEnums.SymbolFlags Flags;
			public long Value;
			public ulong Address;
			public uint Register;
			public uint Scope;
			public NativeEnums.SymbolTag Tag;
			public uint NameLen;
			public int MaxNameLen;
			public byte Name;
		}
	}
}