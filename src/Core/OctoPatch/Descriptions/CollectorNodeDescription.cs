using System;
using OctoPatch.ContentTypes;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Node to combine different inputs to get a complex type message
    /// </summary>
    public sealed class CollectorNodeDescription : NodeDescription
    {
        /// <summary>
        /// Type to collector inputs together for
        /// </summary>
        public string TypeKey { get; set; }

        private CollectorNodeDescription(string key, string typeKey, string displayName, string displayDescription) 
            : base(key, displayName, displayDescription)
        {
            TypeKey = typeKey;
        }

        /// <summary>
        /// Creates a new description for collector nodes
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="pluginId">plugin id</param>
        /// <param name="typeKey">key of the collect type</param>
        /// <param name="displayName">name of the node</param>
        /// <param name="displayDescription">optional description</param>
        /// <returns>node description</returns>
        public static CollectorNodeDescription CreateCollector<T>(Guid pluginId, string typeKey, string displayName, string displayDescription)
        {
            return new CollectorNodeDescription($"{pluginId}:{typeof(T).Name}", typeKey, displayName, displayDescription);
        }

        /// <summary>
        /// Creates a collector description for the given type
        /// </summary>
        /// <param name="typeDescription">type description</param>
        /// <returns>collector description</returns>
        public static CollectorNodeDescription CreateFromComplexType(TypeDescription typeDescription)
        {
            var result = new CollectorNodeDescription($"{typeDescription.Key}:Collector", typeDescription.Key, $"{typeDescription.DisplayName} Collector", null);

            // Add all type properties as inputs
            foreach (var propertyDescription in typeDescription.PropertyDescriptions)
            {
                result.AddInputDescription(new ConnectorDescription(
                    propertyDescription.Key, propertyDescription.DisplayName,
                    propertyDescription.DisplayDescription, propertyDescription.ContentType));
            }

            // Add given type as output
            result.AddOutputDescription(new ConnectorDescription(
                "Output", null, null,
                new ComplexContentType {Key = typeDescription.Key}));

            return result;
        }
    }
}
