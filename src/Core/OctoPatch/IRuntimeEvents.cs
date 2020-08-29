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
        event Action<NodeSetup> OnNodeAdded;

        /// <summary>
        /// Gets a call when an existing node instance was removed
        /// </summary>
        event Action<Guid> OnNodeRemoved;

        /// <summary>
        /// Gets a call when a new wire instance was added
        /// </summary>
        event Action<WireSetup> OnWireAdded;

        /// <summary>
        /// Gets a call when an existing wire instance was removed
        /// </summary>
        event Action<Guid> OnWireRemoved;
    }
}
