using System;

namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of connectors
    /// </summary>
    public abstract class Connector : IConnector
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Guid Guid { get; }

        protected Connector(Guid guid)
        {
            Guid = guid;
        }
    }
}
