namespace SharpCode
{
    public class NamespaceBuilder
    {
        private readonly Namespace _namespace = new Namespace();

        internal NamespaceBuilder() { }

        internal NamespaceBuilder(string name)
        {
            _namespace.Name = name;
        }

        /// <summary>
        /// Sets the name of the namespace being built.
        /// </summary>
        public NamespaceBuilder WithName(string name)
        {
            _namespace.Name = name;
            return this;
        }

        /// <summary>
        /// Adds a using directive to the namespace being built.
        /// </summary>
        /// <param name="usingDirective">
        /// The using directive to be added, for example <c>"System"</c> or <c>"System.Collections.Generic"</c>
        /// The semi-colon at the end is optional.
        /// </param>
        public NamespaceBuilder WithUsing(string usingDirective)
        {
            _namespace.Usings.Add(usingDirective.Replace(";", string.Empty));
            return this;
        }

        /// <summary>
        /// Adds a class definition to the namespace being built.
        /// </summary>
        public NamespaceBuilder WithClass(ClassBuilder builder)
        {
            _namespace.Classes.Add(builder.Build());
            return this;
        }

        /// <summary>
        /// Returns the source code of the built namespace.
        /// </summary>
        /// <param name="formatted">
        /// Indicates whether to format the source code.
        /// </param>
        public string ToSourceCode(bool formatted = true)
        {
            return Build().ToSourceCode(formatted);
        }

        /// <summary>
        /// Returns the source code of the built namespace.
        /// </summary>
        public override string ToString()
        {
            return ToSourceCode();
        }

        internal Namespace Build()
        {
            if (string.IsNullOrWhiteSpace(_namespace.Name))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the namespace is required when building a namespace.");
            }

            return _namespace;
        }
    }
}
