namespace Wist2Msil;

using System.Globalization;

public static class WistNumberParser
{
    private static readonly CultureInfo _dotCulture = new("en") { NumberFormat = { NumberDecimalSeparator = "." } };

    public static double ToDouble(this string s) => double.Parse(s.Replace("_", ""), NumberStyles.Any, _dotCulture);
    public static bool ToBool(this string s) => s.ToLower() is "true" or "yes";
}