﻿namespace OctoPatch
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
        /// Sends a message of the given type
        /// </summary>
        /// <typeparam name="T">message type</typeparam>
        /// <param name="value">message content</param>
        void Send<T>(T value);
    }
}
