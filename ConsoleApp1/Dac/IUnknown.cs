using System;
using System.Runtime.InteropServices;
using ConsoleApp1.Common;

// ReSharper disable UnusedMember.Global

namespace ConsoleApp1.ClrTracer.Com
{
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000000-0000-0000-C000-000000000046")]
	internal interface IUnknown
	{
		int AddRef(Ptr self);

		int Release(Ptr self);

		int QueryInterface(Ptr self, in Guid guid, out Ptr ptr);
	}
}