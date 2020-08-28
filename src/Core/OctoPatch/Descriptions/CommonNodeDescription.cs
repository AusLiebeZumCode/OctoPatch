using System;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Description for a common node
    /// </summary>
    public sealed class CommonNodeDescription : NodeDescription
    {
        private CommonNodeDescription(Guid guid, string name, Version version, string description = null) 
            : base(guid, name, version, description)
        {
        }

        /// <summary>
        /// Creates a new description
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="guid">node description id</param>
        /// <param name="name">name of the node</param>
        /// <param name="version">version</param>
        /// <param name="description">optional description</param>
        /// <returns>node description</returns>
        public static NodeDescription Create<T>(Guid guid, string name, Version version, string description)
        {
            return new CommonNodeDescription(guid, name, version, description)
            {
                TypeName = typeof(T).FullName,
            };
        }
    }
}
