using System;

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
        /// <param name="instance">node instance</param>
        void NodeAdded(NodeInstance instance);

        /// <summary>
        /// Gets a call when a node gets lost
        /// </summary>
        /// <param name="instanceGuid">node guid</param>
        void NodeRemoved(Guid instanceGuid);

        /// <summary>
        /// Gets a call when there is a new wire
        /// </summary>
        /// <param name="instance">wire instance</param>
        void WireAdded(WireInstance instance);

        /// <summary>
        /// Gets a call when a wire gets lost
        /// </summary>
        /// <param name="instanceGuid">wire guid</param>
        void WireRemoved(Guid instanceGuid);
    }
}
