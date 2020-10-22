using System;
using System.Collections.Generic;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for outgoing messages
    /// </summary>
    public sealed class OutputConnector : Connector, IOutputConnector
    {
        private readonly HashSet<Subscription> _subscriptions;

        private OutputConnector(Guid nodeId, Type supportedType, ConnectorDescription description)
            : base(nodeId, supportedType, description)
        {
            // TODO: Make subscriptions threadsafe

            _subscriptions = new HashSet<Subscription>();
        }

        public void Send(Message message)
        {
            // Prevent output from sending invalid types
            if (!Description.ContentType.IsSupportedType(message.Type))
            {
                throw new NotSupportedException("message is not of the right type.");
            }

            // Normalize message content
            message = new Message(message.Type,
                    Description.ContentType.NormalizeValue(message.Content));

            foreach (var subscription in _subscriptions)
            {
                subscription.Send(message);
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
        /// Creates a new output connector for trigger type
        /// </summary>
        /// <param name="type">message type</param>
        /// <param name="nodeId">parent node id</param>
        /// <param name="description">connector description</param>
        /// <returns>new connector</returns>
        public static OutputConnector Create(Type type, Guid nodeId, ConnectorDescription description)
        {
            return new OutputConnector(nodeId, type, description);
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
                try
                {
                    _observer.OnNext(message);
                }
                catch (Exception ex)
                {
                    try
                    {
                        _observer.OnError(ex);
                    }
                    catch
                    {
                        /* We ignore exceptions during OnError handling */
                    }
                }
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
