using ConsoleApp1.Etw.Packets;

namespace ConsoleApp1.Etw
{
	public delegate void RuntimeInformationStartCallback(ref RuntimeInformationStartPacket packet);
	public delegate void MethodLoadUnloadVerboseCallback(ref MethodLoadVerbosePacket packet);
	public delegate void MethodJitInliningFailedCallback(ref MethodJitInliningFailedPacket packet);
	public delegate void MethodJitInliningSucceededCallback(ref MethodJitInliningSucceededPacket packet);
	public delegate void MethodJittingStartedCallback(ref MethodJittingStartedPacket packet);
	public delegate void MethodDetailsCallback(ref MethodDetailsPacket packet);
	public delegate void MethodILToNativeMapCallback(ref MethodILToNativeMapPacket packet);
	public delegate void ILStubGeneratedCallback(ref ILStubGeneratedPacket packet);
	public delegate void AppDomainLoadCallback(ref AppDomainLoadPacket packet);
	public delegate void AssemblyLoadStartCallback(ref AssemblyLoadStartPacket packet);
}