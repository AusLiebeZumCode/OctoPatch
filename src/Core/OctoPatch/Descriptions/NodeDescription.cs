using System.Collections.Generic;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Meta description of a node
    /// </summary>
    public abstract class NodeDescription : KeyDescription
    {
        /// <summary>
        /// Returns the actual type of the node description
        /// </summary>
        public string NodeType => GetType().Name;

        /// <summary>
        /// List of input connectors
        /// </summary>
        public List<ConnectorDescription> InputDescriptions { get; set; }

        /// <summary>
        /// List of output connectors
        /// </summary>
        public List<ConnectorDescription> OutputDescriptions { get; set; }

        protected NodeDescription(string key, string displayName, string displayDescription) 
            : base(key, displayName, displayDescription)
        {
            InputDescriptions = new List<ConnectorDescription>();
            OutputDescriptions = new List<ConnectorDescription>();
        }

        /// <summary>
        /// Adds an output description to the node
        /// </summary>
        /// <param name="description">description</param>
        /// <returns>current instance to chain adds</returns>
        public NodeDescription AddOutputDescription(ConnectorDescription description)
        {
            OutputDescriptions.Add(description);
            return this;
        }

        /// <summary>
        /// Adds an input description to the node
        /// </summary>
        /// <param name="description">description</param>
        /// <returns>current instance to chain adds</returns>
        public NodeDescription AddInputDescription(ConnectorDescription description)
        {
            InputDescriptions.Add(description);
            return this;
        }
    }
}
