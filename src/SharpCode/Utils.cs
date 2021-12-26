using Microsoft.CodeAnalysis;

namespace SharpCode
{
    internal static class Utils
    {
        public static T[] AsArray<T>(params T[] items) => items;

        public static SyntaxTokenList AsList(params SyntaxToken[] items) => new SyntaxTokenList(items);
    }
}
