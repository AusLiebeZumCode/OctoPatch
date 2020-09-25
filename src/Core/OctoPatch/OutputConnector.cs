using System;
using System.Collections.Generic;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for outgoing messages
    /// </summary>
    internal sealed class OutputConnector : Connector, IOutputConnector, IOutputConnectorHandler
    {
        private readonly HashSet<Subscription> _subscriptions;

        private OutputConnector(Guid nodeId, Type supportedType, ConnectorDescription description)
            : base(nodeId, supportedType, description)
        {
            // TODO: Make subscriptions threadsafe

            _subscriptions = new HashSet<Subscription>();
        }

        /// <inheritdoc />
        public void Send()
        {
            var message = Message.Create();
            InternalSend(message);
        }

        /// <inheritdoc />
        public void SendRaw(Message message)
        {
            InternalSend(message);
        }

        /// <inheritdoc />
        public void Send(string value)
        {
            var message = new Message(typeof(string),
                new StringContentType.StringContainer { Content = value });
            InternalSend(message);
        }

        /// <inheritdoc />
        public void Send(byte[] value)
        {
            // TODO: Think about cloning the array since this is a mutable type

            var message = new Message(typeof(byte[]), 
                new BinaryContentType.BinaryContainer { Content = value });
            InternalSend(message);
        }

        /// <inheritdoc />
        public void Send<T>(T value) where T : struct
        {
            var message = Message.Create(value);
            InternalSend(message);
        }

        private void InternalSend(Message message)
        {
            // Prevent output from sending invalid types
            if (Description.ContentType.IsSupportedType(message.Type))
            {
                throw new NotSupportedException("message is not of the right type.");
            }

            // Normalize message content
            var normalizedMessage = new Message(
                    message.Type,
                    Description.ContentType.NormalizeValue(message.Content));

            foreach (var subscription in _subscriptions)
            {
                subscription.Send(normalizedMessage);
            }
        }

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<Message> observer)
        {
            var subscription = new Subscription(observer);
            subscription.Disposed += SubscriptionOnDisposed;
            _subscriptions.Add(subscription);
            return subscription;
        }

        private void SubscriptionOnDisposed(Subscription subscription)
        {
            _subscriptions.Remove(subscription);
        }

        /// <inheritdoc />
        public IDisposable Subscribe<T>(Action<T> messageHandler) where T : struct
        {
            return this.Subscribe((m) =>
            {
                // Handle message if it's from given type
                if (m.Content is T specificType)
                {
                    messageHandler(specificType);
                }
            });
        }

        #region static fabrics

        /// <summary>
        /// Creates a new output connector for trigger type
        /// </summary>
        /// <param name="nodeId">parent node id</param>
        /// <param name="description">connector description</param>
        /// <returns>new connector</returns>
        public static OutputConnector Create(Guid nodeId, ConnectorDescription description)
        {
            return new OutputConnector(nodeId, typeof(void), description);
        }

        /// <summary>
        /// Creates a new output connector for the given type
        /// </summary>
        /// <typeparam name="T">message type</typeparam>
        /// <param name="nodeId">parent node id</param>
        /// <param name="description">connector description</param>
        /// <returns>new connector</returns>
        public static OutputConnector Create<T>(Guid nodeId, ConnectorDescription description)
        {
            return new OutputConnector(nodeId, typeof(T), description);
        }

        #endregion

        /// <summary>
        /// Represents a single subscription within that output
        /// </summary>
        private sealed class Subscription : IDisposable
        {
            /// <summary>
            /// Holds the related observer
            /// </summary>
            private readonly IObserver<Message> _observer;

            /// <summary>
            /// Creates a new subscription and listens to the dispose
            /// </summary>
            /// <param name="observer">reference to the observer</param>
            public Subscription(IObserver<Message> observer)
            {
                _observer = observer ?? throw new ArgumentNullException(nameof(observer));
            }

            /// <summary>
            /// Sends a single message to the observer
            /// </summary>
            /// <param name="message">message</param>
            public void Send(Message message)
            {
                _observer.OnNext(message);
            }

            /// <summary>
            /// Disposes the subscription
            /// </summary>
            public void Dispose()
            {
                _observer.OnCompleted();
                Disposed?.Invoke(this);
            }

            /// <summary>
            /// Gets a call when subscription was disposed
            /// </summary>
            public event Action<Subscription> Disposed;
        }
    }
}
