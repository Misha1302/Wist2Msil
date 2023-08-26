namespace WistConst;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public sealed class WistGcHandleProvider : IDisposable
{
    private GCHandle _handle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistGcHandleProvider(object target)
    {
        _handle = target.ToGcHandle();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if RELEASE
    public unsafe T Get<T>() => Unsafe.As<nint, T>(ref *(nint*)((nint)_handle & ~(nint)1));
#else
    public T Get<T>() => (T)_handle.Target!;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReleaseUnmanagedResources()
    {
        if (_handle.IsAllocated)
            _handle.Free();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ~WistGcHandleProvider()
    {
        ReleaseUnmanagedResources();
    }
}