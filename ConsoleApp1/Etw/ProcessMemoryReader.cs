// using System.Runtime.CompilerServices;
//
// namespace ConsoleApp1.ClrTracer
// {
// 	public readonly struct ProcessMemoryReader
// 	{
// 		private readonly Ptr _processHandle;
//
// 		public ProcessMemoryReader(Ptr processHandle)
// 		{
// 			_processHandle = processHandle;
// 		}
//
// 		public bool Read<T>(Ptr address, out T t)
// 			where T:unmanaged
// 		{
// 			t = default;
// 			return NativeMethods.ReadProcessMemory(_processHandle, address, Ptr.OfRef(ref t), Unsafe.SizeOf<T>(), out _);
// 		}
//
// 		public bool Read(Ptr address, Ptr buffer, int count, out int read)
// 		{
// 			return NativeMethods.ReadProcessMemory(_processHandle, address, buffer, count, out read);
// 		}
// 	}
// }