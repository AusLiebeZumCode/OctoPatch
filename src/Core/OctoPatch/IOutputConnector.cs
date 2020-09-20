using System;

namespace OctoPatch
{
    /// <summary>
    /// Interface for an output connector
    /// </summary>
    public interface IOutputConnector : IConnector, IObservable<Message>
    {
        /// <summary>
        /// Type specific subscription
        /// </summary>
        /// <typeparam name="T">message type</typeparam>
        /// <param name="messageHandler">message handler</param>
        /// <returns>disposable</returns>
        IDisposable Subscribe<T>(Action<T> messageHandler) where T : struct;
    }
}
