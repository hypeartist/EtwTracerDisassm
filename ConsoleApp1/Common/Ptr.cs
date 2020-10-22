using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ConsoleApp1.Common
{
	[DebuggerDisplay("{" + nameof(_ptr) + "}")]
	public readonly unsafe struct Ptr<T>
		where T : unmanaged
	{
		private readonly T* _ptr;
		public Ptr(IntPtr p) => _ptr = (T*)p.ToPointer();
		public Ptr(T* p) => _ptr = p;
		public Ptr(void* p) => _ptr = (T*)p;
		public Ptr(ref T p) => _ptr = (T*)Unsafe.AsPointer(ref p);
		public ref T Value => ref *_ptr;
		public bool IsNull => _ptr == null;
		public ref T this[int idx] => ref _ptr[idx];
		public Ptr<TT> As<TT>()
			where TT : unmanaged =>
			new Ptr<TT>(_ptr);
		public IntPtr AsIntPtr() => new IntPtr(_ptr);
		// public Ptr AsPtr() => new Ptr(_ptr);
		public ReadOnlySpan<T> AsSpan(int length) => new ReadOnlySpan<T>(_ptr, length);
		public Span<T> AsWritableSpan(int length) => new Span<T>(_ptr, length);
		public void CopyTo(Ptr p, int count) => Unsafe.CopyBlock(p.AsIntPtr().ToPointer(), _ptr, (uint) (count * Unsafe.SizeOf<T>()));
		public void Clear(int count) => Unsafe.InitBlock(_ptr, 0, (uint) (count * Unsafe.SizeOf<T>()));
		public static Ptr<T> operator ++(Ptr<T> p1) => new Ptr<T>(p1._ptr + 1);
		public static Ptr<T> operator +(Ptr<T> p1, int disp) => new Ptr<T>(p1._ptr + disp);
		public static Ptr<T> operator +(Ptr<T> p1, uint disp) => new Ptr<T>(p1._ptr + disp);
		public static Ptr<T> operator +(Ptr<T> p1, long disp) => new Ptr<T>(p1._ptr + disp);
		public static Ptr<T> operator +(Ptr<T> p1, ulong disp) => new Ptr<T>(p1._ptr + disp);
		public static Ptr<T> operator --(Ptr<T> p1) => new Ptr<T>(p1._ptr - 1);
		public static Ptr<T> operator -(Ptr<T> p1, int disp) => new Ptr<T>(p1._ptr - disp);
		public static Ptr<T> operator -(Ptr<T> p1, uint disp) => new Ptr<T>(p1._ptr - disp);
		public static Ptr<T> operator -(Ptr<T> p1, long disp) => new Ptr<T>(p1._ptr - disp);
		public static Ptr<T> operator -(Ptr<T> p1, ulong disp) => new Ptr<T>(p1._ptr - disp);
		public static long operator -(Ptr<T> p1, Ptr<T> p2) => p1._ptr - p2._ptr;
		public static bool operator ==(Ptr<T> p1, Ptr<T> p2) => p1._ptr == p2._ptr;
		public static bool operator !=(Ptr<T> p1, Ptr<T> p2) => !(p1 == p2);
		public static implicit operator Ptr(Ptr<T> p) => new Ptr(p._ptr);
		public override string ToString() => ((byte*)_ptr)->ToString(CultureInfo.InvariantCulture);
	}

	[DebuggerDisplay("{" + nameof(_ptr) + "}")]
	public readonly unsafe struct Ptr
	{
		public static readonly Ptr Null = default;

		private readonly void* _ptr;
		public Ptr(void* ptr) => _ptr = ptr;
		public Ptr(IntPtr ptr) => _ptr = ptr.ToPointer();
		public bool IsNull => _ptr == null;
		public Ptr<T> As<T>()
			where T : unmanaged =>
			new Ptr<T>(_ptr);
		public static Ptr<T> OfRef<T>(Span<T> r)
			where T : unmanaged =>
			new Ptr<T>(ref r[0]);
		public static Ptr<T> OfRef<T>(ref T r)
			where T : unmanaged =>
			new Ptr<T>(ref r);
		public static Ptr<T> OfReadOnlyRef<T>(in T r)
			where T : unmanaged =>
			new Ptr<T>(ref Unsafe.AsRef(r));
		public static Ptr<T> OfReadOnlyRef<T>(in ReadOnlySpan<T> r)
			where T : unmanaged =>
			new Ptr<T>(ref Unsafe.AsRef(r[0]));
		public IntPtr AsIntPtr() => new IntPtr(_ptr);
		// public static Ptr operator +(Ptr p1, long disp) => new Ptr((byte*) p1._ptr + disp * IntPtr.Size);
		public static bool operator ==(Ptr p1, Ptr p2) => p1._ptr == p2._ptr;
		public static bool operator !=(Ptr p1, Ptr p2) => !(p1 == p2);
		public static implicit operator Ptr(IntPtr p) => new Ptr(p.ToPointer());
		public static explicit operator int(Ptr p) => p.AsIntPtr().ToInt32();
		public static explicit operator uint(Ptr p) => (uint) p.AsIntPtr().ToInt32();
		public static explicit operator long(Ptr p) => p.AsIntPtr().ToInt64();
		public static explicit operator ulong(Ptr p) => (ulong) p.AsIntPtr().ToInt64();
		public static explicit operator Ptr(int p) => new Ptr((void*) p);
		public static explicit operator Ptr(uint p) => new Ptr((void*) p);
		public static explicit operator Ptr(long p) => new Ptr((void*) p);
		public static explicit operator Ptr(ulong p) => new Ptr((void*) p);
		public override string ToString() => ((byte*)_ptr)->ToString(CultureInfo.InvariantCulture);
	}
	//
	// public readonly unsafe ref struct Blk<T>
	// 	where T : unmanaged
	// {
	// 	private readonly T* _ptr;
	// 	private readonly int _size;
	// 	private readonly bool _stackBased;
	//
	// 	private Blk(T* p, int size, bool stackBased)
	// 	{
	// 		_ptr = p;
	// 		_stackBased = stackBased;
	// 		_size = size;
	// 	}
	//
	// 	public int Size => _size;
	//
	// 	public ref T this[int idx] => ref _ptr[idx];
	// 	public static Blk<T> Of(int size) => new Blk<T>((T*)Marshal.AllocHGlobal(size*Unsafe.SizeOf<T>()), size, false);
	// 	public static Blk<T> OfRef(Span<T> r) => new Blk<T>((T*) Unsafe.AsPointer(ref r[0]), r.Length, true);
	// 	public void Dispose()
	// 	{
	// 		if(_stackBased || _size == 0) return;
	// 		Marshal.FreeHGlobal((IntPtr) _ptr);
	// 		Ptr.OfReadOnlyRef(_size).Value = 0;
	// 	}
	// }

	public unsafe ref struct Lst<T>
		where T : unmanaged
	{
		private T* _dataPointer;
		private int _size;

		public int Count { get; private set; }

		public void Add(in T element)
		{
			if (Count + 1 > _size)
			{
				var tmp = Allocate(_size = Count == 0 ? 4 : Count << 1);
				Unsafe.CopyBlock(tmp, _dataPointer, (uint)(Count * Unsafe.SizeOf<T>()));
				Free(_dataPointer);
				_dataPointer = tmp;
			}
			_dataPointer[Count++] = element;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T* elements, int count)
		{
			var newCnt = Count + count;
			if (newCnt > _size)
			{
				_size = Count == 0 ? 4 : Count << 1;
				while (_size < newCnt)
				{
					_size <<= 1;
				}
				var tmp = Allocate(_size);
				Unsafe.CopyBlock(tmp, _dataPointer, (uint)(Count * Unsafe.SizeOf<T>()));
				Free(_dataPointer);
				_dataPointer = tmp;
			}
			Unsafe.CopyBlock(_dataPointer + Count, elements, (uint)(count * Unsafe.SizeOf<T>()));
			Count += count;
		}

		public void Clear() => Count = 0;

		public ref T this[int pos] => ref _dataPointer[pos];

		public T* this[uint pos] => &_dataPointer[pos];

		public int RawSizeFull => _size * Unsafe.SizeOf<T>();
		
		public int RawSizeData => Count * Unsafe.SizeOf<T>();

		private static T* Allocate(int count)
		{
			return (T*)Marshal.AllocHGlobal(count * Unsafe.SizeOf<T>());
		}

		private static void Free(void* ptr)
		{
			if (ptr == (void*)0)
			{
				return;
			}
			Marshal.FreeHGlobal((IntPtr) ptr);
		}

		public void Dispose()
		{
			Count = _size = 0;
			Free(_dataPointer);
		}
	}
}