using System;
using System.Collections.Generic;
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Send()
        {
            var message = Message.Create();
            InternalSend(message);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void SendRaw(Message message)
        {
            InternalSend(message);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Send<T>(T value) where T : struct
        {
            var message = Message.Create(value);
            InternalSend(message);
        }

        private void InternalSend(Message message)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Send(message);
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
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

        private sealed class Subscription : IDisposable
        {
            private readonly IObserver<Message> _observer;

            public Subscription(IObserver<Message> observer)
            {
                _observer = observer;
            }

            public void Send(Message message)
            {
                _observer.OnNext(message);
            }

            public void Dispose()
            {
                Disposed?.Invoke(this);
            }

            public event Action<Subscription> Disposed;
        }
    }
}
