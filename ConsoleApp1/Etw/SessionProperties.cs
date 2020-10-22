using System;
using System.Diagnostics;
using ConsoleApp1.Common;
using ConsoleApp1.Common.Platform.Win32;
using NativeStructs = ConsoleApp1.Etw.Platform.Win32.NativeStructs;

namespace ConsoleApp1.Etw
{
	internal struct SessionProperties
	{
		internal CharBlob1024 Name;
		public ulong Handle;
		public int Id;
		public int BufferSizeMb;
		public int BufferQuantumKb;
#pragma warning disable 414
		public TraceProperties TraceProperties;
#pragma warning restore 414

		public SessionProperties(in CharBlob64 sessionPrefix)
		{
			const int guidStrLength = 36; //format 'D'

			var guid = Guid.NewGuid();
			var guidStr = Ptr.OfRef(stackalloc char[guidStrLength]);
			guid.TryFormat(guidStr, guidStrLength, "D");
			var nameLength = sessionPrefix.Length + 1 + guidStrLength;
			var name = new CharBlob1024(nameLength);
			var tmp = Ptr.OfRef(stackalloc char[nameLength]);
			sessionPrefix.AsReadOnlySpan().CopyTo(tmp);
			tmp[sessionPrefix.Length] = '_';
			guidStr.CopyTo(tmp + sessionPrefix.Length + 1, guidStrLength);
			name.CopyFrom(tmp.AsSpan(nameLength));
			Name = name;
			BufferQuantumKb = 64;
			Handle = NativeConstants.InvalidHandleValue;
			BufferSizeMb = Math.Max(64, Environment.ProcessorCount * 2);
			Id = -1;
			TraceProperties = default;
		}

		internal SessionProperties(CharBlob1024 sessionName)
		{
			Name = sessionName;
			BufferQuantumKb = 64;
			Handle = NativeConstants.InvalidHandleValue;
			BufferSizeMb = Math.Max(64, Environment.ProcessorCount * 2);
			Id = -1;
			TraceProperties = default;
		}

		public readonly NativeStructs.EventTraceProperties ToNativeProperties()
		{
			NativeStructs.EventTraceProperties nativeProperties = default;
			// var nativePropertiesStructSize = Unsafe.SizeOf<NativeStructs.EventTraceProperties>();

			nativeProperties.LoggerNameOffset = NativeStructs.EventTraceProperties.LoggerNameOffsetDefault;//(uint)(nativePropertiesStructSize - Unsafe.SizeOf<CharBlob1024>() * 2 - Unsafe.SizeOf<NativeStructs.EventTracePropertiesExt>());

			if (Name.Length > Platform.Win32.NativeConstants.MaxSessionNameSize - 1)
			{
				throw new ArgumentException("File name too long");
			}

			nativeProperties.LoggerName = Name;

			nativeProperties.Wnode.BufferSize = NativeStructs.EventTraceProperties.SizeOfStructure;//(uint)nativePropertiesStructSize;
			nativeProperties.Wnode.Flags = Platform.Win32.NativeConstants.WnodeFlagTracedGuid;
			nativeProperties.FlushTimer = 60; // flush every minute for file based collection.  

			Debug.Assert(BufferQuantumKb != 0);
			nativeProperties.BufferSize = (uint)BufferQuantumKb;
			nativeProperties.MinimumBuffers = (uint)(BufferSizeMb * 1024 / BufferQuantumKb);

			nativeProperties.FlushTimer = 1; // flush every second (as fast as possible) for real time. 
			nativeProperties.LogFileMode = Platform.Win32.NativeConstants.EventTraceRealTimeMode;

			nativeProperties.LogFileNameOffset = 0; // real-time logging
			nativeProperties.MaximumBuffers = nativeProperties.MinimumBuffers * 5 / 4 + 10;
			nativeProperties.Wnode.ClientContext = 1; // set Timer resolution to 100ns.  

			return nativeProperties;
		}
	}
}