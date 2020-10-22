/*
Copyright (C) 2018-2019 de4dot@gmail.com

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

// ⚠️This file was generated by GENERATOR!🦹‍♂️

#nullable enable

#if GAS || INTEL || MASM || NASM
namespace Iced.Intel {
	/// <summary>Formatter text kind</summary>
	public enum FormatterTextKind {
		/// <summary>Normal text</summary>
		Text = 0,
		/// <summary>Assembler directive</summary>
		Directive = 1,
		/// <summary>Any prefix</summary>
		Prefix = 2,
		/// <summary>Any mnemonic</summary>
		Mnemonic = 3,
		/// <summary>Any keyword</summary>
		Keyword = 4,
		/// <summary>Any operator</summary>
		Operator = 5,
		/// <summary>Any punctuation</summary>
		Punctuation = 6,
		/// <summary>Number</summary>
		Number = 7,
		/// <summary>Any register</summary>
		Register = 8,
		/// <summary>A decorator, eg. <c>sae</c> in <c>{sae}</c></summary>
		Decorator = 9,
		/// <summary>Selector value (eg. far <c>JMP</c>/<c>CALL</c>)</summary>
		SelectorValue = 10,
		/// <summary>Label address (eg. <c>JE XXXXXX</c>)</summary>
		LabelAddress = 11,
		/// <summary>Function address (eg. <c>CALL XXXXXX</c>)</summary>
		FunctionAddress = 12,
		/// <summary>Data symbol</summary>
		Data = 13,
		/// <summary>Label symbol</summary>
		Label = 14,
		/// <summary>Function symbol</summary>
		Function = 15,
	}
}
#endif