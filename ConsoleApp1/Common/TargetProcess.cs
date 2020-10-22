using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ConsoleApp1.ClrTracer;
using ConsoleApp1.Common.Platform.Win32;

namespace ConsoleApp1.Common
{
	internal sealed class TargetProcess : IDisposable
	{
		private delegate void WaitOrTimerCallback([In] IntPtr lpParameter, [In] bool timerOrWaitFired);

		private Ptr _waitHandle;
		private NativeStructs.ProcessInformation _processInfo;
		private bool _started;
		private readonly WaitOrTimerCallback _waitOrTimerCallback;

		private TargetProcess(NativeStructs.ProcessInformation processInfo)
		{
			_processInfo = processInfo;
			_waitOrTimerCallback = WaitOrTimer;
		}

		public static unsafe TargetProcess Create(ReadOnlySpan<char> targetPath, ReadOnlySpan<char> args = default)
		{
			NativeStructs.Startupinfo sInfo = default;

			if (NativeMethods.CreateProcess((IntPtr)Unsafe.AsPointer(ref Unsafe.AsRef(targetPath[0])), args.Length == 0 ? IntPtr.Zero : (IntPtr)Unsafe.AsPointer(ref Unsafe.AsRef(args[0])), default, default, false, NativeConstants.CreateSuspended, IntPtr.Zero, null, ref sInfo, out var targetProcessInfo) == 0)
			{
				return null;
			}
			var process = new TargetProcess(targetProcessInfo);
			var dummyContext = 1;
			return NativeMethods.RegisterWaitForSingleObject(ref process._waitHandle, targetProcessInfo.hProcess, Marshal.GetFunctionPointerForDelegate(process._waitOrTimerCallback), (IntPtr) (&dummyContext), unchecked((ulong) -1), 0x00000008) == 0 ? null : process;
		}

		public IntPtr Handle => _processInfo.hProcess;

		public int Id => _processInfo.dwProcessId;

		public event EventHandler Terminated;

		public void Start()
		{
			if (_started || _processInfo.hProcess == IntPtr.Zero) return;
			NativeMethods.ResumeThread(_processInfo.hThread);
			_started = true;
		}

		public void Terminate()
		{
			if (_processInfo.hProcess == IntPtr.Zero) return;

			NativeMethods.CloseHandle(_processInfo.hThread);
			NativeMethods.UnregisterWaitEx(_waitHandle, IntPtr.Zero);
			NativeMethods.TerminateProcess(_processInfo.hProcess, 0);
			NativeMethods.CloseHandle(_processInfo.hProcess);

			_waitHandle = IntPtr.Zero;
			_processInfo = default;
			_started = false;

			Terminated?.Invoke(this, EventArgs.Empty);
		}

		private void WaitOrTimer(IntPtr lpParameter, bool timerOrWaitFired) => Terminate();

		public void Dispose()
		{
			Terminate();
		}
	}
}
