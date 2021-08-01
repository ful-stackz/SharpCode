using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

namespace SharpCode
{
    /// <summary>
    /// Provides functionality for building constructors. <see cref="ConstructorBuilder"/> instances are <b>not</b>
    /// immutable.
    /// </summary>
    public class ConstructorBuilder
    {
        internal ConstructorBuilder()
        {
        }

        internal ConstructorBuilder(AccessModifier accessModifier)
        {
            Constructor = new Constructor(accessModifier);
        }

        internal Constructor Constructor { get; private set; } = new Constructor(AccessModifier.Public);

        /// <summary>
        /// Sets the access modifier of the constructor being built.
        /// </summary>
        public ConstructorBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            Constructor = Constructor.With(accessModifier: Option.Some(accessModifier));
            return this;
        }

        /// <summary>
        /// Adds an accepted parameter to the constructor.
        /// </summary>
        /// <param name="type">
        /// The type of the parameter.
        /// </param>
        /// <param name="name">
        /// The name of the parameter.
        /// </param>
        /// <param name="receivingMember">
        /// A member of the class to which this parameter will be assigned. Providing this parameter will result in
        /// the following line being added to the constructor body - <c>{receivingMember} = {name};</c>.
        /// </param>
        /// <example>
        /// This example shows the generated code for a constructor with a parameter.
        ///
        /// <code>
        /// // ConstructorBuilder.WithParameter("string", "username");
        /// public User(string username)
        /// {
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// This example shows the generated code for a constructor with a parameter with a receiving member.
        ///
        /// <code>
        /// // ConstructorBuilder.WithParameter("string", "username", "_username");
        /// public User(string username)
        /// {
        ///     _username = username;
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="type"/> or <paramref name="name"/> are <c>null</c>.
        /// </exception>
        public ConstructorBuilder WithParameter(string type, string name, string? receivingMember = null)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Constructor.Parameters.Add(new Parameter(
                type: type,
                name: name,
                receivingMember: string.IsNullOrEmpty(receivingMember)
                    ? Option.None<string>()
                    : Option.Some(receivingMember!)));
            return this;
        }

        /// <summary>
        /// Adds an accepted parameter to the constructor.
        /// </summary>
        /// <param name="type">
        /// The type of the parameter.
        /// </param>
        /// <param name="name">
        /// The name of the parameter.
        /// </param>
        /// <param name="receivingMember">
        /// A member of the class to which this parameter will be assigned. Providing this parameter will result in
        /// the following line being added to the constructor body - <c>{receivingMember} = {name};</c>.
        /// </param>
        /// <example>
        /// This example shows the generated code for a constructor with a parameter.
        ///
        /// <code>
        /// // ConstructorBuilder.WithParameter(typeof(string), "username");
        /// public User(String username)
        /// {
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// This example shows the generated code for a constructor with a parameter with a receiving member.
        ///
        /// <code>
        /// // ConstructorBuilder.WithParameter(typeof(string), "username", "_username");
        /// public User(String username)
        /// {
        ///     _username = username;
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="type"/> or <paramref name="name"/> are <c>null</c>.
        /// </exception>
        public ConstructorBuilder WithParameter(Type type, string name, string? receivingMember = null)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Constructor.Parameters.Add(new Parameter(
                type: type.Name,
                name: name,
                receivingMember: string.IsNullOrEmpty(receivingMember)
                    ? Option.None<string>()
                    : Option.Some(receivingMember!)));
            return this;
        }

        /// <summary>
        /// Sets the <c>base()</c> call of the constructor with the specified <paramref name="passedParameters" />.
        /// </summary>
        /// <param name="passedParameters">
        /// The parameters that will be passed to the <c>base()</c> call.
        /// </param>
        /// <example>
        /// This example shows the generated code for a constructor with a base call.
        ///
        /// <code>
        /// // ConstructorBuilder.WithBaseCall();
        /// public User(): base()
        /// {
        /// }
        /// </code>
        /// </example>
        /// <example>
        /// This example shows the generated code for a constructor with a base call with passed parameters.
        ///
        /// <code>
        /// // ConstructorBuilder.WithParameter("string", "username").WithBaseCall("username");
        /// public User(string username): base(username)
        /// {
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentException">
        /// One of the <paramref name="passedParameters"/> values is <c>null</c>.
        /// </exception>
        public ConstructorBuilder WithBaseCall(params string[] passedParameters)
        {
            if (passedParameters.Any(x => x is null))
            {
                throw new ArgumentException($"One of the {nameof(passedParameters)} parameter values was null.");
            }

            Constructor = Constructor.With(baseCallParameters: Option.Some<IEnumerable<string>>(passedParameters));
            return this;
        }

        /// <summary>
        /// Adds XML summary documentation to the constructor.
        /// </summary>
        /// <param name="summary">
        /// The content of the summary documentation.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The specified <paramref name="summary"/> is <c>null</c>.
        /// </exception>
        public ConstructorBuilder WithSummary(string summary)
        {
            if (summary is null)
            {
                throw new ArgumentNullException(nameof(summary));
            }

            Constructor = Constructor.With(summary: Option.Some(summary));
            return this;
        }

        internal ConstructorBuilder MakeStatic(bool makeStatic)
        {
            Constructor = Constructor.With(isStatic: Option.Some(makeStatic));
            return this;
        }

        internal ConstructorBuilder WithName(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Constructor = Constructor.With(className: Option.Some(name));
            return this;
        }

        internal Constructor Build()
        {
            if (Constructor.IsStatic)
            {
                if (Constructor.AccessModifier != AccessModifier.None)
                {
                    throw new SyntaxException("Access modifiers are not allowed on static constructors. (CS0515)");
                }
                else if (Constructor.Parameters.Any())
                {
                    throw new SyntaxException("Parameters are not allowed on static constructors. (CS0132)");
                }
                else if (Constructor.BaseCallParameters.HasValue)
                {
                    throw new SyntaxException("Static constructors cannot call base constructors. (CS0514)");
                }
            }

            return Constructor;
        }
    }
}
