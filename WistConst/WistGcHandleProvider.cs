namespace WistConst;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public sealed class WistGcHandleProvider
{
    private GCHandle _handle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistGcHandleProvider(object target)
    {
        _handle = target.ToGcHandle();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if RELEASE
    public unsafe T Get<T>() => Unsafe.As<nint, T>(ref *(nint*)((nint)_handle & ~(nint)1));
#else
    public T Get<T>() => (T)_handle.Target!;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ~WistGcHandleProvider()
    {
        _handle.Free();
    }
}