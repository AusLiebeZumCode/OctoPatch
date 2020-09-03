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
        void NodeAdded(NodeSetup setup, NodeState state, string environment);

        /// <summary>
        /// Gets a call when something changes within the node instance
        /// </summary>
        /// <param name="instance">related node instance</param>
        void NodeUpdated(NodeSetup instance);

        /// <summary>
        /// Gets a call when a node state changed during runtime
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="state">new state</param>
        void NodeStateChanged(Guid nodeId, NodeState state);

        /// <summary>
        /// Gets a call when a node environment changed during runtime
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="environment">serialized environment</param>
        void NodeEnvironmentChanged(Guid nodeId, string environment);

        /// <summary>
        /// Gets a call when a node gets lost
        /// </summary>
        /// <param name="instanceGuid">node guid</param>
        void NodeRemoved(Guid instanceGuid);

        /// <summary>
        /// Gets a call when there is a new wire
        /// </summary>
        /// <param name="instance">wire instance</param>
        void WireAdded(WireSetup instance);

        /// <summary>
        /// Gets a call when a wire gets lost
        /// </summary>
        /// <param name="instanceGuid">wire guid</param>
        void WireRemoved(Guid instanceGuid);
    }
}
