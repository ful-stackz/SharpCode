namespace SharpCode.Test
{
    public static class StringExtensions
    {
        public static string WithUnixEOL(this string source) =>
            source.Replace("\r\n", "\n");
    }
}
