using System;
using System.Collections.Generic;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for incoming messages
    /// </summary>
    public sealed class InputConnector : Connector, IInputConnector, IInputConnectorHandler
    {
        /// <summary>
        /// List of handlers
        /// </summary>
        private readonly List<IHandler> _handlers;

        private InputConnector(Guid nodeId, Type supportedType, ConnectorDescription description) 
            : base(nodeId, supportedType, description)
        {
            _handlers = new List<IHandler>();
        }

        public IInputConnectorHandler HandleRaw(Action<Message> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.Add(new RawHandler(handler));
            return this;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IInputConnectorHandler Handle(Action handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (SupportedType != typeof(void))
            {
                throw new NotSupportedException("connector is not a trigger connector");
            }

            _handlers.Add(new Handler(handler));
            return this;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IInputConnectorHandler Handle<T>(Action<T> handler) where T : struct
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (SupportedType != typeof(T))
            {
                throw new NotSupportedException("connector does not handle the requested type");
            }

            _handlers.Add(new Handler<T>(handler));
            return this;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(Message value)
        {
            foreach (var handler in _handlers)
            {
                handler.Handle(value);
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

        /// <summary>
        /// Common interface for a handler
        /// </summary>
        private interface IHandler
        {
            void Handle(Message message);
        }

        /// <summary>
        /// Handler for raw messages
        /// </summary>
        private sealed class RawHandler : IHandler
        {
            private readonly Action<Message> _handler;

            public RawHandler(Action<Message> handler)
            {
                _handler = handler;
            }

            public void Handle(Message message)
            {
                _handler(message);
            }
        }

        /// <summary>
        /// Handler for trigger
        /// </summary>
        private sealed class Handler : IHandler
        {
            private readonly Action _handler;

            public Handler(Action handler)
            {
                _handler = handler;
            }

            public void Handle(Message message)
            {
                if (message.Type == typeof(void))
                {
                    _handler();
                }
            }
        }

        /// <summary>
        /// Handler for generic handler
        /// </summary>
        /// <typeparam name="T">message type</typeparam>
        private sealed class Handler<T> : IHandler where T : struct
        {
            private readonly Action<T> _handler;

            public Handler(Action<T> handler)
            {
                _handler = handler;
            }

            public void Handle(Message message)
            {
                if (message.Type == typeof(T))
                {
                    _handler((T) message.Content);
                }
            }
        }
    }
}
