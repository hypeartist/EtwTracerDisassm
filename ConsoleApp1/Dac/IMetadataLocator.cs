using System.Runtime.InteropServices;
using ConsoleApp1.Common;

namespace ConsoleApp1.ClrTracer.Com
{
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("aa8fa804-bc05-4642-b2c5-c353ed22fc63")]
	internal interface IMetadataLocator
	{
		[PreserveSig]
		int GetMetadata([In][MarshalAs(UnmanagedType.LPWStr)] string imagePath, uint imageTimestamp, uint imageSize, Ptr mvid, uint mdRva, uint flags, uint bufferSize, Ptr/*[Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] byte[]*/ buffer, Ptr ptr);
	}
}