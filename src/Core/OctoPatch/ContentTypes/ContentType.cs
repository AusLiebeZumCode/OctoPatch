using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Describes the type of content within a connector or complex type
    /// </summary>
    [DataContract]
    public abstract class ContentType
    {
        /// <summary>
        /// Returns the content type name
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public string Type => GetType().Name;

        /// <summary>
        /// Checks if the given type is supported by the content type
        /// </summary>
        /// <typeparam name="T">requested type</typeparam>
        /// <returns>true when the given type can be processed by the content type</returns>
        public bool IsSupportedType<T>()
        {
            return IsSupportedType(typeof(T));
        }

        /// <summary>
        /// Checks if the given type is supported by the content type
        /// </summary>
        /// <param name="type">requested type</param>
        /// <returns>true when the given type can be processed by the content type</returns>
        public abstract bool IsSupportedType(Type type);

        /// <summary>
        /// Normalizes the value of the message based on the given content type parameters
        /// </summary>
        /// <param name="value">input value</param>
        /// <returns>normalized output</returns>
        public virtual ValueType NormalizeValue(ValueType value)
        {
            // In most cases this will handover just the input value
            return value;
        }
    }
}
