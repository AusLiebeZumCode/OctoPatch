using System;

namespace OctoPatch
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

        private CollectorNodeDescription(Guid guid, string name, Version version, string description = null) 
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
            return new CollectorNodeDescription(guid, name, version, description)
            {
                ComplexTypeName = typeof(TM).FullName,
                TypeName = typeof(T).FullName,
            };
        }
    }
}
