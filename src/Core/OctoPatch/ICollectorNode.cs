﻿namespace OctoPatch
{
    /// <summary>
    /// Base interface for all nodes which collect data for a complex type
    /// </summary>
    public interface ICollectorNode : INode
    {
        /// <summary>
        /// Reference to the related connector
        /// </summary>
        IInputConnector Connector { get; }
    }
}
