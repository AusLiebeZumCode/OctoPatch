using System;
using System.Collections.Generic;
using OctoPatch.Exchange;

namespace OctoPatch
{
    /// <summary>
    /// Implementation for a custom types repository
    /// </summary>
    public sealed class Repository : IRepository
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<MessageDescription> GetMessageDescriptions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<NodeDescription> GetNodeDescriptions()
        {
            throw new NotImplementedException();
        }
    }
}
