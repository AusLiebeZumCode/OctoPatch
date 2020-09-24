﻿using System;

namespace OctoPatch
{
    /// <summary>
    /// Interface for all the node side parts of a connector
    /// </summary>
    public interface IInputConnectorHandler
    {
        /// <summary>
        /// Register handler for raw message
        /// </summary>
        /// <param name="handler">raw message handler</param>
        /// <returns>reference to connector handler</returns>
        IInputConnectorHandler HandleRaw(Action<Message> handler);

        /// <summary>
        /// Register handler for empty messages
        /// </summary>
        /// <param name="handler">delegate to handle empty messages</param>
        /// <returns>reference to connector handler</returns>
        IInputConnectorHandler Handle(Action handler);

        /// <summary>
        /// Register handler for messages of specific type
        /// </summary>
        /// <param name="handler">delegate to handle messages</param>
        /// <returns>reference to connector handler</returns>
        IInputConnectorHandler Handle<T>(Action<T> handler) where T : struct;

        /// <summary>
        /// Register handler for messages of string
        /// </summary>
        /// <param name="handler">delegate to handle messages</param>
        /// <returns>reference to connector handler</returns>
        IInputConnectorHandler HandleString(Action<string> handler);

        /// <summary>
        /// Register handler for messages of byte array
        /// </summary>
        /// <param name="handler">delegate to handle messages</param>
        /// <returns>reference to connector handler</returns>
        IInputConnectorHandler HandleBinary(Action<byte[]> handler);
    }
}
