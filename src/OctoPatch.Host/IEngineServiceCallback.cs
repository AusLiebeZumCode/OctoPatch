using System;
using OctoPatch.Exchange;

namespace OctoPatch.Host
{
    public interface IEngineServiceCallback
    {
        #region Engine callbacks

        /// <summary>
        /// Gets a call when the engine gets a new node
        /// </summary>
        /// <param name="instance">node instance</param>
        void NodeAdded(NodeInstance instance);

        /// <summary>
        /// Gets a call when the engine looses a node
        /// </summary>
        /// <param name="instanceGuid">node guid</param>
        void NodeRemoved(Guid instanceGuid);

        /// <summary>
        /// Gets a call when the engine gets a new wire
        /// </summary>
        /// <param name="instance">wire instance</param>
        void WireAdded(WireInstance instance);

        /// <summary>
        /// Gets a call when the engine looses a wire
        /// </summary>
        /// <param name="instanceGuid">wire guid</param>
        void WireRemoved(Guid instanceGuid);

        #endregion
    }
}
