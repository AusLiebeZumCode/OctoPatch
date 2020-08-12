using OctoPatch.Communication;
using System;

namespace OctoPatch.DesktopClient
{
    /// <summary>
    /// Represents a connection to the engine host
    /// </summary>
    public interface IEngineServiceClient : IEngineService
    {
        /// <summary>
        /// Gets a call when a new node instance was added
        /// </summary>
        event Action<NodeInstance> OnNodeAdded;

        /// <summary>
        /// Gets a call when an existing node instance was removed
        /// </summary>
        event Action<Guid> OnNodeRemoved;

        /// <summary>
        /// Gets a call when a new wire instance was added
        /// </summary>
        event Action<WireInstance> OnWireAdded;

        /// <summary>
        /// Gets a call when an existing wire instance was removed
        /// </summary>
        event Action<Guid> OnWireRemoved;
    }
}
