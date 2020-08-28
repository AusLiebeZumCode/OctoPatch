using System;
using System.Collections.Generic;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Meta description of a node
    /// </summary>
    public abstract class NodeDescription : CommonDescription
    {
        /// <summary>
        /// Returns the actual type of the node description
        /// </summary>
        public string NodeType => GetType().Name;

        /// <summary>
        /// Id of a node type
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// full type name of the node
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Version of the type implementation
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// List of input connectors
        /// </summary>
        public List<InputDescription> InputDescriptions { get; set; }

        /// <summary>
        /// List of output connectors
        /// </summary>
        public List<OutputDescription> OutputDescriptions { get; set; }

        protected NodeDescription(Guid guid, string name, Version version, string description = null) 
            : base(name, description)
        {
            Guid = guid;
            Version = version.ToString();
            InputDescriptions = new List<InputDescription>();
            OutputDescriptions = new List<OutputDescription>();
        }

        /// <summary>
        /// Adds an output description to the node
        /// </summary>
        /// <param name="description">description</param>
        /// <returns>current instance to chain adds</returns>
        public NodeDescription AddOutputDescription(OutputDescription description)
        {
            OutputDescriptions.Add(description);
            return this;
        }

        /// <summary>
        /// Adds an input description to the node
        /// </summary>
        /// <param name="description">description</param>
        /// <returns>current instance to chain adds</returns>
        public NodeDescription AddInputDescription(InputDescription description)
        {
            InputDescriptions.Add(description);
            return this;
        }
    }
}
