using System;

namespace OctoPatch
{
    /// <summary>
    /// Interface for an output connector
    /// </summary>
    public interface IOutputConnector : IConnector, IObservable<Message>
    {
    }
}
