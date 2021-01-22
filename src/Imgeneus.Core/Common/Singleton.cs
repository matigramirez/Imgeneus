using System;

namespace Imgeneus.Core.Common
{
    /// <summary>
    /// Lazy singleton implementation.
    /// </summary>
    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> instance = new Lazy<T>(() => new T());

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static T Instance => instance.Value;

        /// <summary>
        /// Creates a new <see cref="Singleton{T}"/> internally.
        /// </summary>
        protected Singleton()
        {

        }
    }
}
