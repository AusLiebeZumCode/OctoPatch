using System;

namespace OctoPatch
{
    /// <summary>
    /// Interface for an input connector
    /// </summary>
    public interface IInputConnector : IConnector, IObserver<Message>
    {
    }
}
