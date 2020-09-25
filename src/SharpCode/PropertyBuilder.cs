using System;
using Optional;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building properties. <see cref="PropertyBuilder"/> instances are <b>not</b>
    /// immutable.
    /// </summary>
    public class PropertyBuilder
    {
        private readonly Property _property = new Property();

        internal PropertyBuilder() { }

        internal PropertyBuilder(AccessModifier accessModifier, string type, string name)
        {
            _property.AccessModifier = accessModifier;
            _property.Type = type;
            _property.Name = name;
        }

        internal PropertyBuilder(AccessModifier accessModifier, Type type, string name)
            : this(accessModifier, type.Name, name)
        {
        }

        /// <summary>
        /// Sets the access modifier of the property being built.
        /// </summary>
        public PropertyBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            _property.AccessModifier = accessModifier;
            return this;
        }

        /// <summary>
        /// Sets the type of the property being built.
        /// </summary>
        public PropertyBuilder WithType(string type)
        {
            _property.Type = type;
            return this;
        }

        /// <summary>
        /// Sets the type of the property being built.
        /// </summary>
        public PropertyBuilder WithType(Type type)
        {
            _property.Type = type.Name;
            return this;
        }

        /// <summary>
        /// Sets the name of the property being built.
        /// </summary>
        public PropertyBuilder WithName(string name)
        {
            _property.Name = name;
            return this;
        }

        /// <summary>
        /// Sets the getter logic of the property being built.
        /// </summary>
        /// <param name="expression">
        /// The expression or block body of the getter. If not specified the default getter will be used - <c>get;</c>
        /// </param>
        /// <example>
        /// This example shows the generated code for a property with a default getter.
        /// 
        /// <code>
        /// // PropertyBuilder.WithName("Identifier").WithGetter()
        /// 
        /// public int Identifier
        /// {
        ///     get;
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// This example shows the generated code for a property with a getter that is an expression.
        /// 
        /// <code>
        /// // PropertyBuilder.WithName("Identifier").WithGetter("_id")
        /// 
        /// public int Identifier
        /// {
        ///     get => _id;
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// This example shows the generated code for a property with a getter that has a block body.
        /// 
        /// <code>
        /// // PropertyBuilder.WithName("IsCreated").WithGetter("{ if (_id == -1) { return false; } else { return true; } }")
        /// 
        /// public int Identifier
        /// {
        ///     get
        ///     {
        ///         if (_id == -1)
        ///         {
        ///             return false;
        ///         }
        ///         else
        ///         {
        ///             return true;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public PropertyBuilder WithGetter(string? expression = null)
        {
            _property.Getter = Option.Some(expression);
            return this;
        }

        /// <summary>
        /// Specifies that the property being built should not have a getter.
        /// </summary>
        public PropertyBuilder WithoutGetter()
        {
            _property.Getter = Option.None<string?>();
            return this;
        }

        /// <summary>
        /// Sets the setter logic of the property being built.
        /// </summary>
        /// <param name="expression">
        /// The expression or block body of the setter. If not specified the default setter will be used - <c>set;</c>
        /// Custom setter logic can make use of the <c>value</c> provided to the property setter.
        /// </param>
        /// <example>
        /// This example shows the generated code for a property with a default setter.
        /// 
        /// <code>
        /// // PropertyBuilder.WithName("Identifier").WithSetter()
        /// 
        /// public int Identifier
        /// {
        ///     set;
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// This example shows the generated code for a property with a setter that is an expression.
        /// 
        /// <code>
        /// // PropertyBuilder.WithName("Identifier").WithSetter("_id = value")
        /// 
        /// public int Identifier
        /// {
        ///     set => _id = value;
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// This example shows the generated code for a property with a setter that has a block body.
        /// 
        /// <code>
        /// // PropertyBuilder.WithName("Value")
        /// //    .WithSetter("{ if (value &lt;= 0) { throw new Exception(); } _value = value; }")
        /// 
        /// public int Identifier
        /// {
        ///     set
        ///     {
        ///         if (_id == -1)
        ///         {
        ///             return false;
        ///         }
        ///         else
        ///         {
        ///             return true;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public PropertyBuilder WithSetter(string? expression = null)
        {
            _property.Setter = Option.Some(expression);
            return this;
        }

        /// <summary>
        /// Specifies that the property being built should not have a setter.
        /// </summary>
        public PropertyBuilder WithoutSetter()
        {
            _property.Setter = Option.None<string?>();
            return this;
        }

        /// <summary>
        /// Sets the default value of the property being built.
        /// </summary>
        /// <param name="defaultValue">
        /// The default value of the property. The value is used as-is, therefore you should wrap any string values in
        /// escaped quotes. For example,
        /// <c>WithDefaultValue(defaultValue: "\"this will be generated as a string\"")</c>
        /// </param>
        public PropertyBuilder WithDefaultValue(string defaultValue)
        {
            _property.DefaultValue = Option.Some(defaultValue);
            return this;
        }

        /// <summary>
        /// Sets whether the property being built should be static or not.
        /// </summary>
        /// <param name="makeStatic">
        /// Indicates whether the property should be static or not.
        /// </param>
        public PropertyBuilder MakeStatic(bool makeStatic = true)
        {
            _property.IsStatic = makeStatic;
            return this;
        }

        /// <summary>
        /// Returns the source code of the built property.
        /// </summary>
        /// <param name="formatted">
        /// Indicates whether to format the source code.
        /// </param>
        public string ToSourceCode(bool formatted = true)
        {
            return Build().ToSourceCode(formatted);
        }

        /// <summary>
        /// Returns the source code of the built property.
        /// </summary>
        public override string ToString()
        {
            return ToSourceCode();
        }

        internal Property Build()
        {
            if (string.IsNullOrWhiteSpace(_property.Name))
            {
                throw new MissingBuilderSettingException(
                    "Providing the name of the property is required when building a property.");
            }
            else if (string.IsNullOrWhiteSpace(_property.Type))
            {
                throw new MissingBuilderSettingException(
                    "Providing the type of the property is required when building a property.");
            }
            else if (_property.Setter.Exists(value => string.IsNullOrWhiteSpace(value)))
            {
                if (!_property.Getter.HasValue)
                {
                    throw new SyntaxException(
                        "Properties with auto implemented setters must also have auto implemented getters. (CS8051)");
                }
                else if (_property.Getter.Exists(value => !string.IsNullOrWhiteSpace(value)))
                {
                    throw new SyntaxException(
                        "Properties with custom getters cannot have auto implemented setters.");
                }
            }
            else if (_property.DefaultValue.HasValue)
            {
                if (_property.Getter.Exists(value => !string.IsNullOrWhiteSpace(value)) ||
                    _property.Setter.Exists(value => !string.IsNullOrWhiteSpace(value)))
                {
                    throw new SyntaxException("Only auto implemented properties can have a default value. (CS8050)");
                }
            }

            return _property;
        }
    }
}
