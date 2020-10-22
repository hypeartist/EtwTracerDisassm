using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.Etw.Packets
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "NotAccessedField.Global")]
	public readonly ref struct RuntimeInformationStartPacket
	{
		[Flags]
		public enum RuntimeSku
		{
			None = 0,
			DesktopClr = 0x1,
			CoreClr = 0x2,
		}

		[Flags]
		public enum StartupMode : byte
		{
			None = 0x00,
			ManagedExecutable = 0x01,
			HostedClr = 0x02,
			CppManagedInterop = 0x04,
			ComActivated = 0x08,
			Other = 0x10
		}

		[Flags]
		public enum StartupFlags
		{
			None = 0,
			ConcurrentGc = 0x000001,
			LoaderOptimizationSingleDomain = 0x000002,
			LoaderOptimizationMultiDomain = 0x000004,
			LoaderSafeMode = 0x000010,
			LoaderSetPreference = 0x000100,
			ServerGc = 0x001000,
			HoardGcVm = 0x002000,
			SingleVersionHostingInterface = 0x004000,
			LegacyImpersonation = 0x010000,
			DisableCommitThreadStack = 0x020000,
			AlwaysFlowImpersonation = 0x040000,
			TrimGcCommit = 0x080000,
			Etw = 0x100000,
			ServerBuild = 0x200000,
			Arm = 0x400000,
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private readonly struct RuntimeInformationStartNativeData
		{
			public readonly ushort ClrInstanceID;
			public readonly ushort Sku;
			public readonly ushort BclMajorVersion;
			public readonly ushort BclMinorVersion;
			public readonly ushort BclBuildVersion;
			public readonly ushort BclRevisionVersion;
			public readonly ushort VmMajorVersion;
			public readonly ushort VmMinorVersion;
			public readonly ushort VmBuildVersion;
			public readonly ushort VmRevisionVersion;
			public readonly uint Flags;
			public readonly byte Mode;
		}

		public readonly ProcessWrapper Process;
		public readonly RuntimeSku Sku;
		public readonly ushort BclMajorVersion;
		public readonly ushort BclMinorVersion;
		public readonly ushort BclBuildVersion;
		public readonly ushort BclRevisionVersion;
		public readonly ushort VmMajorVersion;
		public readonly ushort VmMinorVersion;
		public readonly ushort VmBuildVersion;
		public readonly ushort VmRevisionVersion;
		public readonly StartupFlags Flags;
		public readonly StartupMode Mode;
		public readonly ReadOnlySpan<char> CommandLine;
		public readonly Guid ComGuid;

		public readonly ReadOnlySpan<char> DllPath;

		internal RuntimeInformationStartPacket(TargetProcess targetProcess, Ptr<byte> userData, ushort userDataLength)
		{
			Process = new ProcessWrapper(targetProcess.Handle, targetProcess.Id);

			var data = userData.As<RuntimeInformationStartNativeData>();
			Sku = (RuntimeSku) data.Value.Sku;
			BclMajorVersion = data.Value.BclMajorVersion;
			BclMinorVersion = data.Value.BclMinorVersion;
			BclBuildVersion = data.Value.BclBuildVersion;
			BclRevisionVersion = data.Value.BclRevisionVersion;
			VmMajorVersion = data.Value.VmMajorVersion;
			VmMinorVersion = data.Value.VmMinorVersion;
			VmBuildVersion = data.Value.VmBuildVersion;
			VmRevisionVersion = data.Value.VmRevisionVersion;
			Flags = (StartupFlags) data.Value.Flags;
			Mode = (StartupMode) data.Value.Mode;

			var tmp = Ptr.OfReadOnlyRef(data.Value.Mode).As<byte>() + sizeof(byte);
			CommandLine = tmp.As<char>().AsSpan((int) (userDataLength - (tmp - userData))).GetString();
			tmp += CommandLine.Length + sizeof(char);
			ComGuid = new Guid(tmp.AsSpan(16));

			tmp += 16;
			DllPath = tmp.As<char>().AsSpan((int)(userDataLength - (tmp - userData))).GetString();
		}
	}
}