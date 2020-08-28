using System;

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
        public string ComplexTypeName { get; set; }

        private CollectorNodeDescription(string key, string displayName, string displayDescription) 
            : base(key, displayName, displayDescription)
        {
        }

        /// <summary>
        /// Creates a new description for collector nodes
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="pluginId">plugin id</param>
        /// <param name="displayName">name of the node</param>
        /// <param name="displayDescription">optional description</param>
        /// <returns>node description</returns>
        public static CollectorNodeDescription CreateCollector<T>(Guid pluginId, string displayName, string displayDescription)
        {
            return new CollectorNodeDescription($"{pluginId}:{typeof(T).Name}", displayName, displayDescription);
        }
    }
}
