using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using OctoPatch.ContentTypes;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Description for a single adapter
    /// </summary>
    [DataContract]
    public sealed class AdapterDescription : KeyDescription
    {
        /// <summary>
        /// Gets a list of supported input/output combinations
        /// </summary>
        [DataMember]
        public List<(string input, string output)> SupportedTypeCombinations { get; }

        public AdapterDescription(string key, string displayName, string displayDescription) 
            : base(key, displayName, displayDescription)
        {
            SupportedTypeCombinations = new List<(string input, string output)>();
        }

        /// <summary>
        /// Adds another supported type combination
        /// </summary>
        /// <typeparam name="TI">input type</typeparam>
        /// <typeparam name="TO">output type</typeparam>
        /// <returns>Reference to the description</returns>
        public AdapterDescription AddSupportedTypeCombination<TI, TO>() 
            where TI : ContentType 
            where TO : ContentType
        {
            SupportedTypeCombinations.Add((typeof(TI).Name, typeof(TO).Name));
            return this;
        }

        /// <summary>
        /// Creates a new description for adapters
        /// </summary>
        /// <typeparam name="T">adapter type</typeparam>
        /// <param name="pluginId">plugin id</param>
        /// <param name="displayName">name of adapter</param>
        /// <param name="displayDescription">optional description</param>
        /// <returns>adapter description</returns>
        public static AdapterDescription Create<T>(Guid pluginId, string displayName, string displayDescription)
        {
            return new AdapterDescription($"{pluginId}:{typeof(T).Name}", displayName, displayDescription);
        }
    }
}
