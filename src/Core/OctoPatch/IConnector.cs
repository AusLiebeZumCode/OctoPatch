using System;

namespace OctoPatch
{
    /// <summary>
    /// Common connector interface
    /// </summary>
    public interface IConnector
    {
        /// <summary>
        /// Gets the unique id of the current connector
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Attach the given wire to the connector
        /// </summary>
        /// <param name="wire">wire</param>
        void AttachWire(IWire wire);

        /// <summary>
        /// Detach the wire from the connector
        /// </summary>
        /// <param name="wire">wire</param>
        void DetachWire(IWire wire);
    }
}
