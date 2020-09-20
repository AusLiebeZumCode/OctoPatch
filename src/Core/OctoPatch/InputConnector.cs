using System;
using System.Collections.Generic;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for incoming messages
    /// </summary>
    internal sealed class InputConnector : Connector, IInputConnector, IInputConnectorHandler
    {
        /// <summary>
        /// List of handlers
        /// </summary>
        private readonly List<IHandler> _handlers;

        public InputConnector(Guid nodeId, ConnectorDescription description) 
            : base(nodeId, description)
        {
            _handlers = new List<IHandler>();
        }

        public IInputConnectorHandler HandleRaw(Action<Message> handler)
        {
            _handlers.Add(new RawHandler(handler));
            return this;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IInputConnectorHandler Handle(Action handler)
        {
            _handlers.Add(new Handler(handler));
            return this;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IInputConnectorHandler Handle<T>(Action<T> handler) where T : struct
        {
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
