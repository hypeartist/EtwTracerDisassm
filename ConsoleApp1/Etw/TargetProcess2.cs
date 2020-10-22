using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace ConsoleApp1.ClrTracer
{
	internal sealed class TargetProcess2 : IDisposable
	{
		struct DEBUG_PROCESS_INFOS
		{
			public IntPtr ExeBaseAddress;
			public IntPtr ExeEntryPoint;
			public IntPtr ProcessPID;
		}

		internal struct DEBUG_PROCESS
		{
			public IntPtr hDebuggedProc;
			public IntPtr hDebuggedThread;
			public uint DebuggedPID;

		}

		public unsafe static bool Create(ReadOnlySpan<char> targetPath, out DEBUG_PROCESS pPrivateDbgProc)
		{
			const int DEBUG_PROCESS = 0x00000001;
			const int DEBUG_ONLY_THIS_PROCESS = 0x00000002;
			pPrivateDbgProc = default;
			NativeStructs.Startupinfo sInfo = default;
			sInfo.cb = Unsafe.SizeOf<NativeStructs.Startupinfo>();
			// DEBUG_PROCESS* pPrivateDbgProc = (DEBUG_PROCESS*) Marshal.AllocCoTaskMem(Unsafe.SizeOf<DEBUG_PROCESS>());
			if (NativeMethods.CreateProcess((IntPtr) Unsafe.AsPointer(ref Unsafe.AsRef(targetPath[0])), IntPtr.Zero, default, default, false, DEBUG_ONLY_THIS_PROCESS, IntPtr.Zero, null, ref sInfo, out var targetProcessInfo) == 0)
			{
				goto FreezeProcessOnStartup_ERROR;
			}
			NativeMethods.IsWow64Process(targetProcessInfo.hProcess, out var bTargetProcessArch);
			NativeMethods.IsWow64Process(NativeMethods.GetCurrentProcess(), out var bCallerProcessArch);
			if (bTargetProcessArch != bCallerProcessArch)
			{
				goto FreezeProcessOnStartup_ERROR;
			}

			pPrivateDbgProc.hDebuggedProc = targetProcessInfo.hProcess;
			pPrivateDbgProc.hDebuggedThread = targetProcessInfo.hThread;
			pPrivateDbgProc.DebuggedPID = (uint) targetProcessInfo.dwProcessId;

			NativeMethods.DebugSetProcessKillOnExit(true);
			NativeMethods.DebugActiveProcess(targetProcessInfo.dwProcessId);

			const int EXCEPTION_DEBUG_EVENT = 1;
			const int CREATE_THREAD_DEBUG_EVENT = 2;
			const int CREATE_PROCESS_DEBUG_EVENT = 3;
			const int LOAD_DLL_DEBUG_EVENT = 6;
			const int UNLOAD_DLL_DEBUG_EVENT = 7;

			NativeStructs.DEBUG_EVENT debugEvent = default;
			do
			{
				if (NativeMethods.WaitForDebugEvent(ref debugEvent, 10 * 1000 /*INFINITE*/) == 0)
				{
					// Timeout => go on error;
					goto FreezeProcessOnStartup_ERROR;
				}

				// Stop on debug exception
				switch (debugEvent.dwDebugEventCode)
				{
					case EXCEPTION_DEBUG_EVENT:
						goto FreezeProcessOnStartup_SUCCESS;

					case UNLOAD_DLL_DEBUG_EVENT:
						// Allow the x64 system dlls to be unloaded when dealing with a x86 process
						// To be really clean, we need to check if the currently unloaded dll is a 
						// well-known DLL. Unfortunately, there is no easy way to do so.
						if (!bTargetProcessArch)
						{
							goto FreezeProcessOnStartup_ERROR;
						}

						goto case 6;
					case LOAD_DLL_DEBUG_EVENT:
					case CREATE_THREAD_DEBUG_EVENT:
					case CREATE_PROCESS_DEBUG_EVENT:
						NativeMethods.ContinueDebugEvent(debugEvent.dwProcessId, debugEvent.dwThreadId, /*DBG_CONTINUE*/(int) 0x00010002L);
						break;

					default:
						goto FreezeProcessOnStartup_ERROR;
				}

			} while (true);

			// return a null handle on error => the caller need to check it.
			FreezeProcessOnStartup_ERROR:

			StopProcess(pPrivateDbgProc); // mem free is done here
			pPrivateDbgProc = default;

			return false;


			FreezeProcessOnStartup_SUCCESS:
			EnumerateModules(pPrivateDbgProc);
			return true;
		}

		internal static unsafe void EnumerateModules(in DEBUG_PROCESS pPrivateDbgProc)
		{
			var queryHandle = NativeMethods.OpenProcess((int)(NativeConstants.ProcessVmRead | NativeConstants.ProcessQueryInformation), false, (int) pPrivateDbgProc.DebuggedPID);
			NativeMethods.EnumProcessModules(queryHandle, IntPtr.Zero, 0, out var needed);
			// var err = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message;
			var moduleCount = needed / Unsafe.SizeOf<IntPtr>();
			var moduleHandles = stackalloc IntPtr[(int)needed];
			NativeMethods.EnumProcessModules(queryHandle, (IntPtr)moduleHandles, needed, out _);
			var pFileName = new CharBlobMax(NativeConstants.MaxNameSize);
			// var dacFile = new CharBlobMax(NativeConstants.MaxNameSize);
			// for (var i = 0; i < moduleCount; i++)
			// {
			//
			// 	Unsafe.InitBlock(&pFileName, 0, NativeConstants.MaxNameSize);
			// 	NativeMethods.GetModuleFileNameEx(queryHandle, moduleHandles[i], ref pFileName, NativeConstants.MaxNameSize / sizeof(char));
			// 	var fileName = Utils.Trim(pFileName.AsReadOnlySpan());
			// 	var pe = new PortableExecutableParser(fileName.ToString());
			// 	if (!Path.GetFileNameWithoutExtension(fileName).SequenceEqual("coreclr")) continue;
			// 	var coreClrPath = Path.GetDirectoryName(fileName);
			// 	Unsafe.CopyBlock(&dacFile, Unsafe.AsPointer(ref Unsafe.AsRef(coreClrPath[0])), (uint)(coreClrPath.Length * sizeof(char)));
			// 	var dacName = "mscordaccore.dll".AsSpan();
			// 	Unsafe.CopyBlock((byte*)&dacFile + coreClrPath.Length, Unsafe.AsPointer(ref Unsafe.AsRef(dacName[0])), (uint)(dacName.Length * sizeof(char)));
			// 	break;
			// }
		}

		static unsafe bool StopProcess(in DEBUG_PROCESS pPrivateDbgProc)
		{
			// if (pPrivateDbgProc == null)
			// {
				// return false;
			// }

			// Kill target process
			NativeMethods.DebugActiveProcessStop((int) pPrivateDbgProc.DebuggedPID);
			NativeMethods.TerminateProcess(pPrivateDbgProc.hDebuggedProc, 0);

			// Cleanup
			NativeMethods.CloseHandle(pPrivateDbgProc.hDebuggedProc);
			NativeMethods.CloseHandle(pPrivateDbgProc.hDebuggedThread);
			// Marshal.FreeCoTaskMem((IntPtr) pPrivateDbgProc);

			return true;
		}

		public void Dispose()
		{
		}
	}

    public unsafe class PortableExecutableParser
    {
        public delegate void DLog(string fmt, params object[] args);
        private readonly DLog _fnLog;
        private void Log(string fmt, params object[] args)
        {
            if (_fnLog != null)
                _fnLog(fmt, args);
        }

        private readonly List<string> _exports = new List<string>();
        public IEnumerable<string> Exports { get { return _exports; } }

        private readonly List<Tuple<string, List<string>>> _imports = new List<Tuple<string, List<string>>>();
        public IEnumerable<Tuple<string, List<string>>> Imports { get { return _imports; } }


        public PortableExecutableParser(string path, DLog fnLog = null)
        {
            _fnLog = fnLog;
            LOADED_IMAGE loadedImage;

            if (MapAndLoad(path, null, out loadedImage, true, true))
            {
                LoadExports(loadedImage);
                LoadImports(loadedImage);
            }
        }

        private void LoadExports(LOADED_IMAGE loadedImage)
        {
            var hMod = (void*)loadedImage.MappedAddress;

            if (hMod != null)
            {
                Log("Got handle");

                uint size;
                var pExportDir = (IMAGE_EXPORT_DIRECTORY*)ImageDirectoryEntryToData(
                    (void*)loadedImage.MappedAddress,
                    false,
                    IMAGE_DIRECTORY_ENTRY_EXPORT,
                    out size);

                if (pExportDir != null)
                {
                    Log("Got Image Export Descriptor");

                    var pFuncNames = (uint*)RvaToVa(loadedImage, pExportDir->AddressOfNames);

                    for (uint i = 0; i < pExportDir->NumberOfNames; i++)
                    {
                        uint funcNameRva = pFuncNames[i];
                        if (funcNameRva != 0)
                        {
                            var funcName =
                                (char*)RvaToVa(loadedImage, funcNameRva);
                            var name = Marshal.PtrToStringAnsi((IntPtr)funcName);
                            Log("   funcName: {0}", name);
                            _exports.Add(name);
                        }

                    }

                }

            }

        }

        private static IntPtr RvaToVa(LOADED_IMAGE loadedImage, uint rva)
        {
            return ImageRvaToVa(loadedImage.FileHeader, loadedImage.MappedAddress, rva, IntPtr.Zero);
        }
        private static IntPtr RvaToVa(LOADED_IMAGE loadedImage, IntPtr rva)
        {
            return RvaToVa(loadedImage, (uint)(rva.ToInt32()));
        }



        private void LoadImports(LOADED_IMAGE loadedImage)
        {
            var hMod = (void*)loadedImage.MappedAddress;

            if (hMod != null)
            {
                Console.WriteLine("Got handle");

                uint size;
                var pImportDir =
                    (IMAGE_IMPORT_DESCRIPTOR*)
                    ImageDirectoryEntryToData(hMod, false,
                                                        IMAGE_DIRECTORY_ENTRY_IMPORT, out size);
                if (pImportDir != null)
                {
                    Log("Got Image Import Descriptor");
                    while (pImportDir->OriginalFirstThunk != 0)
                    {
                        try
                        {
                            var szName = (char*)RvaToVa(loadedImage, pImportDir->Name);
                            string name = Marshal.PtrToStringAnsi((IntPtr)szName);

                            var pr = new Tuple<string, List<string>>(name, new List<string>());
                            _imports.Add(pr);


                            var pThunkOrg = (THUNK_DATA*)RvaToVa(loadedImage, pImportDir->OriginalFirstThunk);

                            while (pThunkOrg->AddressOfData != IntPtr.Zero)
                            {
                                uint ord;

                                if ((pThunkOrg->Ordinal & 0x80000000) > 0)
                                {
                                    ord = pThunkOrg->Ordinal & 0xffff;
                                    Log("imports ({0}).Ordinal{1} - Address: {2}", name, ord,
                                                        pThunkOrg->Function);
                                }
                                else
                                {
                                    var pImageByName =
                                        (IMAGE_IMPORT_BY_NAME*)RvaToVa(loadedImage, pThunkOrg->AddressOfData);

                                    if (
                                        !IsBadReadPtr(pImageByName, (uint)sizeof(IMAGE_IMPORT_BY_NAME)))
                                    {
                                        ord = pImageByName->Hint;
                                        var szImportName = pImageByName->Name;
                                        string sImportName = Marshal.PtrToStringAnsi((IntPtr)szImportName);
                                        Log("imports ({0}).{1}@{2} - Address: {3}", name,
                                                            sImportName, ord, pThunkOrg->Function);

                                        pr.Item2.Add(sImportName);
                                    }
                                    else
                                    {
                                        Log("Bad ReadPtr Detected or EOF on Imports");
                                        break;
                                    }
                                }

                                pThunkOrg++;
                            }
                        }
                        catch (AccessViolationException e)
                        {
                            Log("An Access violation occured\n" +
                                                "this seems to suggest the end of the imports section\n");
                            Log(e.ToString());
                        }

                        pImportDir++;
                    }

                }

            }
        }


        // ReSharper disable InconsistentNaming
        private const ushort IMAGE_DIRECTORY_ENTRY_IMPORT = 1;
        private const ushort IMAGE_DIRECTORY_ENTRY_EXPORT = 0;

        private const CallingConvention WINAPI = CallingConvention.Winapi;

        private const string KERNEL_DLL = "kernel32";
        private const string DBGHELP_DLL = "Dbghelp";
        private const string IMAGEHLP_DLL = "ImageHlp";
        // ReSharper restore InconsistentNaming

        [DllImport(KERNEL_DLL, CallingConvention = WINAPI, EntryPoint = "GetModuleHandleA"), SuppressUnmanagedCodeSecurity]
        public static extern void* GetModuleHandleA(/*IN*/ char* lpModuleName);

        [DllImport(KERNEL_DLL, CallingConvention = WINAPI, EntryPoint = "GetModuleHandleW"), SuppressUnmanagedCodeSecurity]
        public static extern void* GetModuleHandleW(/*IN*/ char* lpModuleName);

        [DllImport(KERNEL_DLL, CallingConvention = WINAPI, EntryPoint = "IsBadReadPtr"), SuppressUnmanagedCodeSecurity]
        public static extern bool IsBadReadPtr(void* lpBase, uint ucb);

        [DllImport(DBGHELP_DLL, CallingConvention = WINAPI, EntryPoint = "ImageDirectoryEntryToData"), SuppressUnmanagedCodeSecurity]
        public static extern void* ImageDirectoryEntryToData(void* pBase, bool mappedAsImage, ushort directoryEntry, out uint size);

        [DllImport(DBGHELP_DLL, CallingConvention = WINAPI), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr ImageRvaToVa(
            IntPtr pNtHeaders,
            IntPtr pBase,
            uint rva,
            IntPtr pLastRvaSection);

        [DllImport(DBGHELP_DLL, CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr ImageNtHeader(IntPtr pImageBase);

        [DllImport(IMAGEHLP_DLL, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity]
        public static extern bool MapAndLoad(string imageName, string dllPath, out LOADED_IMAGE loadedImage, bool dotDll, bool readOnly);

    }


    // ReSharper disable InconsistentNaming
    [StructLayout(LayoutKind.Sequential)]
    public struct LOADED_IMAGE
    {
        public IntPtr moduleName;
        public IntPtr hFile;
        public IntPtr MappedAddress;
        public IntPtr FileHeader;
        public IntPtr lastRvaSection;
        public UInt32 numbOfSections;
        public IntPtr firstRvaSection;
        public UInt32 charachteristics;
        public ushort systemImage;
        public ushort dosImage;
        public ushort readOnly;
        public ushort version;
        public IntPtr links_1;  // these two comprise the LIST_ENTRY
        public IntPtr links_2;
        public UInt32 sizeOfImage;
    }



    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct IMAGE_IMPORT_BY_NAME
    {
        [FieldOffset(0)]
        public ushort Hint;
        [FieldOffset(2)]
        public fixed char Name[1];
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct IMAGE_IMPORT_DESCRIPTOR
    {
        #region union
        /// <summary>
        /// CSharp doesnt really support unions, but they can be emulated by a field offset 0
        /// </summary>

        [FieldOffset(0)]
        public uint Characteristics;            // 0 for terminating null import descriptor
        [FieldOffset(0)]
        public uint OriginalFirstThunk;         // RVA to original unbound IAT (PIMAGE_THUNK_DATA)
        #endregion

        [FieldOffset(4)]
        public uint TimeDateStamp;
        [FieldOffset(8)]
        public uint ForwarderChain;
        [FieldOffset(12)]
        public uint Name;
        [FieldOffset(16)]
        public uint FirstThunk;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_EXPORT_DIRECTORY
    {
        public UInt32 Characteristics;
        public UInt32 TimeDateStamp;
        public UInt16 MajorVersion;
        public UInt16 MinorVersion;
        public UInt32 Name;
        public UInt32 Base;
        public UInt32 NumberOfFunctions;
        public UInt32 NumberOfNames;
        public IntPtr AddressOfFunctions;     // RVA from base of image
        public IntPtr AddressOfNames;     // RVA from base of image
        public IntPtr AddressOfNameOrdinals;  // RVA from base of image
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct THUNK_DATA
    {
        [FieldOffset(0)]
        public uint ForwarderString;      // PBYTE 
        [FieldOffset(0)]
        public uint Function;             // PDWORD
        [FieldOffset(0)]
        public uint Ordinal;
        [FieldOffset(0)]
        public IntPtr AddressOfData;        // PIMAGE_IMPORT_BY_NAME
    }
    // ReSharper restore InconsistentNaming
}