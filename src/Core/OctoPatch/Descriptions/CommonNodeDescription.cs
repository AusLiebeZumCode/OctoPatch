using System;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Represents a common node
    /// </summary>
    public sealed class CommonNodeDescription : NodeDescription
    {
        public CommonNodeDescription(string key, string displayName, string displayDescription) 
            : base(key, displayName, displayDescription)
        {
        }

        /// <summary>
        /// Creates a new description for common nodes
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="pluginId">plugin id</param>
        /// <param name="displayName">name of the node</param>
        /// <param name="displayDescription">optional description</param>
        /// <returns>node description</returns>
        public static CommonNodeDescription Create<T>(Guid pluginId, string displayName, string displayDescription)
        {
            return new CommonNodeDescription($"{pluginId}:{typeof(T).Name}", displayName, displayDescription);
        }
    }
}
