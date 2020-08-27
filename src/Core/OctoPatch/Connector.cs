using System;

namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of connectors
    /// </summary>
    internal abstract class Connector : IConnector
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Guid Guid { get; }

        protected Connector(Guid guid)
        {
            Guid = guid;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public abstract void AttachWire(IWire wire);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public abstract void DetachWire(IWire wire);
    }
}
