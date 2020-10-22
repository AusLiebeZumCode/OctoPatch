using System;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;
using Xunit;

namespace OctoPatch.Test
{
    /// <summary>
    /// Collection of tests around the input connector
    /// </summary>
    public sealed class InputConnectorTest
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
        /// Tests the behavior of a properly created input connector
        /// </summary>
        [Fact]
        public void ProperlyCreated()
        {
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            Assert.Equal(DefaultNodeGuid, connector.NodeId);
            Assert.Equal(DefaultDescription.Key, connector.Key);
            Assert.Equal(DefaultDescription.DisplayName, connector.Description.DisplayName);
            Assert.Equal(DefaultDescription.DisplayDescription, connector.Description.DisplayDescription);
            Assert.Equal(DefaultDescription.ContentType.Type, connector.Description.ContentType.Type);
        }

        #region Wrong parameter

        /// <summary>
        /// Test behavior on empty description
        /// </summary>
        [Fact]
        public void EmptyFabricParameter()
        {
            Assert.Throws<ArgumentNullException>(() => InputConnector.Create<bool>(DefaultNodeGuid, null));
            Assert.Throws<ArgumentNullException>(() => InputConnector.Create(DefaultNodeGuid, null));
        }

        /// <summary>
        /// Test the behavior when using a wrong type within create
        /// </summary>
        [Fact]
        public void WrongCreateType()
        {
            // Description is of type bool
            Assert.Throws<ArgumentException>(() => InputConnector.Create(DefaultNodeGuid, DefaultDescription));
            Assert.Throws<ArgumentException>(() => InputConnector.Create<int>(DefaultNodeGuid, DefaultDescription));
        }

        /// <summary>
        /// Test behavior of handler methods with null parameter
        /// </summary>
        [Fact]
        public void EmptyHandler()
        {
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            Assert.Throws<ArgumentNullException>(() => connector.Handle(null));
        }

        #endregion

        /// <summary>
        /// Checks behavior of messages without handler. Expect a successful OnNext without any effect
        /// </summary>
        [Fact]
        public void SendWithoutHandler()
        {
            // Setup
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);

            // Act
            connector.OnNext(Message.Create());
        }

        /// <summary>
        /// Checks behavior of messages with multiple handler
        /// </summary>
        [Fact]
        public void SendMultipleHandler()
        {
            // Setup
            var counter = 0;
            var connector = InputConnector.Create(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new EmptyContentType()));
            connector.Handle((m) => { counter++; });
            connector.Handle((m) => { counter++; });

            // Act
            connector.OnNext(Message.Create());

            // Assert
            Assert.Equal(2, counter);
        }

        /// <summary>
        /// Test behavior when content and type in message does not match
        /// Except not to get a handler call
        /// </summary>
        [Fact]
        public void SendMissmatchType()
        {
            // Setup
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            connector.Handle((b) => { });

            // Act
            Assert.Throws<InvalidCastException>(() => connector.OnNext(new Message(typeof(bool), 42)));
        }

        /// <summary>
        /// Test behavior when exception throws in handler
        /// </summary>
        [Fact]
        public void SendExceptionInHandler()
        {
            // Setup
            var triggered = false;
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            connector.Handle((b) =>
            {
                triggered = true;
                throw new ArgumentException();
            });

            // Act
            connector.OnNext(new Message(typeof(bool), true));

            // Assert
            Assert.True(triggered);
        }

        /// <summary>
        /// Check proper raw handler
        /// </summary>
        [Fact]
        public void RegisterRawHandler()
        {
            // Setup
            var output = new Message(typeof(int), 0);
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            connector.Handle((m) =>
            {
                output = m;
            });

            // Act
            connector.OnNext(new Message(typeof(string), new StringContentType.StringContainer { Content = "test" }));

            // Assert
            Assert.Equal(typeof(string), output.Type);
            Assert.Equal("test", output.Content.ToString());
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
