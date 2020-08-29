using System;

namespace OctoPatch
{
    /// <summary>
    /// Represents a wire between an input- and an output-connector
    /// </summary>
    public sealed class WireInstance
    {
        /// <summary>
        /// Instance id of the node with the output connector
        /// </summary>
        public Guid OutputNode { get; set; }

        /// <summary>
        /// connector id of the output connector
        /// </summary>
        public Guid OutputConnector { get; set; }

        /// <summary>
        /// Instance id of the node with the input connector
        /// </summary>
        public Guid InputNode { get; set; }

        /// <summary>
        /// connector id of the input connector
        /// </summary>
        public Guid InputConnector { get; set; }
    }
}
