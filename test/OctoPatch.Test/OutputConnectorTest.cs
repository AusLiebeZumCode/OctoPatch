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
            connector.SendRaw(Message.Create(true));
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
            connector.SendRaw(Message.Create(true));

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
            connector.SendRaw(Message.Create(true));

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
            connector.SendRaw(Message.Create(true));
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
            connector.SendRaw(new Message(typeof(bool), true));

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
            Assert.Throws<NotSupportedException>(() => connector.SendRaw(new Message(typeof(int), 2)));
        }

        /// <summary>
        /// Test send method of trigger messages
        /// </summary>
        [Fact]
        public void Send_Trigger_Success()
        {
            // Setup
            var message = new Message(typeof(int), 55);
            var connector = OutputConnector.Create(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new EmptyContentType()));
            connector.Subscribe((m) => { message = m; });

            // Act
            connector.Send();

            // Assert
            Assert.Equal(typeof(void), message.Type);
        }

        /// <summary>
        /// Test send method of string messages
        /// </summary>
        [Fact]
        public void Send_String_Success()
        {
            // Setup
            var message = new Message(typeof(void), 0);
            var connector = OutputConnector.Create<string>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new StringContentType()));
            connector.Subscribe((m) => { message = m; });

            // Act
            connector.Send("test");

            // Assert
            Assert.Equal(typeof(string), message.Type);
            Assert.Equal(typeof(StringContentType.StringContainer), message.Content.GetType());
            Assert.Equal("test", ((StringContentType.StringContainer)message.Content).Content);
        }

        /// <summary>
        /// Test send method of string messages
        /// </summary>
        [Fact]
        public void Send_String_Normalized()
        {
            // Setup
            var message = new Message(typeof(void), 0);
            var connector = OutputConnector.Create<string>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new StringContentType { MaximumLength = 2 }));
            connector.Subscribe((m) => { message = m; });

            // Act
            connector.Send("test");

            // Assert
            Assert.Equal(typeof(string), message.Type);
            Assert.Equal(typeof(StringContentType.StringContainer), message.Content.GetType());
            Assert.Equal("te", ((StringContentType.StringContainer)message.Content).Content);
        }

        /// <summary>
        /// Test send method of binary message
        /// </summary>
        [Fact]
        public void Send_Binary_Success()
        {
            // Setup
            var message = new Message(typeof(void), 0);
            var connector = OutputConnector.Create<byte[]>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new BinaryContentType()));
            connector.Subscribe((m) => { message = m; });

            // Act
            connector.Send(new byte[10]);

            // Assert
            Assert.Equal(typeof(byte[]), message.Type);
            Assert.Equal(typeof(BinaryContentType.BinaryContainer), message.Content.GetType());
            Assert.Equal(10, ((BinaryContentType.BinaryContainer)message.Content).Content.Length);
        }

        /// <summary>
        /// Test send method of binary message
        /// </summary>
        [Fact]
        public void Send_Binary_Normalized()
        {
            // Setup
            var message = new Message(typeof(void), 0);
            var connector = OutputConnector.Create<byte[]>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new BinaryContentType { MaximumLength = 4 }));
            connector.Subscribe((m) => { message = m; });

            // Act
            connector.Send(new byte[10]);

            // Assert
            Assert.Equal(typeof(byte[]), message.Type);
            Assert.Equal(typeof(BinaryContentType.BinaryContainer), message.Content.GetType());
            Assert.Equal(4, ((BinaryContentType.BinaryContainer)message.Content).Content.Length);
        }

        /// <summary>
        /// Test send method of generic messages (int)
        /// </summary>
        [Fact]
        public void Send_Generic_Success()
        {
            // Setup
            var message = new Message(typeof(void), 0);
            var connector = OutputConnector.Create<int>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new IntegerContentType()));
            connector.Subscribe((m) => { message = m; });

            // Act
            connector.Send(42);

            // Assert
            Assert.Equal(typeof(int), message.Type);
            Assert.Equal(42, message.Content);
        }

        /// <summary>
        /// Test send method of generic messages (int)
        /// </summary>
        [Fact]
        public void Send_Generic_Normalized()
        {
            // Setup
            var message = new Message(typeof(void), 0);
            var connector = OutputConnector.Create<int>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new IntegerContentType { MaximumValue = 10 }));
            connector.Subscribe((m) => { message = m; });

            // Act
            connector.Send(42);

            // Assert
            Assert.Equal(typeof(int), message.Type);
            Assert.Equal(10, message.Content);
        }

        /// <summary>
        /// Test behavior of sending complex types
        /// </summary>
        [Fact]
        public void Send_Complex_Success()
        {
            // Setup
            var pluginId = Guid.Parse("{F007FF2A-F237-4443-B937-218CBF6A47F0}");
            var message = new Message(typeof(void), 0);
            var connector = OutputConnector.Create<TestType>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", ComplexContentType.Create<TestType>(pluginId)));
            connector.Subscribe((m) => { message = m; });

            // Act
            connector.Send(new TestType { Value1 = 1, Value2 = 2 });

            // Assert
            Assert.Equal(typeof(TestType), message.Type);
            Assert.Equal(1, ((TestType)message.Content).Value1);
            Assert.Equal(2, ((TestType)message.Content).Value2);
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
