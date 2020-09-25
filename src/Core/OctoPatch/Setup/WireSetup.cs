using System;
using System.Runtime.Serialization;

namespace OctoPatch.Setup
{
    /// <summary>
    /// Represents a wire between an input- and an output-connector
    /// </summary>
    [DataContract]
    public sealed class WireSetup
    {
        /// <summary>
        /// Gets the wire id
        /// </summary>
        [DataMember]
        public Guid WireId { get; set; }

        /// <summary>
        /// Instance id of the node with the output connector
        /// </summary>
        [DataMember]
        public Guid OutputNodeId { get; set; }

        /// <summary>
        /// connector key of the output connector
        /// </summary>
        [DataMember]
        public string OutputConnectorKey { get; set; }

        /// <summary>
        /// Instance id of the node with the input connector
        /// </summary>
        [DataMember]
        public Guid InputNodeId { get; set; }

        /// <summary>
        /// connector key of the input connector
        /// </summary>
        [DataMember]
        public string InputConnectorKey { get; set; }

        /// <summary>
        /// key of the adapter of this wire or null, if no adapter was set
        /// </summary>
        [DataMember]
        public string AdapterKey { get; set; }

        /// <summary>
        /// Adapter configuration
        /// </summary>
        [DataMember]
        public string AdapterConfiguration { get; set; }
    }
}
