using System;
using OctoPatch.Setup;

namespace OctoPatch
{
    /// <summary>
    /// Interface with runtime event callback translations
    /// </summary>
    public interface IRuntimeCallbacks
    {
        /// <summary>
        /// Gets a call when there is a new node
        /// </summary>
        /// <param name="setup">node setup</param>
        /// <param name="state">current state of the new node</param>
        /// <param name="environment">serialized environment of this node</param>
        void OnNodeAdded(NodeSetup setup, NodeState state, string environment);

        /// <summary>
        /// Gets a call when something changes within the node instance
        /// </summary>
        /// <param name="instance">related node instance</param>
        void OnNodeUpdated(NodeSetup instance);

        /// <summary>
        /// Gets a call when a node state changed during runtime
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="state">new state</param>
        void OnNodeStateChanged(Guid nodeId, NodeState state);

        /// <summary>
        /// Gets a call when a node environment changed during runtime
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="environment">serialized environment</param>
        void OnNodeEnvironmentChanged(Guid nodeId, string environment);

        /// <summary>
        /// Gets a call when a node gets lost
        /// </summary>
        /// <param name="instanceGuid">node guid</param>
        void OnNodeRemoved(Guid instanceGuid);

        /// <summary>
        /// Gets a call when there is a new wire
        /// </summary>
        /// <param name="instance">wire instance</param>
        void OnWireAdded(WireSetup instance);

        /// <summary>
        /// Gets a call when a wire gets lost
        /// </summary>
        /// <param name="instanceGuid">wire guid</param>
        void OnWireRemoved(Guid instanceGuid);
    }
}
