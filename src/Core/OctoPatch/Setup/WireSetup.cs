using System;

namespace OctoPatch.Setup
{
    /// <summary>
    /// Represents a wire between an input- and an output-connector
    /// </summary>
    public sealed class WireSetup
    {
        /// <summary>
        /// Gets the wire id
        /// </summary>
        public Guid WireId { get; set; }

        /// <summary>
        /// Instance id of the node with the output connector
        /// </summary>
        public Guid OutputNodeId { get; set; }

        /// <summary>
        /// connector key of the output connector
        /// </summary>
        public string OutputConnectorKey { get; set; }

        /// <summary>
        /// Instance id of the node with the input connector
        /// </summary>
        public Guid InputNodeId { get; set; }

        /// <summary>
        /// connector key of the input connector
        /// </summary>
        public string InputConnectorKey { get; set; }
    }
}
