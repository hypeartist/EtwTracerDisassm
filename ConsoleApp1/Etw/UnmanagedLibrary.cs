using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.Unicode;
using System.Threading;

namespace ConsoleApp1.ClrTracer
{
	public unsafe sealed class UnmanagedLibrary
	{
		private readonly void* _code;

		public enum Status
		{
			Ok,
			FailedToAllocateMemory,
			InvalidDosSignature,
			InvalidNtSignature,
			NotSupportedImageType,
			MachineTypeMismatch,
			InvalidSectionAlignment,
			InvalidImportTable,
			FailedToLoadLibrary,
			FailedToGetFunctionAddress,
			FailedToApplySectionAttributes
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		[SuppressUnmanagedCodeSecurity]
		private delegate int DllMainD(IntPtr hModule, int reason, IntPtr lpReserved);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		[SuppressUnmanagedCodeSecurity]
		private delegate IntPtr LoadLibraryWD(IntPtr p);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		[SuppressUnmanagedCodeSecurity]
		private delegate IntPtr LoadLibraryAD(IntPtr p);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		[SuppressUnmanagedCodeSecurity]
		private delegate IntPtr VirtualAllocD(IntPtr p, ulong p1, AllocationType p2, MemoryProtection p3);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		[SuppressUnmanagedCodeSecurity]
		private delegate bool VirtualFreeD(IntPtr p, ulong p1, AllocationType p2);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		[SuppressUnmanagedCodeSecurity]
		private delegate bool VirtualProtectD(IntPtr p, ulong p1, PageProtection p2, IntPtr p3);

		private static readonly DllMainD DllMain;
		private static readonly LoadLibraryWD LoadLibraryW;
		private static readonly LoadLibraryAD LoadLibraryA;
		private static readonly VirtualAllocD VirtualAlloc;
		private static readonly VirtualFreeD VirtualFree;
		private static readonly VirtualProtectD VirtualProtect;
		// private static readonly UnsafeDelegates.Stdcall.Func<IntPtr, UnsafePointer> LoadLibraryW;
		// private static readonly UnsafeDelegates.Stdcall.Func<UnsafePointer, UnsafePointer> LoadLibraryA;
		// private static readonly UnsafeDelegates.Stdcall.Func<UnsafePointer, ulong, AllocationType, MemoryProtection, UnsafePointer> VirtualAlloc;
		// private static readonly UnsafeDelegates.Stdcall.Func<UnsafePointer, ulong, AllocationType, bool> VirtualFree;
		// private static readonly UnsafeDelegates.Stdcall.Func<UnsafePointer, ulong, PageProtection, UnsafePointer<PageProtection>, bool> VirtualProtect;

		static UnmanagedLibrary()
        {
			void* kernel32;
			var threadAccessor = new ThreadAccessor(Thread.CurrentThread);
			var currentThreadHandle = (void*)(threadAccessor.ThreadProxy.ThreadHandle);
			if (Environment.Is64BitProcess)
            {
                var stackEndAddress = (ulong*) *((ulong*) currentThreadHandle + 34);
                while ((*(--stackEndAddress) & 0x000000000000ffff) != 0x0000000000007bd4)
                {
                }

                var baseThreadInitThunk = *stackEndAddress;//kernel32.dll!BaseThreadInitThunk
                kernel32 = (void*) ((baseThreadInitThunk & 0xffffffffffff0000) - 0x10000);
            }
            else
			{
				var stackEndAddress = (uint*) *((uint*) currentThreadHandle + 49);
                while ((*(--stackEndAddress) & 0x0000ffff) != 0x00006340)
				{
                }

                var baseThreadInitThunk = *stackEndAddress;//kernel32.dll!BaseThreadInitThunk
                kernel32 = (void*) ((baseThreadInitThunk & 0xffff0000) - 0x10000);
			}

			LoadLibraryW = Marshal.GetDelegateForFunctionPointer<LoadLibraryWD>((IntPtr) GetProcAddress(kernel32, (char*) Marshal.StringToHGlobalUni(nameof(LoadLibraryW)), nameof(LoadLibraryW).Length));
			LoadLibraryA = Marshal.GetDelegateForFunctionPointer<LoadLibraryAD>((IntPtr) GetProcAddress(kernel32, (char*) Marshal.StringToHGlobalUni(nameof(LoadLibraryA)), nameof(LoadLibraryA).Length));
			VirtualAlloc = Marshal.GetDelegateForFunctionPointer<VirtualAllocD>((IntPtr) GetProcAddress(kernel32, (char*) Marshal.StringToHGlobalUni(nameof(VirtualAlloc)), nameof(VirtualAlloc).Length));
			VirtualFree = Marshal.GetDelegateForFunctionPointer<VirtualFreeD>((IntPtr) GetProcAddress(kernel32, (char*) Marshal.StringToHGlobalUni(nameof(VirtualFree)), nameof(VirtualFree).Length));
			VirtualProtect = Marshal.GetDelegateForFunctionPointer<VirtualProtectD>((IntPtr) GetProcAddress(kernel32, (char*) Marshal.StringToHGlobalUni(nameof(VirtualProtect)), nameof(VirtualProtect).Length));

			// LoadLibraryW = new UnsafeDelegates.Stdcall.Func<IntPtr, UnsafePointer>(GetProcAddress(kernel32, UnsafeMemoryBlock<char>.Of(nameof(LoadLibraryW))));
            // LoadLibraryA = new UnsafeDelegates.Stdcall.Func<UnsafePointer, UnsafePointer>(GetProcAddress(kernel32, UnsafeMemoryBlock<char>.Of(nameof(LoadLibraryA))));
            // VirtualAlloc = new UnsafeDelegates.Stdcall.Func<UnsafePointer, ulong, AllocationType, MemoryProtection, UnsafePointer>(GetProcAddress(kernel32, UnsafeMemoryBlock<char>.Of(nameof(VirtualAlloc))));
            // VirtualFree = new UnsafeDelegates.Stdcall.Func<UnsafePointer, ulong, AllocationType, bool>(GetProcAddress(kernel32, UnsafeMemoryBlock<char>.Of(nameof(VirtualFree))));
            // VirtualProtect = new UnsafeDelegates.Stdcall.Func<UnsafePointer, ulong, PageProtection, UnsafePointer<PageProtection>, bool>(GetProcAddress(kernel32, UnsafeMemoryBlock<char>.Of(nameof(VirtualProtect))));

			// var kernel32Name = UnsafeReferenceBlock<char>.Of("KERNEL32.DLL");
			//
			// var modules = Process.GetCurrentProcess().Modules;
			//
			// for (var i = 0; i < modules.Count; i++)
			// {
			// 	var module = modules[i];
			// 	var moduleName = (UnsafeReferenceBlock<char>)module.ModuleName.AsSpan();
			// 	if (!((ReadOnlySpan<byte>)UnsafeReferenceBlock<byte>.Of(moduleName.Pointer.ReinterpretAs<byte>(), moduleName.Length * sizeof(char))).SequenceEqual(UnsafeReferenceBlock<byte>.Of(kernel32Name.Pointer.ReinterpretAs<byte>(), kernel32Name.Length * sizeof(char)))) continue;
			//
			// 	var kernel32 = module.BaseAddress;
			//
			// 	LoadLibraryW = new UnsafeDelegates.Stdcall.Func<IntPtr, UnsafePointer>(GetProcAddress(kernel32, UnsafeReferenceBlock<char>.Of(nameof(LoadLibraryW))));
			// 	LoadLibraryA = new UnsafeDelegates.Stdcall.Func<UnsafePointer, UnsafePointer>(GetProcAddress(kernel32, UnsafeReferenceBlock<char>.Of(nameof(LoadLibraryA))));
			// 	VirtualAlloc = new UnsafeDelegates.Stdcall.Func<UnsafePointer, ulong, AllocationType, MemoryProtection, UnsafePointer>(GetProcAddress(kernel32, UnsafeReferenceBlock<char>.Of(nameof(VirtualAlloc))));
			// 	VirtualFree = new UnsafeDelegates.Stdcall.Func<UnsafePointer, ulong, AllocationType, bool>(GetProcAddress(kernel32, UnsafeReferenceBlock<char>.Of(nameof(VirtualFree))));
			// 	VirtualProtect = new UnsafeDelegates.Stdcall.Func<UnsafePointer, ulong, PageProtection, UnsafePointer<PageProtection>, bool>(GetProcAddress(kernel32, UnsafeReferenceBlock<char>.Of(nameof(VirtualProtect))));
			//
			// 	break;
			// }
		}

		private UnmanagedLibrary(void* code)
		{
			_code = code;
		}

		public IntPtr GetProcAddress(ReadOnlySpan<char> procName)
		{
			return (IntPtr) GetProcAddress(_code, (char*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(procName)), procName.Length);
		}

        public static (Status status, UnmanagedLibrary library) Load(Stream src)
		{
			Status status;

			ImageNtHeaders ntHeaders;

			if (((status, ntHeaders) = ValidateHeaders(src)).status != Status.Ok)
			{
				return (status, null);
			}

			if ((ntHeaders.FileHeader.Characteristics & 0x2000 /*IMAGE_FILE_DLL*/) == 0)
			{
				return (Status.NotSupportedImageType, null);
			}

			if ((status = ValidateSectionAlignment(src, ntHeaders)) != Status.Ok)
			{
				return (status, null);
			}

			IntPtr code;
			if (((status, code) = AllocateImageMemory(ntHeaders)).status != Status.Ok)
			{
				return (status, null);
			}
			
			MapHeaders(src, (byte*) code, ntHeaders);
			MapSections(src, (byte*)code);
			RelocateImage((byte*)code, ntHeaders);

			if ((status = ProcessImportTable((byte*) code, ntHeaders)) != Status.Ok)
			{
				VirtualFree(code, ntHeaders.OptionalHeader.SizeOfImage, AllocationType.Release);
				return (status, null);
			}

			if ((status = ApplySectionsAttributes((byte*)code)) != Status.Ok)
			{
				VirtualFree.Invoke(code, ntHeaders.OptionalHeader.SizeOfImage, AllocationType.Release);
				return (status, null);
			}

			ExecuteTlsCallbacks((byte*)code);

			var dllMain = Marshal.GetDelegateForFunctionPointer<DllMainD>((IntPtr) ((byte*) code + ntHeaders.OptionalHeader.AddressOfEntryPoint));
			var ret = dllMain(code, 1 /*DLL_PROCESS_ATTACH*/, IntPtr.Zero);
			
			return (Status.Ok, new UnmanagedLibrary((void*) code));
		}

		private static (Status status, ImageNtHeaders ntHeaders) ValidateHeaders(Stream src)
		{
			var dosHeader = src.Read<ImageDosHeader>();
			if (dosHeader.Magic != Signatures.Dos)
			{
				return (Status.InvalidDosSignature, default);
			}

			var ntHeaders = src.Read<ImageNtHeaders>(dosHeader.LfaNew);
			if (ntHeaders.Signature != Signatures.Nt)
			{
				return (Status.InvalidNtSignature, default);
			}

			return ntHeaders.FileHeader.Machine != (Environment.Is64BitProcess ? MachineType.Amd64 : MachineType.I386) ? (Status.MachineTypeMismatch, default) : (Status.Ok, ntHeaders);
		}

		private static Status ValidateSectionAlignment(Stream src, ImageNtHeaders ntHeaders)
		{
			if ((ntHeaders.OptionalHeader.SectionAlignment & 1) > 0)
			{
				return Status.InvalidSectionAlignment;
			}

			var sectionOffset = GetStreamOffsetOfFirstSection(src, &ntHeaders);
			var lastSectionEnd = 0u;
			var sectionSize = ntHeaders.OptionalHeader.SectionAlignment;

			for (var i = 0; i < ntHeaders.FileHeader.NumberOfSections; i++)
			{
				var sectionHeader = src.Read<ImageSectionHeader>((int)sectionOffset);
				uint endOfSection;

				if (sectionHeader.SizeOfRawData == 0)       
				{
					endOfSection = sectionHeader.VirtualAddress + sectionSize;
				}
				else
				{
					endOfSection = sectionHeader.VirtualAddress + sectionHeader.SizeOfRawData;
				}

				if (endOfSection > lastSectionEnd)
				{
					lastSectionEnd = endOfSection;
				}

				sectionOffset += Unsafe.SizeOf<ImageSectionHeader>();
			}

			var systemPageSize = (ulong)Environment.SystemPageSize;
			var alignedImageSize = AlignValueUp(ntHeaders.OptionalHeader.SizeOfImage, systemPageSize);

			return alignedImageSize != AlignValueUp(lastSectionEnd, systemPageSize) ? Status.InvalidSectionAlignment : Status.Ok;
		}

		private static (Status status, IntPtr code) AllocateImageMemory(ImageNtHeaders ntHeaders)
		{
			var code = VirtualAlloc((IntPtr) (Environment.Is64BitProcess ? ntHeaders.OptionalHeader.ImageBase64 : ntHeaders.OptionalHeader.ImageBase32), ntHeaders.OptionalHeader.SizeOfImage, AllocationType.Reserve | AllocationType.Commit, MemoryProtection.ReadWrite);
			if (code == IntPtr.Zero)
			{
				code = VirtualAlloc(IntPtr.Zero, ntHeaders.OptionalHeader.SizeOfImage, AllocationType.Reserve | AllocationType.Commit, MemoryProtection.ReadWrite);
			}

			return code == IntPtr.Zero ? (Status.FailedToAllocateMemory, IntPtr.Zero) : (Status.Ok, code);
		}

		private static void MapHeaders(Stream src, byte* code, ImageNtHeaders ntHeaders)
		{
			src.Position = 0;
			src.Read(MemoryMarshal.CreateSpan(ref *code, (int) ntHeaders.OptionalHeader.SizeOfHeaders));

			var ntHeadersPtr = (ImageNtHeaders*)(code + (*(ImageDosHeader*)code).LfaNew);

			if (Environment.Is64BitProcess)
			{
				ntHeadersPtr->OptionalHeader.ImageBase64 = (ulong) code;
			}
			else
			{
				ntHeadersPtr->OptionalHeader.ImageBase32 = (uint) code;
			}
		}

		private static void MapSections(Stream src, byte* code)
		{
			var ntHeadersPtr = (ImageNtHeaders*)(code + (*(ImageDosHeader*)code).LfaNew);
			var sectionHeaderPtr = GetFirstSectionPointer(ntHeadersPtr);

			for (var i = 0; i < ntHeadersPtr->FileHeader.NumberOfSections; i++)
			{
				byte* dest;

				if (sectionHeaderPtr->SizeOfRawData == 0)
				{
					var size = ntHeadersPtr->OptionalHeader.SectionAlignment;
					if (size > 0)
					{
						dest = code + sectionHeaderPtr->VirtualAddress;

						sectionHeaderPtr->PhysicalAddress = (uint)dest & 0xffffffff;
						Unsafe.InitBlock(dest, 0, size);
						// dest.Fill(0, (int)size);
					}
					continue;
				}

				dest = code + sectionHeaderPtr->VirtualAddress;
				src.Position = sectionHeaderPtr->PointerToRawData;
				src.Read(MemoryMarshal.CreateSpan(ref *dest, (int)sectionHeaderPtr->SizeOfRawData));

				sectionHeaderPtr->PhysicalAddress = (uint)dest & 0xffffffff;

				sectionHeaderPtr++;
			}
		}

		private static void RelocateImage(byte* code, ImageNtHeaders oldNtHeaders)
		{
			var ntHeaders = *(ImageNtHeaders*)(code + (*(ImageDosHeader*)code).LfaNew);
			var locationDelta = (Environment.Is64BitProcess ? ntHeaders.OptionalHeader.ImageBase64 : ntHeaders.OptionalHeader.ImageBase32) - (Environment.Is64BitProcess ? oldNtHeaders.OptionalHeader.ImageBase64 : oldNtHeaders.OptionalHeader.ImageBase32);

			if (locationDelta == 0) return;

			var imageSizeOfBaseRelocation = Unsafe.SizeOf<ImageBaseRelocation>();
			var relocationTable = Environment.Is64BitProcess ? ntHeaders.OptionalHeader.BaseRelocationTable64 : ntHeaders.OptionalHeader.BaseRelocationTable32;

			if (relocationTable.Size > 0)
			{
				var relocation = (ImageBaseRelocation*)(code + relocationTable.VirtualAddress);
				while (relocation->VirtualAdress > 0)
				{
					var dest = code + relocation->VirtualAdress;
					var relInfo = (ushort*)((byte*)relocation + imageSizeOfBaseRelocation);

					for (var i = 0; i < (relocation->SizeOfBlock - imageSizeOfBaseRelocation) / 2; i++, relInfo++)
					{
						var type = (BasedRelocationType)(*relInfo >> 12);
						var offset = *relInfo & 0xfff;

						switch (type)
						{
							case BasedRelocationType.Absolute:
								break;
							case BasedRelocationType.HighLow:
								var patchAddrHighLow = (uint*)(dest + offset);
								*patchAddrHighLow += (uint)locationDelta;
								break;
							case BasedRelocationType.Dir64:
								var patchAddr64 = (ulong*)(dest + offset);
								*patchAddr64 += locationDelta;
								break;
						}
					}
					relocation = (ImageBaseRelocation*)((byte*)relocation + relocation->SizeOfBlock);
				}
			}
		}

		private static Status ProcessImportTable(byte* code, ImageNtHeaders ntHeaders)
		{
			var directory = Environment.Is64BitProcess ? ntHeaders.OptionalHeader.ImportTable64 : ntHeaders.OptionalHeader.ImportTable32;

			if (directory.Size == 0)
			{
				return Status.InvalidImportTable;
			}

			var importDesc = (ImageImportDescriptor*)(code + directory.VirtualAddress);

			if (Environment.Is64BitProcess)
			{
				for (; importDesc != null && importDesc->Name > 0; importDesc++)
				{
					ulong* thunkRef;
					long* funcRef;

					var handle = LoadLibraryA((IntPtr) (code + importDesc->Name));

					if (handle == IntPtr.Zero)
					{
						return Status.FailedToLoadLibrary;
					}

					if (importDesc->OriginalFirstThunk > 0)
					{
						thunkRef = (ulong*)(code + importDesc->OriginalFirstThunk);
						funcRef = (long*)(code + importDesc->FirstThunk);
					}
					else
					{
						thunkRef = (ulong*) (code + importDesc->FirstThunk);
						funcRef = (long*) (code + importDesc->FirstThunk);
					}
					for (; *thunkRef > 0; thunkRef++, funcRef++)
					{
						if (ImageSnapByOrdinal64(*thunkRef))
						{
							var ptr = ImageOrdianl64(*thunkRef);
							*(ulong*) funcRef = (ulong) GetProcAddress((void*)handle, (byte*)ptr, GetAnsiString((byte*) ptr));
						}
						else
						{
							var thunkData = (ImageImportByName*) (code + *thunkRef);
							*(ulong*)funcRef = (ulong) GetProcAddress((void*)handle, (byte*) Unsafe.AsPointer(ref *thunkData->Name), GetAnsiString(thunkData->Name));
						}

						if (*funcRef == 0)
						{
							return Status.FailedToGetFunctionAddress;
						}
					}
				}
			}
			else
			{
				for (; importDesc != null && importDesc->Name > 0; importDesc++)
				{
					uint* thunkRef;
					int* funcRef;

					var handle = LoadLibraryA((IntPtr) (code + importDesc->Name));

					if (handle == IntPtr.Zero)
					{
						return Status.FailedToLoadLibrary;
					}

					if (importDesc->OriginalFirstThunk > 0)
					{
						thunkRef = (uint*) (code + importDesc->OriginalFirstThunk);
						funcRef = (int*) (code + importDesc->FirstThunk);
					}
					else
					{
						thunkRef = (uint*) (code + importDesc->FirstThunk);
						funcRef = (int*) (code + importDesc->FirstThunk);
					}
					for (; *thunkRef > 0; thunkRef++, funcRef++)
					{
						if (ImageSnapByOrdinal32(*thunkRef))
						{
							var ptr = ImageOrdianl32(*thunkRef);
							*(uint*) funcRef = (uint) GetProcAddress((void*)handle, (byte*) ptr, GetAnsiString((byte*) ptr));
						}
						else
						{
							var thunkData = (ImageImportByName*)(code + *thunkRef);
							*(uint*)funcRef = (uint) GetProcAddress((void*)handle, (byte*)Unsafe.AsPointer(ref *thunkData->Name), GetAnsiString(thunkData->Name));
						}

						if (*funcRef == 0)
						{
							return Status.FailedToGetFunctionAddress;
						}
					}
				}
			}

			return Status.Ok;
		}

		private static Status ApplySectionsAttributes(byte* code)
		{
			var ntHeadersPtr =(ImageNtHeaders*)(code + (*(ImageDosHeader*)code).LfaNew);

			var sectionHeaderPtr = GetFirstSectionPointer(ntHeadersPtr);
			var imageOffset = (Environment.Is64BitProcess ? ntHeadersPtr->OptionalHeader.ImageBase64 : ntHeadersPtr->OptionalHeader.ImageBase32) & 0xffffffff00000000;

			var sectionData = new SectionFinalizeData
			{
				Address = (void*) (sectionHeaderPtr->PhysicalAddress | imageOffset),
				Size = GetRealSectionSize(sectionHeaderPtr, *ntHeadersPtr),
				Characteristics = sectionHeaderPtr->Characteristics,
				Last = false
			};
			sectionData.AlignedAddress = AlignAddressDown(sectionData.Address, (ulong)Environment.SystemPageSize);
			sectionHeaderPtr++;

			// loop through all sections and change access flags
			for (var i = 1; i < ntHeadersPtr->FileHeader.NumberOfSections; i++, sectionHeaderPtr++)
			{
				var sectionAddress = (void*)(sectionHeaderPtr->PhysicalAddress | imageOffset);
				var alignedAddress = AlignAddressDown(sectionAddress, (ulong)Environment.SystemPageSize);
				var sectionSize = (uint)GetRealSectionSize(sectionHeaderPtr, *ntHeadersPtr);
				// Combine access flags of all sections that share a page
				if (sectionData.AlignedAddress == alignedAddress || (byte*)sectionData.Address + sectionData.Size > alignedAddress)
				{
					// Section shares page with previous
					if ((sectionHeaderPtr->Characteristics & Discardable) == 0 || (sectionData.Characteristics & Discardable) == 0)
					{
						sectionData.Characteristics = (sectionData.Characteristics | sectionHeaderPtr->Characteristics) & ~Discardable;
					}
					else
					{
						sectionData.Characteristics |= sectionHeaderPtr->Characteristics;
					}
					sectionData.Size = (ulong) ((byte*)sectionAddress + sectionSize - (byte*) sectionData.Address);
					continue;
				}

				if (!ApplySectionAttributes(ntHeadersPtr, sectionData, (uint)Environment.SystemPageSize))
				{
					return Status.FailedToApplySectionAttributes;
				}

				sectionData.Address = sectionAddress;
				sectionData.AlignedAddress = alignedAddress;
				sectionData.Size = sectionSize;
				sectionData.Characteristics = sectionHeaderPtr->Characteristics;
			}
			sectionData.Last = true;
			return !ApplySectionAttributes(ntHeadersPtr, sectionData, (uint)Environment.SystemPageSize) ? Status.FailedToApplySectionAttributes : Status.Ok;
        }

		private static bool ApplySectionAttributes(ImageNtHeaders* ntHeadersPtr, SectionFinalizeData sectionData, uint pageSize)
		{
			if (sectionData.Size == 0)
			{
				return true;
			}

			if ((sectionData.Characteristics & Discardable) > 0)
			{
				if (sectionData.Address == sectionData.AlignedAddress && (sectionData.Last || ntHeadersPtr->OptionalHeader.SectionAlignment == pageSize || sectionData.Size % pageSize == 0))
				{
					VirtualFree((IntPtr) sectionData.Address, sectionData.Size, AllocationType.Decommit);
				}
				return true;
			}

			var executable = (sectionData.Characteristics & (uint)ImageSectionFlags.Execute) != 0 ? 4 : 0;
			var readable = (sectionData.Characteristics & (uint)ImageSectionFlags.Read) != 0 ? 2 : 0;
			var writeable = (sectionData.Characteristics & (uint)ImageSectionFlags.Write) != 0 ? 1 : 0;
			var protect = ProtectionFlags[executable + readable + writeable];
			if ((sectionData.Characteristics & NotCached) > 0)
			{
				protect |= (PageProtection)0x200;//PAGE_NOCACHE;
			}

			PageProtection oldProtection = default;
			return VirtualProtect((IntPtr) sectionData.Address, sectionData.Size, protect, (IntPtr) (&oldProtection));
		}

		private static void ExecuteTlsCallbacks(byte* code)
		{
			var ntHeaders = *(ImageNtHeaders*)(code + (*(ImageDosHeader*)code).LfaNew);
			var tlsTable = Environment.Is64BitProcess ? ntHeaders.OptionalHeader.TLSTable64 : ntHeaders.OptionalHeader.TLSTable32;
			if (tlsTable.VirtualAddress == 0) return;

			var tlsDir = *(ImageTlsDirectory*) (code + tlsTable.VirtualAddress);
			var callbackPtr = (ulong*)(Environment.Is64BitProcess ? tlsDir.AddressOfCallBacks64 : tlsDir.AddressOfCallBacks32);
			if (callbackPtr == null) return;

			for (; *callbackPtr > 0; callbackPtr++)
			{
                // new UnsafeDelegates.Stdcall.Action<UnsafePointer, uint, UnsafePointer>(callbackPtr).Invoke(callbackPtr, 1 /*DLL_PROCESS_ATTACH*/, null);
			}
		}

		private static ReadOnlySpan<PageProtection> ProtectionFlags => new[]
		{
			PageProtection.NoAccess, PageProtection.WriteCopy,
			PageProtection.ReadOnly, PageProtection.ReadWrite,
			PageProtection.Execute, PageProtection.WriteCopy,
			PageProtection.ExecuteRead, PageProtection.ExecuteReadWrite
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong GetRealSectionSize(ImageSectionHeader* section, ImageNtHeaders ntHeaders)
		{
			var size = section->SizeOfRawData;

			if (size != 0)
			{
				return size;
			}

			if ((section->Characteristics & InitializedData) > 0)
			{
				size = ntHeaders.OptionalHeader.SizeOfInitializedData;
			}
			else if ((section->Characteristics & UninitializedData) > 0)
			{
				size = ntHeaders.OptionalHeader.SizeOfUninitializedData;
			}
			return size;
		}


		private static void* GetProcAddress(void* moduleHandle, char* procName, int procNameLength)
		{
			Span<byte> tmp = stackalloc byte[260/*MAX_PATH_LENGTH*/];
			Utf8.FromUtf16(MemoryMarshal.CreateReadOnlySpan(ref *procName, procNameLength), tmp, out _, out var bw);
			return GetProcAddress(moduleHandle, (byte*) Unsafe.AsPointer(ref tmp.Slice(0, bw)[0]), bw);
		}

		private static void* GetProcAddress(void* moduleHandle, byte* procName, int procNameLength)
		{
			var module = (byte*)moduleHandle;

			var dosHeader = *(ImageDosHeader*)module;
			var optHeader = (*(ImageNtHeaders*) (module + dosHeader.LfaNew)).OptionalHeader;
			var exportTable = (Environment.Is64BitProcess ? optHeader.ExportTable64 : optHeader.ExportTable32);

			var exportDirectoryPtr = (ImageExportDirectory*)(module + exportTable.VirtualAddress);
			var exportDirectory = *exportDirectoryPtr;

			var funcTable = (uint*)(module + exportDirectory.AddressOfFunctions);
			var ordTable = (ushort*)(module + exportDirectory.AddressOfNameOrdinals);
			var nameTable = (uint*) (module + exportDirectory.AddressOfNames);
			void* address = default;

			if ((uint)procName >> 16 == 0)
			{
				var ordinal = unchecked((short)(long)procName);
				if (ordinal < exportDirectory.Base || ordinal > exportDirectory.Base + exportDirectory.NumberOfFunctions)
				{
					return default;
				}
				address = module + (ushort)(int)funcTable[(int)(ordinal - exportDirectory.Base)];
			}
			else
			{
				var pns = MemoryMarshal.CreateReadOnlySpan(ref *procName, procNameLength);
				for (var i = 0; i < exportDirectory.NumberOfNames; i++)
				{
					var namePtr = module + nameTable[i];
					var nameLength = GetAnsiString(namePtr);
					var fns = MemoryMarshal.CreateReadOnlySpan(ref *namePtr, nameLength);
					if (!pns.SequenceEqual(fns)) continue;
					address = module + funcTable[ordTable[i]];
					break;
				}
			}

			if (address < exportDirectoryPtr || address >= exportDirectoryPtr + exportTable.Size)
			{
				return address;
			}

			var dllNameLength = GetAnsiString((byte*) address);
			var dns = MemoryMarshal.CreateSpan(ref *(byte*)address, dllNameLength);
			var splitAt = dns.IndexOf((byte)'.');
			var forwardedDllName = dns.Slice(0, splitAt);
			Span<byte> tmp = stackalloc byte[dns.Length + 4];
			Unsafe.CopyBlock(ref tmp[0], ref forwardedDllName[0], (uint) forwardedDllName.Length);
            // forwardedDllName.Pointer.CopyTo(tmp.Pointer, forwardedDllName.Length);
			tmp[^4] = (byte)'.';
			tmp[^3] = (byte)'d';
			tmp[^2] = (byte)'l';
			tmp[^1] = (byte)'l';
			var forwardedModule = LoadLibraryA((IntPtr) Unsafe.AsPointer(ref tmp[0]));
			if (forwardedModule != IntPtr.Zero)
			{
				address = GetProcAddress((void*)forwardedModule, (byte*)address + splitAt + 1, dllNameLength);
			}

			return address;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetAnsiString(byte* pointer)
		{
			var start = pointer;
			while (*pointer++ != 0)
			{
			}
			return (int)(pointer - start - 1);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong ImageOrdianl32(uint ordianl) => ordianl & 0xffff;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong ImageOrdianl64(ulong ordianl) => ordianl & 0xffff;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ImageSnapByOrdinal32(uint ordinal) => (ordinal & 0x80000000) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ImageSnapByOrdinal64(ulong ordinal) => (ordinal & 0x8000000000000000UL) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong AlignValueUp(ulong value, ulong alignment) => (value + alignment - 1) & ~(alignment - 1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong AlignValueDown(ulong value, ulong alignment) => value & ~(alignment - 1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void* AlignAddressDown(void* address, ulong alignment) => (void*) AlignValueDown((ulong) address, alignment);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetStreamOffsetOfFirstSection(Stream src, ImageNtHeaders* ntHeaders)
		{
			src.Position = 0;
			return src.Read<ImageDosHeader>().LfaNew + ((byte*)&ntHeaders->OptionalHeader - (byte*)ntHeaders) + ntHeaders->FileHeader.SizeOfOptionalHeader;
			// return src.Read<ImageDosHeader>().LfaNew + (int)(UnsafePointer<ImageOptionalHeader>.Of(UnsafePointer<ImageNtHeaders>.Of(ntHeaders).Value.OptionalHeader).ReinterpretAs<byte>() - UnsafePointer<ImageNtHeaders>.Of(ntHeaders).ReinterpretAs<byte>() + ntHeaders.FileHeader.SizeOfOptionalHeader);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ImageSectionHeader* GetFirstSectionPointer(ImageNtHeaders* ntHeaders)
		{
			return (ImageSectionHeader*) ((byte*) ntHeaders + ((byte*) &ntHeaders->OptionalHeader - (byte*) ntHeaders + ntHeaders->FileHeader.SizeOfOptionalHeader));

			// return ((byte*)ntHeaders + (int)(UnsafePointer<ImageOptionalHeader>.Of(UnsafePointer<ImageNtHeaders>.Of(ntHeaders.Value).Value.OptionalHeader).ReinterpretAs<byte>() - UnsafePointer<ImageNtHeaders>.Of(ntHeaders.Value).ReinterpretAs<byte>() + ntHeaders.Value.FileHeader.SizeOfOptionalHeader)).ReinterpretAs<ImageSectionHeader>();
		}

		[StructLayout(LayoutKind.Explicit, Size = 0x40)]
		private readonly struct ImageDosHeader
		{
			[FieldOffset(0x00)]
			public readonly ushort Magic;
			[FieldOffset(0x3c)]
			public readonly int LfaNew;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct ImageNtHeaders
		{
			[FieldOffset(0x00)]
			public readonly uint Signature;
			[FieldOffset(0x04)]
			public readonly ImageFileHeader FileHeader;
			[FieldOffset(0x18)]
			public ImageOptionalHeader OptionalHeader;
		}

		[StructLayout(LayoutKind.Explicit, Size = 0x14)]
		private readonly struct ImageFileHeader
		{
			[FieldOffset(0x00)]
			public readonly MachineType Machine;
			[FieldOffset(0x02)]
			public readonly ushort NumberOfSections;
			[FieldOffset(0x10)]
			public readonly ushort SizeOfOptionalHeader;
			[FieldOffset(0x12)]
			public readonly ushort Characteristics;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct ImageOptionalHeader
		{
			[FieldOffset(0x08)]
			public readonly uint SizeOfInitializedData;
			[FieldOffset(0x0c)]
			public readonly uint SizeOfUninitializedData;
			[FieldOffset(0x10)]
			public readonly uint AddressOfEntryPoint;
			[FieldOffset(0x1c)]
			public uint ImageBase32;
			[FieldOffset(0x18)]
			public ulong ImageBase64;
			[FieldOffset(0x20)]
			public readonly uint SectionAlignment;
			[FieldOffset(0x38)]
			public readonly uint SizeOfImage;
			[FieldOffset(0x3c)]
			public readonly uint SizeOfHeaders;
			[FieldOffset(0x60)]
			public readonly ImageDataDirectory ExportTable32;
			[FieldOffset(0x70)]
			public readonly ImageDataDirectory ExportTable64;
			[FieldOffset(0x68)]
			public readonly ImageDataDirectory ImportTable32;
			[FieldOffset(0x78)]
			public readonly ImageDataDirectory ImportTable64;
			[FieldOffset(0xa0)]
			public readonly ImageDataDirectory BaseRelocationTable32;
			[FieldOffset(0xb0)]
			public readonly ImageDataDirectory BaseRelocationTable64;
			[FieldOffset(0xd8)]
			public readonly ImageDataDirectory TLSTable32;
			[FieldOffset(0xe8)]
			public readonly ImageDataDirectory TLSTable64;
		}

		[StructLayout(LayoutKind.Explicit, Size = 0x08)]
		private readonly struct ImageDataDirectory
		{
			[FieldOffset(0x00)]
			public readonly uint VirtualAddress;
			[FieldOffset(0x04)]
			public readonly uint Size;
		}

		[StructLayout(LayoutKind.Explicit, Size = 0x28)]
		private struct ImageSectionHeader
		{
			[FieldOffset(0x08)]
			public uint PhysicalAddress;
			[FieldOffset(0x0c)]
			public readonly uint VirtualAddress;
			[FieldOffset(0x10)]
			public readonly uint SizeOfRawData;
			[FieldOffset(0x14)]
			public readonly uint PointerToRawData;
			[FieldOffset(0x24)]
			public readonly uint Characteristics;
		}

		[StructLayout(LayoutKind.Explicit)]
		private readonly struct ImageImportDescriptor
		{
			[FieldOffset(0x00)]
			public readonly uint OriginalFirstThunk;
			[FieldOffset(0x0c)]
			public readonly uint Name;
			[FieldOffset(0x10)]
			public readonly uint FirstThunk;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct ImageImportByName
		{
			[FieldOffset(0x02)]
			public fixed byte Name[1];
		}

		[StructLayout(LayoutKind.Explicit)]
		private readonly struct ImageExportDirectory
		{
			[FieldOffset(0x10)]
			public readonly uint Base;
			[FieldOffset(0x14)]
			public readonly uint NumberOfFunctions;
			[FieldOffset(0x18)]
			public readonly uint NumberOfNames;
			[FieldOffset(0x1c)]
			public readonly uint AddressOfFunctions;
			[FieldOffset(0x20)]
			public readonly uint AddressOfNames;
			[FieldOffset(0x24)]
			public readonly uint AddressOfNameOrdinals;
		}

		[StructLayout(LayoutKind.Sequential)]
		private readonly struct ImageBaseRelocation
		{
			public readonly uint VirtualAdress;
			public readonly uint SizeOfBlock;
		}

		[StructLayout(LayoutKind.Explicit)]
		private readonly struct ImageTlsDirectory
		{
			[FieldOffset(0x0c)]
			public readonly ulong AddressOfCallBacks32;
			[FieldOffset(0x18)]
			public readonly ulong AddressOfCallBacks64;
		}

		private static class Signatures
		{
			public const ushort Dos = 0x5A4D;
			public const uint Nt = 0x00004550;
		}

		private enum MachineType : ushort
		{
			I386 = 0x014c,
			Amd64 = 0x8664
		}

		private enum BasedRelocationType
		{
			Absolute = 0,
			HighLow = 3,
			Dir64 = 10
		}

		private const uint InitializedData = 0x00000040;
		private const uint UninitializedData = 0x00000080;
		private const uint Discardable = 0x02000000;
		private const uint NotCached = 0x04000000;

		private enum ImageSectionFlags : uint
		{
			Execute = 0x20000000,
			Read = 0x40000000,
			Write = 0x80000000
		}

		private struct SectionFinalizeData
		{
			public void* Address;
			public void* AlignedAddress;
			public ulong Size;
			public uint Characteristics;
			public bool Last;
		}

		[Flags]
		private enum AllocationType : uint
		{
			Commit = 0x1000,
			Reserve = 0x2000,
			Decommit = 0x4000,
			Release = 0x8000
		}

		[Flags]
		private enum MemoryProtection : uint
		{
			ReadWrite = 0x04,
		}

		[Flags]
		private enum PageProtection
		{
			NoAccess = 0x01,
			ReadOnly = 0x02,
			ReadWrite = 0x04,
			WriteCopy = 0x08,
			Execute = 0x10,
			ExecuteRead = 0x20,
			ExecuteReadWrite = 0x40,
		}

        // ReSharper disable once ClassNeverInstantiated.Local
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0044 // Add readonly modifier
        private sealed class ThreadProxy
        {
            private ExecutionContext? _executionContext;
            private SynchronizationContext? _synchronizationContext;

            private string? _name;
            private Delegate? _delegate;
            private object? _threadStartArg;
#pragma warning disable 649
            public/*Sic!*/ IntPtr ThreadHandle;
#pragma warning restore 649
        }
#pragma warning restore IDE0044 // Add readonly modifier

        [StructLayout(LayoutKind.Explicit)]
        private readonly struct ThreadAccessor
        {
            [FieldOffset(0)]
            // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
            private readonly Thread _thread;
            [FieldOffset(0)]
            public readonly ThreadProxy ThreadProxy;

            public ThreadAccessor(Thread thread) : this() => _thread = thread;
        }
#pragma warning restore IDE0051 // Remove unused private members

	}

	internal unsafe static class StreamExtentions
	{
		public static T Read<T>(this Stream s)
			where T : unmanaged
		{
			T t = default;
			s.Read(MemoryMarshal.CreateSpan(ref *(byte*)&t, Unsafe.SizeOf<T>()));
			return t;
		}

		public static T Read<T>(this Stream s, int offset)
			where T : unmanaged
		{
			s.Position = offset;
			T t = default;
			s.Read(MemoryMarshal.CreateSpan(ref *(byte*)&t, Unsafe.SizeOf<T>()));
			return t;
		}
	}
}
