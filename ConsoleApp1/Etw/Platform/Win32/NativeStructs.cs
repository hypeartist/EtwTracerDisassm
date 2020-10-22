using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw.Platform.Win32
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	internal unsafe static class NativeStructs
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct WnodeHeader
		{
			public uint BufferSize;
			public readonly uint ProviderId;
			public readonly ulong HistoricalContext;
			public readonly ulong TimeStamp;
			public readonly Guid Guid;
			public uint ClientContext; // Determines the time stamp resolution
			public uint Flags;
		}

		[StructLayout(LayoutKind.Sequential, Size = NativeConstants.MaxExtensionSize)]
		public struct EventTracePropertiesExt
		{
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct EventTraceProperties
		{
			public static readonly uint SizeOfStructure = (uint) Unsafe.SizeOf<EventTraceProperties>();
			public static readonly uint LoggerNameOffsetDefault = (uint) (SizeOfStructure - Unsafe.SizeOf<CharBlob1024>() * 2 - Unsafe.SizeOf<EventTracePropertiesExt>());

			public WnodeHeader Wnode; // Timer Resolution determined by the Wnode.ClientContext.  
			public uint BufferSize;
			public uint MinimumBuffers;
			public uint MaximumBuffers;
			public readonly uint MaximumFileSize;
			public uint LogFileMode;
			public uint FlushTimer;
			public readonly uint EnableFlags;
			public readonly int AgeLimit;
			public readonly uint NumberOfBuffers;
			public readonly uint FreeBuffers;
			public readonly uint EventsLost;
			public readonly uint BuffersWritten;
			public readonly uint LogBuffersLost;
			public readonly uint RealTimeBuffersLost;
			public readonly IntPtr LoggerThreadId;
			public uint LogFileNameOffset;
			public uint LoggerNameOffset;
			public CharBlob1024 LoggerName;
			private readonly CharBlob1024 LogFileName;
			private readonly EventTracePropertiesExt _ext;
		}

		[StructLayout(LayoutKind.Sequential)]
		public readonly struct EventTraceHeader
		{
			public readonly ushort Size;
			public readonly ushort FieldTypeFlags;
			public readonly byte Type;
			public readonly byte Level;
			public readonly ushort Version;
			public readonly int ThreadId;
			public readonly int ProcessId;
			public readonly long TimeStamp;
			public readonly Guid Guid;
			public readonly int KernelTime; // Offset 0x28
			public readonly int UserTime;
		}

		[StructLayout(LayoutKind.Sequential)]
		public readonly ref struct EventHeader
		{
			public readonly ushort Size;
			public readonly ushort HeaderType;
			public readonly ushort Flags; // offset: 0x4
			public readonly ushort EventProperty;
			public readonly int ThreadId; // offset: 0x8
			public readonly int ProcessId; // offset: 0xc
			public readonly long TimeStamp; // offset: 0x10
			public readonly Guid ProviderId; // offset: 0x18
			public readonly ushort Id; // offset: 0x28
			public readonly byte Version; // offset: 0x2a
			public readonly byte Channel;
			public readonly byte Level; // offset: 0x2c
			public readonly byte Opcode;
			public readonly ushort Task;
			public readonly ulong Keyword;
			public readonly int KernelTime; // offset: 0x38
			public readonly int UserTime; // offset: 0x3C
			public readonly Guid ActivityId;
		}

		[StructLayout(LayoutKind.Sequential)]
		public readonly struct EtwBufferContext
		{
			public readonly byte ProcessorNumber;
			public readonly byte Alignment;
			public readonly ushort LoggerId;
		}

		[StructLayout(LayoutKind.Sequential)]
		public readonly ref struct EventHeaderExtendedDataItem
		{
			public readonly ushort Reserved1;
			public readonly ushort ExtType;
			public readonly ushort Reserved2;
			public readonly ushort DataSize;
			public readonly ulong DataPtr;
		};

		[StructLayout(LayoutKind.Sequential)]
		public readonly ref struct EventRecord
		{
			public readonly EventHeader EventHeader; //  size: 80
			public readonly EtwBufferContext BufferContext; //  size: 4
			public readonly ushort ExtendedDataCount;
			public readonly ushort UserDataLength; //  offset: 86
			public readonly EventHeaderExtendedDataItem* ExtendedData;
			public readonly Ptr UserData;
			public readonly Ptr UserContext;
		}

		[StructLayout(LayoutKind.Sequential)]
		public readonly struct EventTrace
		{
			public readonly EventTraceHeader Header;
			public readonly uint InstanceId;
			public readonly uint ParentInstanceId;
			public readonly Guid ParentGuid;
			public readonly IntPtr MofData; // PVOID
			public readonly int MofLength;
			public readonly EtwBufferContext BufferContext;
		}

		[StructLayout(LayoutKind.Sequential, Size = 0xac, CharSet = CharSet.Unicode)]
		public struct TimeZoneInformation
		{
		}

		[StructLayout(LayoutKind.Sequential)]
		public readonly struct TraceLogFileHeader
		{
			public readonly uint BufferSize;
			public readonly uint Version; // This is for the operating system it was collected on.  Major, Minor, SubVerMajor, subVerMinor
			public readonly uint ProviderVersion;
			public readonly uint NumberOfProcessors;
			public readonly long EndTime; // 0x10
			public readonly uint TimerResolution;
			public readonly uint MaximumFileSize;
			public readonly uint LogFileMode; // 0x20
			public readonly uint BuffersWritten;
			public readonly uint StartBuffers;
			public readonly uint PointerSize;
			public readonly uint EventsLost; // 0x30
			public readonly uint CpuSpeedInMHz;
			public readonly IntPtr LoggerName; // string, but not CoTaskMemAlloc'd
			public readonly IntPtr LogFileName; // string, but not CoTaskMemAlloc'd
			public readonly TimeZoneInformation TimeZone; // 0x40         0xac size
			public readonly long BootTime;
			public readonly long PerfFreq;
			public readonly long StartTime;
			public readonly uint ReservedFlags;
			public readonly uint BuffersLost; // 0x10C?        
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct EventTraceLogFileW
		{
			public readonly IntPtr LogFileName;
			public IntPtr LoggerName;
			public readonly long CurrentTime;
			public readonly uint BuffersRead;
			public uint LogFileMode;
			public readonly EventTrace CurrentEvent;
			public readonly TraceLogFileHeader LogFileHeader;
			public IntPtr BufferCallback;
			public readonly int BufferSize;
			public readonly int Filled;
			public readonly int EventsLost;
			public IntPtr EventCallback;
			public readonly int IsKernelTrace; // TRUE for kernel logfile
			public readonly IntPtr Context; // reserved for internal use
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct EventFilterDescriptor
		{
			[FieldOffset(0)]
			public int* Ptr; // Data
			[FieldOffset(8)]
			public int Size;
			[FieldOffset(12)]
			public int Type; // Can be user defined, but also the EVENT_FILTER_TYPE* constants above.  
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct EnableTraceParameters
		{
			public uint Version;
			public readonly uint EnableProperty;
			public readonly uint ControlFlags;
			public readonly Guid SourceId;
			public EventFilterDescriptor* EnableFilterDesc;
			public int FilterDescCount; // Only used for V2 (Win 8.1)
		}
	}
}