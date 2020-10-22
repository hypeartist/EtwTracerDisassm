using System.Runtime.InteropServices;
using ConsoleApp1.Common;

// ReSharper disable UnusedMember.Global

namespace ConsoleApp1.ClrTracer.Com
{
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("5c552ab6-fc09-4cb3-8e36-22fa03c798b7")]
	internal interface IXCLRDataProcess
	{
		void Flush();
		void StartEnumTasks();
		void EnumTask();
		void EndEnumTasks();
		[PreserveSig]
		int GetTaskByOSThreadID(uint id, [Out, MarshalAs(UnmanagedType.IUnknown)] out object task);
		void GetTaskByUniqueID( /*[in] ULONG64 taskID, [out] IXCLRDataTask** task*/);
		void GetFlags( /*[out] ULONG32* flags*/);
		void IsSameObject( /*[in] IXCLRDataProcess* process*/);
		void GetManagedObject( /*[out] IXCLRDataValue** value*/);
		void GetDesiredExecutionState( /*[out] ULONG32* state*/);
		void SetDesiredExecutionState( /*[in] ULONG32 state*/);
		void GetAddressType( /*[in] CLRDATA_ADDRESS address, [out] CLRDataAddressType* type*/);
		void GetRuntimeNameByAddress( /*[in] CLRDATA_ADDRESS address, [in] ULONG32 flags, [in] ULONG32 bufLen, [out] ULONG32 *nameLen, [out, size_is(bufLen)] WCHAR nameBuf[], [out] CLRDATA_ADDRESS* displacement*/);
		void StartEnumAppDomains( /*[out] CLRDATA_ENUM* handle*/);
		void EnumAppDomain( /*[in, out] CLRDATA_ENUM* handle, [out] IXCLRDataAppDomain** appDomain*/);
		void EndEnumAppDomains( /*[in] CLRDATA_ENUM handle*/);
		void GetAppDomainByUniqueID( /*[in] ULONG64 id, [out] IXCLRDataAppDomain** appDomain*/);
		void StartEnumAssemblies( /*[out] CLRDATA_ENUM* handle*/);
		void EnumAssembly( /*[in, out] CLRDATA_ENUM* handle, [out] IXCLRDataAssembly **assembly*/);
		void EndEnumAssemblies( /*[in] CLRDATA_ENUM handle*/);
		void StartEnumModules( /*[out] CLRDATA_ENUM* handle*/);
		void EnumModule( /*[in, out] CLRDATA_ENUM* handle, [out] IXCLRDataModule **mod*/);
		void EndEnumModules( /*[in] CLRDATA_ENUM handle*/);
		void GetModuleByAddress( /*[in] CLRDATA_ADDRESS address, [out] IXCLRDataModule** mod*/);
		[PreserveSig]
		int StartEnumMethodInstancesByAddress(ulong address, [In, MarshalAs(UnmanagedType.Interface)] object appDomain, out ulong handle);
		[PreserveSig]
		int EnumMethodInstanceByAddress(ref ulong handle, [Out, MarshalAs(UnmanagedType.Interface)] out object method);
		[PreserveSig]
		int EndEnumMethodInstancesByAddress(ulong handle);
		void GetDataByAddress( /*[in] CLRDATA_ADDRESS address, [in] ULONG32 flags, [in] IXCLRDataAppDomain* appDomain, [in] IXCLRDataTask* tlsTask, [in] ULONG32 bufLen, [out] ULONG32 *nameLen, [out, size_is(bufLen)] WCHAR nameBuf[], [out] IXCLRDataValue** value, [out] CLRDATA_ADDRESS* displacement*/);
		void GetExceptionStateByExceptionRecord( /*[in] EXCEPTION_RECORD64* record, [out] IXCLRDataExceptionState **exState*/);
		void TranslateExceptionRecordToNotification();
		[PreserveSig]
		int Request(uint reqCode, uint inBufferSize, Ptr/*[In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[]*/ inBuffer, uint outBufferSize, Ptr/* [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[]*/ outBuffer);
	}
}