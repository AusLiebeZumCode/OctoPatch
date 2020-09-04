using System;
using OctoPatch.ContentTypes;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Node to split up complex types into its properties
    /// </summary>
    public sealed class SplitterNodeDescription : NodeDescription
    {
        /// <summary>
        /// Gets the type key to split
        /// </summary>
        public string TypeKey { get; set; }

        private SplitterNodeDescription(string key, string typeKey, string displayName, string displayDescription) 
            : base(key, displayName, displayDescription)
        {
            TypeKey = typeKey;
        }

        /// <summary>
        /// Creates a new description for splitter nodes
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="pluginId">plugin id</param>
        /// <param name="typeKey">key of the split type</param>
        /// <param name="displayName">name of the node</param>
        /// <param name="displayDescription">optional description</param>
        /// <returns>node description</returns>
        public static SplitterNodeDescription CreateSplitter<T>(Guid pluginId, string typeKey, string displayName, string displayDescription)
        {
            return new SplitterNodeDescription($"{pluginId}:{typeof(T).Name}", typeKey, displayName, displayDescription);
        }

        /// <summary>
        /// Creates a splitter description for the given type
        /// </summary>
        /// <param name="typeDescription">type description</param>
        /// <returns>splitter description</returns>
        public static SplitterNodeDescription CreateFromComplexType(TypeDescription typeDescription)
        {
            var result = new SplitterNodeDescription($"{typeDescription.Key}:Splitter", typeDescription.Key, $"{typeDescription.DisplayName} Splitter", null);
            
            // Add all type properties as outputs
            foreach (var propertyDescription in typeDescription.PropertyDescriptions)
            {
                result.AddOutputDescription(new ConnectorDescription(
                    propertyDescription.Key, propertyDescription.DisplayName,
                    propertyDescription.DisplayDescription, propertyDescription.ContentType));
            }

            return result;
        }
    }
}
