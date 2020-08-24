using System;

namespace OctoPatch.Core
{
    /// <summary>
    /// Interface for an input connector
    /// </summary>
    public interface IInputConnector : IConnector
    {
        IInputConnector HandleEmpty(Action handler);
        IInputConnector HandleBool(Action<bool> handler);
        IInputConnector HandleInteger(Action<int> handler);
        IInputConnector HandleFloat(Action<float> handler);
        IInputConnector HandleString(Action<string> handler);
        IInputConnector HandleBinary(Action<byte[]> handler);
        IInputConnector HandleComplex<T>(Action<T> handler);
    }
}
