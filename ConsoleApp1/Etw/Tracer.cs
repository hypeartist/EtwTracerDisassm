using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ConsoleApp1.Common;
using ConsoleApp1.Etw.Packets;
using ConsoleApp1.Etw.Platform.Win32;

namespace ConsoleApp1.Etw
{
	public sealed class Tracer : IDisposable
	{
		private enum ClrTask
		{
			Method = 0x09,
			ILStub = 0x0f,
			Loader = 0x0a,
			AssemblyLoader = 0x20,
			EEStartup = 0x13
		}

		private enum ClrTaskVersion
		{
			Default = 0x00,
			Version1 = 0x01,
			Version2 = 0x02
		}

		private enum ClrOpcode
		{
			RuntimeInformationStart = 0x01,
			AssemblyLoadStart = 0x01,
			AppDomainLoad = 0x29,
			MethodLoadVerbose = 0x25,
			MethodJittingStarted = 0x2a,
			MethodDetails = 0x2b,
			MethodJitInliningSucceeded = 0x53,
			MethodJitInliningFailed = 0x54,
			ILStubGenerated = 0x58,
			MethodILToNativeMap = 0x57
		}

		private enum ClrEvent
		{
			RuntimeInformationStart = 0xbb,
			AssemblyLoadStart = 0x122,
			AppDomainLoad = 0x9c,
			ILStubGenerated = 0x58,
			MethodLoadVerbose = 0x8f,
			MethodJittingStarted = 0x91,
			MethodDetails = 0x48,
			MethodJitInliningSucceeded = 0xb9,
			MethodJitInliningFailed = 0xc0,
			MethodILToNativeMap = 0xbe
		}

		private static readonly Guid MicrosoftWindowsDotNETRuntime = new Guid(unchecked((int) 0xe13c0d23), unchecked((short) 0xccbc), 0x4e12, 0x93, 0x1b, 0xd9, 0xcc, 0x2e, 0xee, 0x27, 0xe4);

		private readonly CharBlob64 _sessionPrefix = default;
		private SessionProperties _sessionProperties;
		private Keywords _keywords;
		private TargetProcess _targetProcess;
		private bool _running;
		private readonly NativeCallbacks.EventTraceBufferCallback _eventTraceBufferCallback;
		private readonly NativeCallbacks.EventTraceEventCallback _eventTraceEventCallback;

		private event RuntimeInformationStartCallback RuntimeInformationStartEvent;
		private event MethodLoadUnloadVerboseCallback MethodLoadVerboseEvent;
		private event MethodILToNativeMapCallback MethodILToNativeMapEvent;
		private event MethodJittingStartedCallback MethodJittingStartedEvent;
		private event MethodDetailsCallback MethodDetailsEvent;
		private event MethodJitInliningFailedCallback MethodJitInliningFailedEvent;
		private event MethodJitInliningSucceededCallback MethodJitInliningSucceededEvent;
		private event ILStubGeneratedCallback ILStubGeneratedEvent;
		private event AssemblyLoadStartCallback AssemblyLoadStartEvent;
		private event AppDomainLoadCallback AppDomainLoadEvent;

		public Tracer(ReadOnlySpan<char> sessionPrefix)
		{
			_sessionPrefix.CopyFrom(sessionPrefix);
			_sessionProperties = new SessionProperties(_sessionPrefix);
			_eventTraceBufferCallback = TraceEventBufferCallback;
			_eventTraceEventCallback = DispatchNativeEventRecord;
		}

		public bool StartSession(ReadOnlySpan<char> targetPath, TraceLevel traceLevel = TraceLevel.Verbose) => StartSession(targetPath, default, traceLevel);

		public bool StartSession(ReadOnlySpan<char> targetPath, ReadOnlySpan<char> args, TraceLevel traceLevel = TraceLevel.Verbose)
		{
			if ((_targetProcess = TargetProcess.Create(targetPath, args)) == null || !CloseRemnantSessions() || !CreateSession() || !SetProcessFilterAndEnableProvider(traceLevel))
			{
				return false;
			}

			_targetProcess.Terminated += OnTargetProcessTerminated;
			Task.Run(StartTrace);
			_running = true;
			_targetProcess.Start();
			return true;
		}

		private void OnTargetProcessTerminated(object sender, EventArgs e) => StopSession();

		public bool StopSession()
		{
			if (!_running)
			{
				return false;
			}

			_running = false;

			if (_targetProcess != null)
			{
				_targetProcess.Terminated -= OnTargetProcessTerminated;
				_targetProcess.Terminate();
				_targetProcess = null;
			}

			return StopTrace(_sessionProperties) == 0;
			;
		}

		private bool TraceEventBufferCallback(IntPtr rawLogFile)
		{
			return true; //!stopProcessing;
		}

		private void DispatchNativeEventRecord(in NativeStructs.EventRecord eventRecord)
		{
			var processId = eventRecord.EventHeader.ProcessId;
			var opcode = (ClrOpcode) eventRecord.EventHeader.Opcode;
			var id = (ClrEvent) eventRecord.EventHeader.Id;
			var taskId = (ClrTask) eventRecord.EventHeader.Task;
			// var version = (ClrTaskVersion)eventRecord.EventHeader.Version;
			// var keyword = (Keywords)eventRecord.EventHeader.Keyword;

			// Debug.WriteLine($"keyword: {keyword}, opcode: {opcode:X}, event: {id:X}, task: {taskId:X}, version: {version}");

			switch (opcode)
			{
				case ClrOpcode.RuntimeInformationStart when id == ClrEvent.RuntimeInformationStart && taskId == ClrTask.EEStartup:
				{
					var packet = new RuntimeInformationStartPacket(_targetProcess, eventRecord.UserData.As<byte>(), eventRecord.UserDataLength);
					RuntimeInformationStartEvent?.Invoke(ref packet);
					break;
				}
				case ClrOpcode.MethodLoadVerbose when id == ClrEvent.MethodLoadVerbose && taskId == ClrTask.Method:
				{
					var packet = new MethodLoadVerbosePacket(processId, eventRecord.UserData.As<byte>(), eventRecord.UserDataLength);
					MethodLoadVerboseEvent?.Invoke(ref packet);
					break;
				}
				case ClrOpcode.MethodJittingStarted when id == ClrEvent.MethodJittingStarted && taskId == ClrTask.Method:
				{
					var packet = new MethodJittingStartedPacket(processId, eventRecord.UserData.As<byte>(), eventRecord.UserDataLength);
					MethodJittingStartedEvent?.Invoke(ref packet);
					break;
				}
				case ClrOpcode.MethodJitInliningFailed when id == ClrEvent.MethodJitInliningFailed && taskId == ClrTask.Method:
				{
					var packet = new MethodJitInliningFailedPacket(processId, eventRecord.UserData.As<byte>(), eventRecord.UserDataLength);
					MethodJitInliningFailedEvent?.Invoke(ref packet);
					break;
				}
				case ClrOpcode.MethodJitInliningSucceeded when id == ClrEvent.MethodJitInliningSucceeded && taskId == ClrTask.Method:
				{
					var packet = new MethodJitInliningSucceededPacket(processId, eventRecord.UserData.As<byte>(), eventRecord.UserDataLength);
					MethodJitInliningSucceededEvent?.Invoke(ref packet);
					break;
				}
				case ClrOpcode.MethodDetails when id == ClrEvent.MethodDetails && taskId == ClrTask.Method:
				{
					var packet = new MethodDetailsPacket(processId, eventRecord.UserData.As<byte>(), eventRecord.UserDataLength);
					MethodDetailsEvent?.Invoke(ref packet);
					break;
				}
				case ClrOpcode.MethodILToNativeMap when id == ClrEvent.MethodILToNativeMap && taskId == ClrTask.Method:
				{
					var packet = new MethodILToNativeMapPacket(processId, eventRecord.UserData.As<byte>());
					MethodILToNativeMapEvent?.Invoke(ref packet);
					break;
				}
				// case ClrOpcode.ILStubGenerated when id == ClrEvent.ILStubGenerated && taskId == ClrTask.ILStub:
				// {
				// 	var packet = new ILStubGeneratedPacket(processId, eventRecord.UserData.As<byte>(), eventRecord.UserDataLength);
				// 	Debug.WriteLine($"{packet.StubMethodId:x16}");
				// 	var str = new string(packet.StubMethodILCode);
				// 	ILStubGeneratedEvent?.Invoke(ref packet);
				// 	break;
				// }
				// case ClrOpcode.AssemblyLoadStart when id == ClrEvent.AssemblyLoadStart && taskId == ClrTask.AssemblyLoader:
				// {
				// 	var packet = new AssemblyLoadStartPacket(processId, (byte*)eventRecord.UserData, eventRecord.UserDataLength);
				// 	AssemblyLoadStartEvent?.Invoke(ref packet);
				// 	break;
				// }
				// case ClrOpcode.AppDomainLoad when id == ClrEvent.AppDomainLoad && taskId == ClrTask.Loader:
				// {
				// 	var packet = new AppDomainLoadPacket(processId, (byte*)eventRecord.UserData, eventRecord.UserDataLength);
				// 	AppDomainLoadEvent?.Invoke(ref packet);
				// 	break;
				// }
			}
		}

		private unsafe bool CreateSession()
		{
			var nativeProperties = _sessionProperties.ToNativeProperties();
			var hr = NativeMethods.StartTraceW(out _sessionProperties.Handle, _sessionProperties.Name, ref nativeProperties);
			if (hr != 0)
			{
				return false;
			}

			_sessionProperties.Id = (int) nativeProperties.Wnode.HistoricalContext;
			ref var traceProperties = ref _sessionProperties.TraceProperties;
			ref var logFile = ref traceProperties.LogFile;
			logFile.LoggerName = (IntPtr) Unsafe.AsPointer(ref Unsafe.AsRef(_sessionProperties.Name));
			logFile.LogFileMode |= NativeConstants.EventTraceRealTimeMode;
			logFile.BufferCallback = Marshal.GetFunctionPointerForDelegate(_eventTraceBufferCallback);

			logFile.LogFileMode |= NativeConstants.ProcessTraceModeEventRecord;
			logFile.EventCallback = Marshal.GetFunctionPointerForDelegate(_eventTraceEventCallback);
			logFile.LogFileMode |= NativeConstants.ProcessTraceModeRawTimestamp;
			var handle = NativeMethods.OpenTrace(ref logFile);
			if (handle == Common.Platform.Win32.NativeConstants.InvalidHandleValue)
			{
				return false;
			}

			traceProperties.TraceHandle = handle;
			return true;
		}

		private unsafe bool SetProcessFilterAndEnableProvider(TraceLevel traceLevel)
		{
			if (_keywords == 0)
			{
				return false;
			}

			NativeStructs.EventFilterDescriptor filter = default;
			var processId = _targetProcess.Id;
			filter.Ptr = &processId;
			filter.Size = sizeof(int);
			filter.Type = NativeConstants.EventFilterTypePid;

			NativeStructs.EnableTraceParameters parameters = default;
			parameters.Version = NativeConstants.EnableTraceParametersVersion2;
			parameters.FilterDescCount = 1;
			parameters.EnableFilterDesc = &filter;

			var hr = NativeMethods.EnableTraceEx2(_sessionProperties.Handle, MicrosoftWindowsDotNETRuntime, NativeConstants.EventControlCodeEnableProvider, (byte) traceLevel, (ulong) _keywords, 0, 0, ref parameters);
			return hr == 0;
		}

		private bool StartTrace()
		{
			return NativeMethods.ProcessTrace(Ptr.OfReadOnlyRef(_sessionProperties.TraceProperties.TraceHandle), 1, 0, 0) == 0;
		}

		private static int AttachToSession(in CharBlob1024 sessionName, out SessionProperties properties)
		{
			properties = new SessionProperties(sessionName);
			var nativeProperties = properties.ToNativeProperties();
			if (NativeMethods.ControlTrace(0UL, sessionName, ref nativeProperties, NativeConstants.EventTraceControlQuery) == NativeConstants.ErrorWmiInstanceNotFound)
			{
				return (int) NativeConstants.ErrorWmiInstanceNotFound;
			}

			properties.Id = (int) nativeProperties.Wnode.HistoricalContext;

			if (nativeProperties.BufferSize != 0)
			{
				properties.BufferQuantumKb = (int) nativeProperties.BufferSize;
			}

			properties.BufferSizeMb = (int) (nativeProperties.MinimumBuffers * properties.BufferQuantumKb) / 1024;

			return 0;
		}

		private static int StopTrace(in SessionProperties properties)
		{
			var nativeProperties = properties.ToNativeProperties();
			return NativeMethods.ControlTrace(0UL, properties.Name, ref nativeProperties, NativeConstants.EventTraceControlStop);
		}

		private bool CloseRemnantSessions()
		{
			var activeSessions = Ptr.OfRef(stackalloc CharBlob1024[NativeConstants.MaxSessions]);
			var count = GetActiveSessionNames(activeSessions);
			for (var i = 0; i < count; i++)
			{
				var session = activeSessions[i];
				if (!session.AsReadOnlySpan().StartsWith(_sessionPrefix.AsReadOnlySpan())) continue;
				if (AttachToSession(session, out var properties) != 0 || StopTrace(properties) != 0)
				{
					return false;
				}
			}

			return true;
		}

		private static unsafe int GetActiveSessionNames(Ptr<CharBlob1024> list)
		{
			// var nativePropertiesStructSize = Unsafe.SizeOf<NativeStructs.EventTraceProperties>();

			var sessionsArray = Ptr.OfRef(stackalloc NativeStructs.EventTraceProperties[NativeConstants.MaxSessions]);
			var propetiesArray = Ptr.OfRef(stackalloc Ptr<NativeStructs.EventTraceProperties>[NativeConstants.MaxSessions]);

			for (var i = 0; i < NativeConstants.MaxSessions; i++)
			{
				ref var nativeProperties = ref sessionsArray[i];
				nativeProperties.Wnode.BufferSize = NativeStructs.EventTraceProperties.SizeOfStructure;//(uint) nativePropertiesStructSize;
				nativeProperties.LoggerNameOffset = NativeStructs.EventTraceProperties.LoggerNameOffsetDefault;//(uint) (nativePropertiesStructSize - Unsafe.SizeOf<CharBlob1024>() * 2 - Unsafe.SizeOf<NativeStructs.EventTracePropertiesExt>());
				nativeProperties.LogFileNameOffset = 0;//(uint) (nativeProperties.LoggerNameOffset + Unsafe.SizeOf<CharBlobSessionMax>());
				propetiesArray[i] = Ptr.OfRef(ref nativeProperties);
			}

			var sessionCount = 0;
			var hr = NativeMethods.QueryAllTraces(propetiesArray, NativeConstants.MaxSessions, ref sessionCount);

			if (hr != 0)
			{
				return 0;
			}

			for (var i = 0; i < sessionCount; i++)
			{
				var namePtr = (propetiesArray[i].As<byte>() + propetiesArray[i].Value.LoggerNameOffset).As<char>();
				var nameLength = FindStringLength(namePtr);
				Ptr.OfReadOnlyRef(list[i].Length).Value = nameLength;
				namePtr.CopyTo(list + i, nameLength);
			}

			static int FindStringLength(Ptr<char> start)
			{
				var pos = 0;
				while (pos < NativeConstants.MaxSessionNameSize - 1)
				{
					if (pos > 0 && start[pos] == 0)
					{
						return pos;
					}

					pos++;
				}

				return NativeConstants.MaxSessionNameSize;
			}

			return sessionCount;
		}

		public event RuntimeInformationStartCallback RuntimeInformationStart
		{
			add
			{
				_keywords |= Keywords.Runtime;
				RuntimeInformationStartEvent += value;
			}
			remove
			{
				_keywords ^= Keywords.Runtime;
				RuntimeInformationStartEvent -= value;
			}
		}

		public event MethodLoadUnloadVerboseCallback MethodLoadVerbose
		{
			add
			{
				_keywords |= Keywords.Jit;
				MethodLoadVerboseEvent += value;
			}
			remove
			{
				_keywords ^= Keywords.Jit;
				MethodLoadVerboseEvent -= value;
			}
		}

		public event MethodJittingStartedCallback MethodJittingStarted
		{
			add
			{
				_keywords |= Keywords.Jit;
				MethodJittingStartedEvent += value;
			}
			remove
			{
				_keywords ^= Keywords.Jit;
				MethodJittingStartedEvent -= value;
			}
		}

		public event MethodDetailsCallback MethodDetailsStarted
		{
			add
			{
				_keywords |= Keywords.MethodDiagnostic;
				MethodDetailsEvent += value;
			}
			remove
			{
				_keywords ^= Keywords.MethodDiagnostic;
				MethodDetailsEvent -= value;
			}
		}

		public event MethodJitInliningFailedCallback MethodJitInliningFailed
		{
			add
			{
				_keywords |= Keywords.JitTracing;
				MethodJitInliningFailedEvent += value;
			}
			remove
			{
				_keywords ^= Keywords.JitTracing;
				MethodJitInliningFailedEvent -= value;
			}
		}

		public event MethodJitInliningSucceededCallback MethodJitInliningSucceeded
		{
			add
			{
				_keywords |= Keywords.JitTracing;
				MethodJitInliningSucceededEvent += value;
			}
			remove
			{
				_keywords ^= Keywords.JitTracing;
				MethodJitInliningSucceededEvent -= value;
			}
		}

		public event MethodILToNativeMapCallback MethodILToNativeMap
		{
			add
			{
				_keywords |= Keywords.JittedMethodIlToNativeMap;
				MethodILToNativeMapEvent += value;
			}
			remove
			{
				_keywords ^= Keywords.JittedMethodIlToNativeMap;
				MethodILToNativeMapEvent -= value;
			}
		}

		public event ILStubGeneratedCallback ILStubGenerated
		{
			add
			{
				_keywords |= Keywords.Interop;
				ILStubGeneratedEvent += value;
			}
			remove
			{
				_keywords ^= Keywords.Interop;
				ILStubGeneratedEvent -= value;
			}
		}

		public event AssemblyLoadStartCallback AssemblyLoadStart
		{
			add
			{
				_keywords |= Keywords.AssemblyLoader;
				AssemblyLoadStartEvent += value;
			}
			remove
			{
				_keywords ^= Keywords.AssemblyLoader;
				AssemblyLoadStartEvent -= value;
			}
		}

		public event AppDomainLoadCallback AppDomainLoad
		{
			add
			{
				_keywords |= Keywords.Loader;
				AppDomainLoadEvent += value;
			}
			remove
			{
				_keywords ^= Keywords.Loader;
				AppDomainLoadEvent -= value;
			}
		}

		public void Dispose() => StopSession();
	}
}