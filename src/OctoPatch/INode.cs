using System;
using System.Collections.Generic;
using OctoPatch.Exchange;

namespace OctoPatch
{
    /// <summary>
    /// Represents the interface of a node within the OctoPatch grid
    /// StreamNote: TheBlubb14 was here (2020-06-30 20:47)
    /// </summary>
    public interface INode : IDisposable
    {
        /// <summary>
        /// Gets the current node instance information
        /// </summary>
        NodeInstance Instance { get; }

        /// <summary>
        /// Gets the list of all inputs
        /// </summary>
        IEnumerable<IInputConnector> Inputs { get; }
        
        /// <summary>
        /// Gets the list of all outputs
        /// </summary>
        IEnumerable<IOutputConnector> Outputs { get; }

        /// <summary>
        /// Configures the current node instance
        /// </summary>
        /// <param name="instance">instance information</param>
        void Setup(NodeInstance instance);
    }
}
