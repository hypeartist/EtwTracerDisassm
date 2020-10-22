using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using ConsoleApp1.ClrData;
using ConsoleApp1.Common;
using ConsoleApp1.Disassembler.Platform.Win32;
using ConsoleApp1.Etw.Packets;
using Iced.Intel;
using Decoder = Iced.Intel.Decoder;

namespace ConsoleApp1.Disassembler
{
	internal sealed class NativeCodeDisassembler
	{
		private static ReadOnlySpan<char> Buffer => new char[2048];

		private readonly DacDistpatcher _dacDistpatcher;
		private readonly SymbolHelper _symbolHelper;
		private readonly ProcessWrapper _process;

		private readonly Dictionary<ulong, (string, MethodLoadVerbosePacket.OptimizationTier)> _processedMethods = new Dictionary<ulong, (string, MethodLoadVerbosePacket.OptimizationTier)>();

		private readonly Decoder _decoder;
		private readonly IntelFormatter _intelFormatter;
		private readonly StringBuilder _instructionOutputBuffer = new StringBuilder(256);
		private readonly StringOutput _formatterBuffer = new StringOutput();
		private readonly Dictionary<ulong, List<ulong>> _localLabels = new Dictionary<ulong, List<ulong>>();
		private readonly StackBasedCodeReader _codeReader = new StackBasedCodeReader();
		private readonly InstructionList _instructionList = new InstructionList();

		public NativeCodeDisassembler(DacDistpatcher dacDistpatcher, SymbolHelper symbolHelper, ProcessWrapper process)
		{
			_dacDistpatcher = dacDistpatcher;
			_symbolHelper = symbolHelper;
			_process = process;

			_intelFormatter = new IntelFormatter(FormatterOptions.CreateMasm());
			_intelFormatter.Options.HexPrefix = "0x";
			_intelFormatter.Options.HexSuffix = null;
			_intelFormatter.Options.AddLeadingZeroToHexNumbers = true;
			_intelFormatter.Options.ShowBranchSize = false;
			_intelFormatter.Options.UppercaseHex = false;

			_decoder = Decoder.Create(IntPtr.Size << 3, _codeReader);
		}

		public unsafe void DisassembleMethod(MethodLoadVerbosePacket e, StringBuilder outputBuffer, bool showOpcodes = true)
		{
			var methodStartAddress = (ulong) e.MethodStartAddress;
			var methodSize = e.MethodSize;

			var menthodFullNameBuffer = Ptr.OfReadOnlyRef(Buffer).AsWritableSpan(Buffer.Length);
			if (!_dacDistpatcher.TryGetMethodByInstructionPointer(e.MethodStartAddress, ref menthodFullNameBuffer)) return;
			var menthodFullName = new string(menthodFullNameBuffer);

			FormatSignature(ref menthodFullName);
			if (_processedMethods.ContainsKey(methodStartAddress) && _processedMethods[methodStartAddress].Item2 == e.Tier) return;
			_processedMethods[methodStartAddress] = (menthodFullName, e.Tier);

			outputBuffer.AppendLine(";=========================================================");
			outputBuffer.AppendLine($";{menthodFullName}");
			outputBuffer.AppendLine(";---------------------------------------------------------");
			outputBuffer.AppendLine($";OptimizationTier: {e.Tier}");

			outputBuffer.AppendLine(string.Empty);

			var codeBuffer = stackalloc byte[methodSize << 1];
			_codeReader.Init(codeBuffer, methodSize << 1);
			var span = _codeReader.AsSpan();
			_process.Read(e.MethodStartAddress, Ptr.OfRef(span), span.Length, out _);

			_decoder.IP = methodStartAddress;

			var endAddress = _decoder.IP + (ulong) methodSize;

			_instructionList.Clear();
			while (_decoder.IP < endAddress)
			{
				_decoder.Decode(out _instructionList.AllocUninitializedElement());
			}

			for (var i = 0; i < _instructionList.Count; i++)
			{
				ref var instruction = ref _instructionList[i];

				if (!TryGetLocalJmpAddress(instruction, out var address)) continue;
				if (address >= methodStartAddress && address <= methodStartAddress + (ulong) methodSize)
				{
					if (!_localLabels.ContainsKey(address))
					{
						_localLabels[address] = new List<ulong>();
					}

					_localLabels[address].Add(instruction.IP);
				}
			}

			var labelTargetAdresses = _localLabels.Keys.ToList();
			labelTargetAdresses.Sort();

			for (var i = 0; i < _instructionList.Count; i++)
			{
				ref var instruction = ref _instructionList[i];

				_intelFormatter.Format(instruction, _formatterBuffer);
				var formattedInstruction = _formatterBuffer.ToStringAndReset();

				var jmpToLabel = false;
				for (var j = 0; j < labelTargetAdresses.Count; j++)
				{
					var target = labelTargetAdresses[j];
					if (instruction.IP == target)
					{
						outputBuffer.AppendLine($"Label_{j}:");
						break;
					}

					if (!_localLabels[target].Contains(instruction.IP)) continue;
					formattedInstruction = formattedInstruction.Replace(FormatAddress(target), $"Label_{j}");
					jmpToLabel = true;
					break;
				}

				if (!jmpToLabel)
				{
					if (instruction.IsJmpFar || instruction.IsJmpNear || instruction.IsCallFar || instruction.IsCallNear)
					{
						var isStub = false;
						var addressIn = instruction.MemoryAddress64;
						if (!TryGetMethodNameByAddress(addressIn, out var methodName) && instruction.IsCallNear)
						{
							_process.Read<int>((Ptr) (instruction.IP + 1), out var disp);
							var jmp = instruction.IP + (ulong) instruction.Length + (ulong) disp;
							_process.Read((Ptr) (jmp + 1), out disp);
							var addressOut = jmp + 5 + (ulong) disp;
							isStub = TryGetMethodNameByAddress(addressOut, out methodName);
						}

						if (methodName.Length > 0)
						{
							FormatSignature(ref methodName);
							_processedMethods[addressIn] = (methodName, MethodLoadVerbosePacket.OptimizationTier.Unknown);
							formattedInstruction = formattedInstruction.Replace(FormatAddress(addressIn), isStub ? $"@{methodName}" : $"{methodName}");
						}
					}
					else if (instruction.IsCallFarIndirect || instruction.IsCallNearIndirect)
					{
						_process.Read<Ptr>((Ptr) instruction.IPRelativeMemoryAddress, out var address);
						TryGetMethodNameByAddress((ulong) address, out var methodName);
						if (methodName.Length > 0)
						{
							FormatSignature(ref methodName);
							_processedMethods[(ulong) address] = (methodName, MethodLoadVerbosePacket.OptimizationTier.Unknown);
							formattedInstruction = $"call #{methodName}";
						}
					}
				}

				_instructionOutputBuffer.Append(FormatAddress(instruction.IP));
				_instructionOutputBuffer.Append(" ");

				if (showOpcodes)
				{
					var instructionLength = instruction.Length;
					var byteBaseIndex = (int) (instruction.IP - methodStartAddress);
					for (var j = 0; j < instructionLength; j++)
					{
						_instructionOutputBuffer.Append(codeBuffer[byteBaseIndex + j].ToString("x2"));
					}

					var missingBytes = 13 - instructionLength;
					for (var j = 0; j < missingBytes; j++)
					{
						_instructionOutputBuffer.Append("  ");
					}

					_instructionOutputBuffer.Append(" ");
				}

				_instructionOutputBuffer.Append(formattedInstruction);
				outputBuffer.AppendLine(_instructionOutputBuffer.ToString());

				_formatterBuffer.Reset();
				_instructionOutputBuffer.Clear();
			}

			_localLabels.Clear();

			outputBuffer.AppendLine(string.Empty);
		}

		private bool TryGetMethodNameByAddress(ulong address, out string methodNameOutput)
		{
			if (_processedMethods.ContainsKey(address))
			{
				methodNameOutput = _processedMethods[address].Item1;
				return true;
			}

			var methodNameBuffer = Ptr.OfReadOnlyRef(Buffer).AsWritableSpan(Buffer.Length);
			if (!_dacDistpatcher.TryGetJitHelperFunctionName((Ptr) address, ref methodNameBuffer))
			{
				methodNameBuffer = Ptr.OfReadOnlyRef(Buffer).AsWritableSpan(Buffer.Length);
				if (!_dacDistpatcher.TryGetMethodByInstructionPointer((Ptr) address, ref methodNameBuffer))
				{
					methodNameBuffer = Ptr.OfReadOnlyRef(Buffer).AsWritableSpan(Buffer.Length);
					NativeStructs.SymbolInfo sym = default;
					if (_symbolHelper.TryGetSymbolFromAddress((Ptr) address, ref sym, out _) != 0)
					{
						Utf8.ToUtf16(Ptr.OfReadOnlyRef(sym.Name).AsSpan((int) sym.NameLen), methodNameBuffer, out _, out _);
						methodNameBuffer = methodNameBuffer.Slice(0, (int) sym.NameLen);
					}
					else
					{
						methodNameBuffer = Span<char>.Empty;
					}
				}
			}

			if (methodNameBuffer.Length > 0)
			{
				methodNameOutput = new string(methodNameBuffer);
				return true;
			}

			methodNameOutput = string.Empty;
			return false;
		}

		private enum State
		{
			Initial,
			OpenBracket,
			CollectChars,
			CollectRestChars,
			CloseBracket,
			SkipChars
		}

		private static unsafe void FormatSignature(ref string input)
		{
			return;

			// var inputPos = 0;
			// var curState = State.Initial;
			// var prevState = State.Initial;
			// var buf = Ptr.OfRef(stackalloc char[input.Length]);
			// var bufferPos = 0;
			//
			// while (inputPos < input.Length)
			// {
			// 	var c = input[inputPos++];
			// 	switch (c)
			// 	{
			// 		case '[':
			// 			if (curState == State.OpenBracket)
			// 			{
			// 				prevState = curState;
			// 				buf[bufferPos++] = '<';
			// 				break;
			// 			}
			//
			// 			prevState = curState;
			// 			curState = State.OpenBracket;
			// 			break;
			// 		case ']':
			// 			if (curState == State.CloseBracket)
			// 			{
			// 				prevState = curState;
			// 				buf[bufferPos++] = '>';
			// 				break;
			// 			}
			//
			// 			prevState = curState;
			// 			curState = State.CloseBracket;
			// 			break;
			// 		case ',':
			// 			if (curState == State.CollectChars || prevState == State.CloseBracket)
			// 			{
			// 				prevState = curState;
			// 				curState = State.SkipChars;
			// 				break;
			// 			}
			//
			// 			goto default;
			// 		case '`':
			// 			prevState = curState;
			// 			curState = State.SkipChars;
			// 			break;
			// 		case '<':
			// 		case '>':
			// 			curState = prevState;
			// 			goto default;
			// 		case '(':
			// 			curState = State.CollectRestChars;
			// 			goto default;
			// 		case '+':
			// 			c = '.';
			// 			goto default;
			// 		default:
			// 			if (curState == State.SkipChars) break;
			//
			// 			prevState = curState;
			// 			curState = prevState == State.CollectRestChars ? prevState : State.CollectChars;
			// 			buf[bufferPos++] = c;
			// 			if (c == ',' && curState != State.CollectRestChars)
			// 			{
			// 				buf[bufferPos++] = ' ';
			// 			}
			//
			// 			break;
			// 	}
			// }
			//
			// input = string.Empty;
			// buf.CopyTo(Ptr.OfReadOnlyRef(input.AsSpan()), bufferPos);
		}

		private static bool TryGetLocalJmpAddress(Instruction instruction, out ulong referencedAddress)
		{
			referencedAddress = default;

			for (var i = 0; i < instruction.OpCount; i++)
			{
				switch (instruction.GetOpKind(i))
				{
					case OpKind.NearBranch16:
					case OpKind.NearBranch32:
					case OpKind.NearBranch64:
						referencedAddress = instruction.NearBranchTarget;
						return referencedAddress > ushort.MaxValue;
				}
			}

			return false;
		}

		//
		// private void FormatAddress(ulong address, /*ref UnsafeLineBuilder/// </summary>StringBuilder line)
		// {
		// 	line.Append(_intelFormatter.Options.HexPrefix);
		// 	var tmp = Ptr.OfRef(stackalloc char[16]);
		// 	address.TryFormat(tmp.AsWritableSpan(16), out _, "x16");
		// 	line.Append(tmp.AsSpan(16));
		// 	line.Append(_intelFormatter.Options.HexSuffix ?? ReadOnlySpan<char>.Empty);
		// }

		private string FormatAddress(ulong address) => $"{_intelFormatter.Options.HexPrefix}{address:x16}{_intelFormatter.Options.HexSuffix}";
		//
		// private void SendToOutput(string text)
		// {
		// 	// _finalOutputBuffer.Add(text);
		// 	File.AppendAllText(OutputFile, $"{text}{Environment.NewLine}");
		// }

		static int FindStringLength(Ptr<byte> start, int maxLength = 1024)
		{
			var pos = 0;
			while (pos < maxLength - 1)
			{
				if (pos > 0 && start[pos] == 0)
				{
					return pos;
				}

				pos++;
			}

			return maxLength;
		}

		static int FindStringLength(Ptr<char> start, int maxLength = 1024)
		{
			var pos = 0;
			while (pos < maxLength - 1)
			{
				if (pos > 0 && start[pos] == 0)
				{
					return pos;
				}

				pos++;
			}

			return maxLength;
		}

		private sealed unsafe class StackBasedCodeReader : CodeReader
		{
			private int _length;
			private int _pos;
			private byte* _data;

			public void Init(byte* data, int length)
			{
				_data = data;
				_length = length;
				_pos = 0;
			}

			public override int ReadByte()
			{
				if (_pos >= _length)
				{
					return -1;
				}

				return _data[_pos++];
			}

			public Span<byte> AsSpan() => MemoryMarshal.CreateSpan(ref *_data, _length);
		}

		// private sealed class UnsafeFormatterOutput : FormatterOutput
		// {
		// 	public override void Write(string text, FormatterTextKind kind)
		// 	{
		// 	}
		// }

		// private ref struct UnsafeLineBuilder
		// {
		// 	private readonly Ptr<char> _buffer;
		// 	private int _offset;
		//
		// 	public UnsafeLineBuilder(Ptr<char> buffer)
		// 	{
		// 		_buffer = buffer;
		// 		_offset = 0;
		// 	}
		//
		// 	public void Append(ReadOnlySpan<char> span)
		// 	{
		// 		if(span.IsEmpty) return;
		// 		Ptr.OfReadOnlyRef(span).CopyTo(_buffer + _offset, span.Length);
		// 		_offset += span.Length;
		// 	}
		//
		// 	public string AsString() => new string(_buffer.AsSpan(_offset));
		// }
		//
		// public void Dispose()
		// {
		// 	_stringBuilder.Dispose();
		// }
	}

	// public sealed class UnsafeStringBuilder
	// {
	// 	private struct Line
	// 	{
	// 		public Ptr Pointer;
	// 		public long Length;
	// 	}
	//
	// 	private int _capacity;
	// 	private Ptr<Line> _lines;
	// 	private int _lineCount;
	// 	private int _size;
	//
	// 	public UnsafeStringBuilder()
	// 	{
	// 		_size = 0;
	// 		_capacity = 512 * sizeof(char) + Unsafe.SizeOf<Line>();
	// 		_lines = ((Ptr) Marshal.AllocHGlobal(_capacity)).As<Line>();
	// 		_lines.Clear(_capacity);
	// 		_lineCount = 0;
	// 	}
	//
	// 	public void AddLine(ReadOnlySpan<char> span)
	// 	{
	// 		var requiredSize = span.Length * sizeof(char) + Unsafe.SizeOf<Line>();
	// 		var newSize = _size + requiredSize;
	// 		if (newSize > _capacity)
	// 		{
	// 			while (_capacity <= newSize)
	// 			{
	// 				_capacity <<= 1;
	// 			}
	// 			var tmp = ((Ptr) Marshal.AllocHGlobal(_capacity)).As<Line>();
	// 			tmp.Clear(_capacity);
	// 			_lines.CopyTo(tmp, _size);
	// 			Marshal.FreeHGlobal(_lines.AsIntPtr());
	// 			_lines = tmp;
	// 		}
	//
	// 		var offset = 0L;
	// 		for (var i = 0; i < _lineCount; i++)
	// 		{
	// 			offset += Unsafe.SizeOf<Line>() + _lines[i].Length * sizeof(char);
	// 		}
	//
	// 		var line = (_lines.As<byte>() + offset).As<Line>();
	// 		line.Value.Length = span.IsEmpty ? 0 : span.Length;
	// 		line.Value.Pointer = _lines.As<byte>() + Unsafe.SizeOf<Line>() + offset;
	// 		_size = newSize;
	// 		_lineCount++;
	// 		Ptr.OfReadOnlyRef(span).CopyTo(line.Value.Pointer, span.Length);
	// 	}
	//
	// 	public void Clear()
	// 	{
	// 		_lines.Clear(_capacity);
	// 		_size = 0;
	// 		_lineCount = 0;
	// 	}
	//
	// 	public void Dispose()
	// 	{
	// 		Marshal.FreeHGlobal(_lines.AsIntPtr());
	// 	}
	// }

	// public static class Ex
	// {
	// 	public static void Replace(this ReadOnlySpan<char> span, in ReadOnlySpan<char> pattern, in ReadOnlySpan<char> replacement, ref Span<char> result)
	// 	{
	// 		var i = span.IndexOf(pattern);
	// 		if (pattern.Length <= replacement.Length)
	// 		{
	// 			var insertionPtr = Ptr.OfReadOnlyRef(span).As<char>() + i;
	// 			Ptr.OfReadOnlyRef(replacement).CopyTo(insertionPtr, replacement.Length);
	//
	// 		}
	// 	}
	// }
}