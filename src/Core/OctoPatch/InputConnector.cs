using System;
using System.Collections.Generic;
using OctoPatch.ContentTypes;
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

        /// <inheritdoc />
        public IInputConnectorHandler HandleRaw(Action<Message> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.Add(new RawHandler(handler));
            return this;
        }

        /// <inheritdoc />
        public IInputConnectorHandler Handle(Action handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!ContentType.IsSupportedType(typeof(void)))
            {
                throw new NotSupportedException("connector is not a trigger connector");
            }

            _handlers.Add(new TriggerHandler(handler, ContentType));
            return this;
        }

        /// <inheritdoc />
        public IInputConnectorHandler Handle<T>(Action<T> handler) where T : struct
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.Add(new ValueTypeHandler<T>(handler, ContentType));
            return this;
        }

        /// <inheritdoc />
        public IInputConnectorHandler HandleString(Action<string> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.Add(new StringHandler(handler, ContentType));
            return this;
        }

        /// <inheritdoc />
        public IInputConnectorHandler HandleBinary(Action<byte[]> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.Add(new BinaryHandler(handler, ContentType));
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
            /// <summary>
            /// Tries to handle the incoming message
            /// </summary>
            /// <param name="message">message</param>
            void Handle(Message message);
        }

        /// <summary>
        /// Handler for trigger
        /// </summary>
        private sealed class TriggerHandler : IHandler
        {
            /// <summary>
            /// Holds the delegate
            /// </summary>
            private readonly Action _handlerDelegate;

            /// <summary>
            /// Creates a new handler for non content messages
            /// </summary>
            /// <param name="handlerDelegate">delegate to call on fitting messages</param>
            /// <param name="contentType">content type</param>
            public TriggerHandler(Action handlerDelegate, ContentType contentType)
            {
                if (contentType == null)
                {
                    throw new ArgumentNullException(nameof(contentType));
                }

                // Make sure content type fits the the given handler type
                if (!contentType.IsSupportedType(typeof(void)))
                {
                    throw new NotSupportedException("connector does not handle the requested type");
                }

                _handlerDelegate = handlerDelegate ?? throw new ArgumentNullException(nameof(handlerDelegate));
            }

            /// <inheritdoc />
            public void Handle(Message message)
            {
                if (message.Type == typeof(void))
                {
                    _handlerDelegate();
                }
            }
        }

        /// <summary>
        /// Handler for raw messages
        /// </summary>
        private sealed class RawHandler : IHandler
        {
            /// <summary>
            /// holds the delegate
            /// </summary>
            private readonly Action<Message> _handlerDelegate;

            /// <summary>
            /// Creates a new handler for non content messages
            /// </summary>
            /// <param name="handlerDelegate">delegate to call on fitting messages</param>
            public RawHandler(Action<Message> handlerDelegate)
            {
                _handlerDelegate = handlerDelegate ?? throw new ArgumentNullException(nameof(handlerDelegate));
            }

            /// <inheritdoc />
            public void Handle(Message message)
            {
                _handlerDelegate(message);
            }
        }

        /// <summary>
        /// Handler for callbacks with a value
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        private abstract class Handler<T> : IHandler
        {
            /// <summary>
            /// Reference to the content type
            /// </summary>
            protected ContentType ContentType { get; }

            /// <summary>
            /// Holds the delegate
            /// </summary>
            protected Action<T> HandlerDelegate { get; }

            /// <summary>
            /// Creates a new handler for a specific content type
            /// </summary>
            /// <param name="handlerDelegate">handler</param>
            /// <param name="contentType">content type</param>
            protected Handler(Action<T> handlerDelegate, ContentType contentType)
            {
                if (contentType == null)
                {
                    throw new ArgumentNullException(nameof(contentType));
                }

                // Make sure content type fits the the given handler type
                if (!contentType.IsSupportedType<T>())
                {
                    throw new NotSupportedException("connector does not handle the requested type");
                }

                ContentType = contentType;
                HandlerDelegate = handlerDelegate ?? throw new ArgumentNullException(nameof(handlerDelegate));
            }

            /// <inheritdoc />
            public void Handle(Message message)
            {
                // Make sure message fits the expected type
                if (!ContentType.IsSupportedType<T>())
                {
                    return;
                }

                OnHandle(message);
            }

            /// <summary>
            /// Gets a call when the given message fits to the content type
            /// </summary>
            /// <param name="message"></param>
            protected abstract void OnHandle(Message message);
        }

        /// <summary>
        /// Special handler implementation for value types (which his the common case)
        /// </summary>
        /// <typeparam name="T">type of message</typeparam>
        private sealed class ValueTypeHandler<T> : Handler<T> where T : struct
        {
            /// <summary>
            /// Creates a new handler for the given value type
            /// </summary>
            /// <param name="handlerDelegate">handler delegate</param>
            /// <param name="contentType">content type</param>
            public ValueTypeHandler(Action<T> handlerDelegate, ContentType contentType)
                : base(handlerDelegate, contentType)
            {
            }

            /// <inheritdoc />
            protected override void OnHandle(Message message)
            {
                // Normalize value by the given content type
                var normalizedValue = ContentType.NormalizeValue(message.Content);
                HandlerDelegate((T)normalizedValue);
            }
        }

        /// <summary>
        /// Handler for string messages
        /// </summary>
        private sealed class StringHandler : Handler<string>
        {
            /// <summary>
            /// Creates a new handler for string values
            /// </summary>
            /// <param name="handlerDelegate">handler delegate</param>
            /// <param name="contentType">content type</param>
            public StringHandler(Action<string> handlerDelegate, ContentType contentType)
                : base(handlerDelegate, contentType)
            {
            }

            /// <inheritdoc />
            protected override void OnHandle(Message message)
            {
                // Normalize value by the given content type
                var normalizedValue = ContentType.NormalizeValue(message.Content);

                var container = (StringContentType.StringContainer)normalizedValue;
                HandlerDelegate.Invoke(container.Content);
            }
        }

        /// <summary>
        /// Handler for binary messages
        /// </summary>
        private sealed class BinaryHandler : Handler<byte[]>
        {
            /// <summary>
            /// Creates a new handler for binary messages
            /// </summary>
            /// <param name="handlerDelegate">handler delegate</param>
            /// <param name="contentType">content type</param>
            public BinaryHandler(Action<byte[]> handlerDelegate, ContentType contentType)
                : base(handlerDelegate, contentType)
            {
            }

            /// <inheritdoc />
            protected override void OnHandle(Message message)
            {
                // Normalize value by the given content type
                var normalizedValue = ContentType.NormalizeValue(message.Content);

                // TODO: Think about cloning the array since this is a mutable type

                var container = (BinaryContentType.BinaryContainer)normalizedValue;
                HandlerDelegate.Invoke(container.Content);
            }
        }
    }
}
