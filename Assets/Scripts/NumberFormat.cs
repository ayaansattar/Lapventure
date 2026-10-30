public static class NumberFormat
{
    static readonly string[] suffixes = { "", "K", "M", "B", "T", "Qa", "Qi" };
    public static string Format(double v)
    {
        int i = 0;
        while (v >= 1000 && i < suffixes.Length - 1) { v /= 1000; i++; }
        return i == 0 ? v.ToString("0") : v.ToString("0.##") + suffixes[i];
    }
}
