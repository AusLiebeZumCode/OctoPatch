using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch.Client
{
    /// <summary>
    /// Represents a connection to the runtime
    /// </summary>
    public interface IRuntimeClient : IRuntimeMethods, IRuntimeEvents
    {
        /// <summary>
        /// Initializes the client with the given uri
        /// </summary>
        /// <param name="uri">server uri</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task Setup(Uri uri, CancellationToken cancellationToken);
    }
}
