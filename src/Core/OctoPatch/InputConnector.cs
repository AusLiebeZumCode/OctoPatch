using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OctoPatch.Descriptions;
using OctoPatch.Logging;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for incoming messages
    /// </summary>
    public sealed class InputConnector : Connector, IInputConnector
    {
        /// <summary>
        /// Reference to the logger
        /// </summary>
        private static readonly ILogger<NodeDescription> Logger = LogManager.GetLogger<NodeDescription>();

        /// <summary>
        /// List of handlers
        /// </summary>
        private readonly List<Action<Message>> _handlers;

        private InputConnector(Guid nodeId, Type supportedType, ConnectorDescription description)
            : base(nodeId, supportedType, description)
        {
            _handlers = new List<Action<Message>>();
        }

        public void Handle(Action<Message> handler)
        {
            _handlers.Add(handler ?? throw new ArgumentNullException(nameof(handler)));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(Message value)
        {
            // Normalize value by the given content type
            value = new Message(value.Type, ContentType.NormalizeValue(value.Content));

            foreach (var action in _handlers)
            {
                try
                {
                    action(value);
                }
                catch (InvalidCastException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error in message handler");
                }
            }
        }

        #region static fabric

        /// <summary>
        /// Creates a new input connector for trigger messages
        /// </summary>
        /// <param name="nodeId">parent node id</param>
        /// <param name="description">connector description</param>
        /// <returns>new connector</returns>
        public static InputConnector Create(Guid nodeId, ConnectorDescription description)
        {
            return new InputConnector(nodeId, typeof(void), description);
        }

        /// <summary>
        /// Creates a new input connector with the given message type
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        /// <param name="nodeId">parent node id</param>
        /// <param name="description">connector description</param>
        /// <returns>new connector</returns>
        public static InputConnector Create<T>(Guid nodeId, ConnectorDescription description)
        {
            return new InputConnector(nodeId, typeof(T), description);
        }

        #endregion
    }
}
