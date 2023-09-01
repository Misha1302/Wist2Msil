namespace WistConst;

public sealed class WistRepeatEnumerator
{
    private readonly int _max;
    private readonly int _step;
    private int _cur;

    public WistRepeatEnumerator(int start, int max, int step)
    {
        _cur = start - step;
        _max = max;
        _step = step;
    }

    public bool Next()
    {
        _cur += _step;
        return _cur <= _max;
    }

    public WistConst Current() => new(_cur);
}