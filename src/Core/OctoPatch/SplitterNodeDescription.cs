using System;

namespace OctoPatch
{
    /// <summary>
    /// Node to split up complex types into its properties
    /// </summary>
    public sealed class SplitterNodeDescription : NodeDescription
    {
        /// <summary>
        /// Gets the complex type name to split
        /// </summary>
        public string ComplexTypeName { get; set; }

        private SplitterNodeDescription(Guid guid, string name, Version version, string description = null) 
            : base(guid, name, version, description)
        {
        }

        /// <summary>
        /// Creates a new description
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <typeparam name="TM">complex message type</typeparam>
        /// <param name="guid">node description id</param>
        /// <param name="name">name of the node</param>
        /// <param name="version">version</param>
        /// <param name="description">optional description</param>
        /// <returns>node description</returns>
        public static NodeDescription Create<T, TM>(Guid guid, string name, Version version, string description)
        {
            return new SplitterNodeDescription(guid, name, version, description)
            {
                TypeName = typeof(T).FullName,
                ComplexTypeName = typeof(TM).FullName,
            };
        }
    }
}
