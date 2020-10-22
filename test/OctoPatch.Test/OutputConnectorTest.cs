using System;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;
using Xunit;

namespace OctoPatch.Test
{
    /// <summary>
    /// Collection of tests around output connectors
    /// </summary>
    public sealed class OutputConnectorTest
    {
        /// <summary>
        /// default node guid
        /// </summary>
        private static readonly Guid DefaultNodeGuid = Guid.Parse("{C7929EB6-DBFF-4683-B85F-5E6753AFBA50}");

        /// <summary>
        /// default connector setup
        /// </summary>
        private static ConnectorDescription DefaultDescription => new ConnectorDescription(
            "test", "Test connection", "Test connection description", new BoolContentType());

        /// <summary>
        /// Test behavior without subscriptions
        /// </summary>
        [Fact]
        public void SendWithoutSubscriptions()
        {
            // Setup
            var connector = OutputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            
            // Act
            connector.Send(Message.Create(true));
        }

        /// <summary>
        /// Test behavior with single subscription
        /// </summary>
        [Fact]
        public void SendWithSingleSubscription()
        {
            // Setup
            var count = 0;
            var connector = OutputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            connector.Subscribe((m) => { count++; });

            // Act
            connector.Send(Message.Create(true));

            // Assert
            Assert.Equal(1, count);
        }

        /// <summary>
        /// Test behavior with multiple subscriptions
        /// </summary>
        [Fact]
        public void SendWithMultipleSubscriptions()
        {
            // Setup
            var count = 0;
            var connector = OutputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            connector.Subscribe((m) => { count++; });
            connector.Subscribe((m) => { count++; });

            // Act
            connector.Send(Message.Create(true));

            // Assert
            Assert.Equal(2, count);
        }

        /// <summary>
        /// Test behavior with exception in handler
        /// </summary>
        [Fact]
        public void SendWithExceptionInHandler()
        {
            // Setup
            var connector = OutputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            connector.Subscribe((m) => { throw new NotSupportedException(); });

            // Act
            connector.Send(Message.Create(true));
        }

        /// <summary>
        /// Test behavior of sending raw data of right type
        /// </summary>
        [Fact]
        public void Send_Raw_Success()
        {
            // Setup
            var message = new Message(typeof(void), 0);
            var connector = OutputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            connector.Subscribe((m) => { message = m; });

            // Act
            connector.Send(new Message(typeof(bool), true));

            // Assert
            Assert.Equal(typeof(bool), message.Type);
            Assert.Equal(true, message.Content);
        }

        /// <summary>
        /// Test behavior of sending raw data of wrong type
        /// </summary>
        [Fact]
        public void Send_Raw_ArgumentException()
        {
            // Setup
            var connector = OutputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);

            // Act
            Assert.Throws<NotSupportedException>(() => connector.Send(new Message(typeof(int), 2)));
        }

        #region custom type

        /// <summary>
        /// Test structure for test purposes
        /// </summary>
        public struct TestType
        {
            /// <summary>
            /// Dummy value 1
            /// </summary>
            public int Value1 { get; set; }

            /// <summary>
            /// Dummy value 2
            /// </summary>
            public int Value2 { get; set; }
        }

        #endregion
    }
}
