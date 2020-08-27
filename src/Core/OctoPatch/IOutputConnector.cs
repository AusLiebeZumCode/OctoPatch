namespace OctoPatch
{
    /// <summary>
    /// Interface for an output connector
    /// </summary>
    public interface IOutputConnector : IConnector
    {
        /// <summary>
        /// Returns the description for this output
        /// </summary>
        OutputDescription OutputDescription { get; }

        void SendEmpty();

        void SendInteger(int value);

        void SendFloat(float value);

        void SendBool(bool value);

        void SendString(string value);

        void SendBinary(byte[] value);

        void SendComplex<T>(T value);
    }
}
