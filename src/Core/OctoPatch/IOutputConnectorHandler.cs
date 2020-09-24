namespace OctoPatch
{
    /// <summary>
    /// Interface for all the node internal connector methods
    /// </summary>
    public interface IOutputConnectorHandler
    {
        /// <summary>
        /// Sends an empty message
        /// </summary>
        void Send();

        /// <summary>
        /// Sends the raw message
        /// </summary>
        /// <param name="message">raw message</param>
        void SendRaw(Message message);

        /// <summary>
        /// Sends a message of the given type
        /// </summary>
        /// <param name="value">message as string</param>
        void Send(string value);

        /// <summary>
        /// Sends a message of the given type
        /// </summary>
        /// <param name="value">message as byte[]</param>
        void Send(byte[] value);


        /// <summary>
        /// Sends a message of the given type
        /// </summary>
        /// <typeparam name="T">message type</typeparam>
        /// <param name="value">message content</param>
        void Send<T>(T value) where T : struct;
    }
}
