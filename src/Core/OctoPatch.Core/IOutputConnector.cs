namespace OctoPatch.Core
{
    /// <summary>
    /// Interface for an output connector
    /// </summary>
    public interface IOutputConnector : IConnector
    {
        void SendEmpty();

        void SendInteger(int value);

        void SendFloat(float value);

        void SendBool(bool value);

        void SendString(string value);

        void SendBinary(byte[] value);

        void SendComplex<T>(T value);
    }
}
