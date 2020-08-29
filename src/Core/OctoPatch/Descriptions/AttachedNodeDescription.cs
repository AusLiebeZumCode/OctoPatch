using System;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Node description for attached nodes
    /// </summary>
    public sealed class AttachedNodeDescription : NodeDescription
    {
        /// <summary>
        /// Gets or sets the key of the parent node to attach to
        /// </summary>
        public string ParentKey { get; set; }

        private AttachedNodeDescription(string key, string displayName, string displayDescription, string parentKey)
            : base(key, displayName, displayDescription)
        {
            ParentKey = parentKey;
        }

        /// <summary>
        /// Creates a new description for attached nodes
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="pluginId">plugin id</param>
        /// <param name="parentKey">id of the node to attach to</param>
        /// <param name="displayName">name of the node</param>
        /// <param name="displayDescription">optional description</param>
        /// <returns>node description</returns>
        public static AttachedNodeDescription CreateAttached<T>(Guid pluginId, string displayName, string displayDescription, string parentKey)
        {
            return new AttachedNodeDescription($"{pluginId}:{typeof(T).Name}", displayName, displayDescription, parentKey);
        }
    }
}
