using System;
using OctoPatch.Setup;

namespace OctoPatch
{
    /// <summary>
    /// Interface with all the events from the runtime
    /// </summary>
    public interface IRuntimeEvents
    {
        /// <summary>
        /// Gets a call when a new node instance was added
        /// </summary>
        event Action<NodeSetup, NodeState, string> NodeAdded;

        /// <summary>
        /// Gets a call when an existing node instance was removed
        /// </summary>
        event Action<Guid> NodeRemoved;

        /// <summary>
        /// Gets a call when an existing node was updated
        /// </summary>
        event Action<NodeSetup> NodeUpdated;

        /// <summary>
        /// Gets a call when a node state changed
        /// </summary>
        event Action<Guid, NodeState> NodeStateChanged;

        /// <summary>
        /// Gets a call when a node environment changed
        /// </summary>
        event Action<Guid, string> NodeEnvironmentChanged;

        /// <summary>
        /// Gets a call when a new wire instance was added
        /// </summary>
        event Action<WireSetup> WireAdded;

        /// <summary>
        /// Gets a call when an existing wire instance was removed
        /// </summary>
        event Action<Guid> WireRemoved;
    }
}
