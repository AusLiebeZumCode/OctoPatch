using System;

namespace OctoPatch
{
    /// <summary>
    /// Node description for attached nodes
    /// </summary>
    public sealed class AttachedNodeDescription : NodeDescription
    {
        /// <summary>
        /// Gets the id of the parent node to attach to
        /// </summary>
        public Guid ParentNode { get; set; }

        private AttachedNodeDescription(Guid guid, Guid parentNode, string name, Version version, string description = null) 
            : base(guid, name, version, description)
        {
            ParentNode = parentNode;
        }

        /// <summary>
        /// Creates a new description
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="guid">node description id</param>
        /// <param name="parentNode">id of the node to attach to</param>
        /// <param name="name">name of the node</param>
        /// <param name="version">version</param>
        /// <param name="description">optional description</param>
        /// <returns>node description</returns>
        public static NodeDescription Create<T>(Guid guid, Guid parentNode, string name, Version version, string description)
        {
            return new AttachedNodeDescription(guid, parentNode, name, version, description)
            {
                TypeName = typeof(T).FullName,
            };
        }
    }
}
