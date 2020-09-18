namespace SharpCode
{
    public static class Code
    {
        public static ClassBuilder CreateClass()
        {
            return new ClassBuilder();
        }

        public static ClassBuilder CreateClass(string name, AccessModifier accessModifier = AccessModifier.Public)
        {
            return new ClassBuilder(accessModifier, name);
        }

        public static FieldBuilder CreateField()
        {
            return new FieldBuilder();
        }

        public static FieldBuilder CreateField(
            string type,
            string name,
            AccessModifier accessModifier = AccessModifier.Private)
        {
            return new FieldBuilder(accessModifier, type, name);
        }

        public static PropertyBuilder CreateProperty()
        {
            return new PropertyBuilder();
        }

        public static PropertyBuilder CreateProperty(
            string type,
            string name,
            AccessModifier accessModifier = AccessModifier.Public)
        {
            return new PropertyBuilder();
        }
    }
}
