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
        private readonly Constructor _constructor = new Constructor();

        internal ConstructorBuilder(AccessModifier accessModifier)
        {
            _constructor.AccessModifier = accessModifier;
        }

        /// <summary>
        /// Sets the access modifier of the constructor being built.
        /// </summary>
        public ConstructorBuilder WithAccessModifier(AccessModifier accessModifier)
        {
            _constructor.AccessModifier = accessModifier;
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
        /// the following line being added to the constructor body - <c>{receivingMember} = {name};</c>
        /// </param>
        /// <example>
        /// This example shows the generated code for a constructor with a parameter.
        ///
        /// <code>
        /// // ConstructorBuilder.WithParameter("string", "username");
        /// 
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
        /// 
        /// public User(string username)
        /// {
        ///     _username = username;
        /// }
        /// </code>
        /// </example>
        public ConstructorBuilder WithParameter(string type, string name, string? receivingMember = null)
        {
            _constructor.Parameters.Add(new Parameter
            {
                Name = name,
                ReceivingMember = receivingMember,
                Type = type,
            });
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
        /// the following line being added to the constructor body - <c>{receivingMember} = {name};</c>
        /// </param>
        /// <example>
        /// This example shows the generated code for a constructor with a parameter.
        ///
        /// <code>
        /// // ConstructorBuilder.WithParameter(typeof(string), "username");
        /// 
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
        /// 
        /// public User(String username)
        /// {
        ///     _username = username;
        /// }
        /// </code>
        /// </example>
        public ConstructorBuilder WithParameter(Type type, string name, string? receivingMember = null)
        {
            _constructor.Parameters.Add(new Parameter
            {
                Name = name,
                ReceivingMember = receivingMember,
                Type = type.Name,
            });
            return this;
        }

        /// <summary>
        /// Sets the <c>base()</c> call of the constructor with the specified <paramref name="passedParamaters" />.
        /// </summary>
        /// <param name="passedParameters">
        /// The parameters that will be passed to the <c>base()</c> call.
        /// </param>
        /// <example>
        /// This example shows the generated code for a constructor with a base call.
        ///
        /// <code>
        /// // ConstructorBuilder.WithBaseCall();
        /// 
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
        /// 
        /// public User(string username): base(username)
        /// {
        /// }
        /// </code>
        /// </example>
        public ConstructorBuilder WithBaseCall(params string[] passedParameters)
        {
            _constructor.BaseCallParameters = Option.Some<IEnumerable<string>>(passedParameters);
            return this;
        }

        internal ConstructorBuilder MakeStatic(bool makeStatic)
        {
            _constructor.IsStatic = makeStatic;
            return this;
        }

        internal ConstructorBuilder WithClassName(string name)
        {
            _constructor.ClassName = name;
            return this;
        }

        internal Constructor Build()
        {
            if (_constructor.IsStatic)
            {
                if (_constructor.AccessModifier != AccessModifier.None)
                {
                    throw new SyntaxException("Access modifiers are not allowed on static constructors. (CS0515)");
                }
                else if (_constructor.Parameters.Any())
                {
                    throw new SyntaxException("Parameters are not allowed on static constructors. (CS0132)");
                }
                else if (_constructor.BaseCallParameters.HasValue)
                {
                    throw new SyntaxException("Static constructors cannot call base constructors. (CS0514)");
                }
            }

            return _constructor;
        }
    }
}
