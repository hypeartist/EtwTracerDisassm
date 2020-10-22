using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using ConsoleApp1.ClrData;
using ConsoleApp1.Disassembler;
using ConsoleApp1.Disassembler.Platform.Win32;
using ConsoleApp1.Etw;
using ConsoleApp1.Etw.Packets;

namespace ConsoleApp1
{
	[SuppressMessage("ReSharper", "PossibleNullReferenceException")] 
	unsafe class Program
	{
		private const string OutputFile = @"d:\\ClrTracer_dump.txt";

		private static DacDistpatcher _dacDistpatcher;
		private static SymbolHelper _symbolHelper;
		private static NativeCodeDisassembler _disassembler;
		private static readonly StringBuilder DisassemblyOutput = new StringBuilder(256);

		static void Main(string[] args)
		{
			var tracer = new Tracer("ClrTracer");
			tracer.RuntimeInformationStart += (ref RuntimeInformationStartPacket p) =>
			{
				if(!DacDistpatcher.Create(p.Process, out _dacDistpatcher) || !SymbolHelper.Create(p.Process, NativeEnums.SymbolOptions.DeferredLoads | NativeEnums.SymbolOptions.UndecorateNames, @"d:\symbols", @"d:\symbols", out _symbolHelper)) return;
				_disassembler = new NativeCodeDisassembler(_dacDistpatcher, _symbolHelper, p.Process);
				if (File.Exists(OutputFile))
				{
					File.Delete(OutputFile);
				}
			};
			tracer.MethodLoadVerbose += (ref MethodLoadVerbosePacket p) =>
			{
				if(_disassembler == null) return;
				DisassemblyOutput.Clear();
				_disassembler.DisassembleMethod(p, DisassemblyOutput);
				File.AppendAllText(OutputFile, DisassemblyOutput.ToString());
			};
			// tracer.MethodJitInliningFailed += (ref MethodJitInliningFailedPacket p) =>
			// {
			// 	Console.WriteLine($"PID: {p.ProcessId}, Method: {p.MethodBeingCompiledNamespace.ToString()}::{p.MethodBeingCompiledName.ToString()}, Reason: {p.FailReason.ToString()}");
			// };
			// tracer.MethodJitInliningSucceeded += (ref MethodJitInliningSucceededPacket p) =>
			// {
			// 	ref var pp = ref p;
			// };
			// tracer.MethodILToNativeMap += (ref MethodILToNativeMapPacket p) =>
			// {
			// 	var ssss = 0;
			// };
			// tracer.MethodJittingStarted += (ref MethodJittingStartedPacket p) =>
			// {
			// 	Debug.WriteLine($"{new string(p.MethodNamespace)}::{new string(p.MethodName)}");
			// };
			tracer.StartSession(@"d:\Repos\AggLibSharp\AggLibSharp.Samples.WinForms\bin\Release\net5.0\AggLibSharp.Samples.WinForms.exe");
			// tracer.StartSession(@"d:\Tools\dnSpy\dnSpy.exe");
			Console.ReadKey();
		}
	}
}