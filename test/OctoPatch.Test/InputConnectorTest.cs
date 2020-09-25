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
            Assert.Throws<ArgumentNullException>(() => connector.Handle<bool>(null));
            Assert.Throws<ArgumentNullException>(() => connector.HandleRaw(null));
        }

        /// <summary>
        /// Test the behavior when register for the wrong type (match against description)
        /// </summary>
        [Fact]
        public void WrongHandlerType()
        {
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);
            Assert.Throws<NotSupportedException>(() => connector.Handle<int>((i) => { }));
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
            connector.Handle(() => { counter++; });
            connector.Handle(() => { counter++; });

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
            connector.Handle<bool>((b) => { });

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
            connector.Handle<bool>((b) =>
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
            connector.HandleRaw((m) =>
            {
                output = m;
            });

            // Act
            connector.OnNext(new Message(typeof(string), new StringContentType.StringContainer { Content = "test" }));

            // Assert
            Assert.Equal(typeof(string), output.Type);
            Assert.Equal("test", output.Content.ToString());
        }

        /// <summary>
        /// Test behavior of the trigger handler on empty content type
        /// </summary>
        [Fact]
        public void RegisterTriggerHandler_Success()
        {
            // Setup
            var triggered = false;
            var connector = InputConnector.Create(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new EmptyContentType()));
            connector.Handle(() =>
            {
                triggered = true;
            });

            // Act
            connector.OnNext(Message.Create());

            // Assert
            Assert.True(triggered);
        }

        /// <summary>
        /// Tries to register a trigger handler. But will fail
        /// </summary>
        [Fact]
        public void RegisterTriggerHandler_Exception()
        {
            // Setup
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);

            // Act
            Assert.Throws<NotSupportedException>(() => connector.Handle(() => { }));
        }

        /// <summary>
        /// Registers a string handler
        /// </summary>
        [Fact]
        public void RegisterStringHandler_Success()
        {
            // Setup
            var output = "";
            var connector = InputConnector.Create<string>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new StringContentType()));
            connector.HandleString((s) =>
            {
                output = s;
            });

            // Act
            connector.OnNext(new Message(typeof(string), new StringContentType.StringContainer("test")));

            // Assert
            Assert.Equal("test", output);
        }

        /// <summary>
        /// Registers a string handler with a normalizer
        /// </summary>
        [Fact]
        public void RegisterStringHandler_Normalizer()
        {
            // Setup
            var output = "";
            var connector = InputConnector.Create<string>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new StringContentType { MaximumLength = 2 }));
            connector.HandleString((s) =>
            {
                output = s;
            });

            // Act
            connector.OnNext(new Message(typeof(string), new StringContentType.StringContainer("test")));

            // Assert
            Assert.Equal("te", output);
        }

        /// <summary>
        /// Registers a string handler but fail
        /// </summary>
        [Fact]
        public void RegisterStringHandler_Exception()
        {
            // Setup
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);

            // Act
            Assert.Throws<NotSupportedException>(() => connector.HandleString((s) => { }));
        }

        /// <summary>
        /// Registers a binary handler
        /// </summary>
        [Fact]
        public void RegisterBinaryHandler_Success()
        {
            // Setup
            var output = new byte[0];
            var connector = InputConnector.Create<byte[]>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new BinaryContentType()));
            connector.HandleBinary((b) =>
            {
                output = b;
            });

            // Act
            connector.OnNext(new Message(typeof(byte[]), new BinaryContentType.BinaryContainer(new byte[10])));

            // Assert
            Assert.Equal(10, output.Length);
        }

        /// <summary>
        /// Registers a binary handler with a normalizer
        /// </summary>
        [Fact]
        public void RegisterBinaryHandler_Normalizer()
        {
            // Setup
            var output = new byte[0];
            var connector = InputConnector.Create<byte[]>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new BinaryContentType { MaximumLength = 4 }));
            connector.HandleBinary((b) =>
            {
                output = b;
            });

            // Act
            connector.OnNext(new Message(typeof(byte[]), new BinaryContentType.BinaryContainer(new byte[10])));

            // Assert
            Assert.Equal(4, output.Length);
        }

        /// <summary>
        /// Registers a binary handler but fail
        /// </summary>
        [Fact]
        public void RegisterBinaryHandler_Exception()
        {
            // Setup
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);

            // Act
            Assert.Throws<NotSupportedException>(() => connector.Handle(() => { }));
        }

        /// <summary>
        /// Registers an integer handler
        /// </summary>
        [Fact]
        public void RegisterGenericHandler_Success()
        {
            // Setup
            var output = 0;
            var connector = InputConnector.Create<int>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new IntegerContentType()));
            connector.Handle<int>((i) =>
            {
                output = i;
            });

            // Act
            connector.OnNext(Message.Create(42));

            // Assert
            Assert.Equal(42, output);
        }

        /// <summary>
        /// Registers an integer handler with a normalizer
        /// </summary>
        [Fact]
        public void RegisterGenericHandler_Normalizer()
        {
            // Setup
            var output = 0;
            var connector = InputConnector.Create<int>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", new IntegerContentType { MaximumValue = 10 }));
            connector.Handle<int>((i) =>
            {
                output = i;
            });

            // Act
            connector.OnNext(Message.Create(42));

            // Assert
            Assert.Equal(10, output);
        }

        /// <summary>
        /// Registers an integer handler but fails
        /// </summary>
        [Fact]
        public void RegisterGenericHandler_Exception()
        {
            // Setup
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);

            // Act
            Assert.Throws<NotSupportedException>(() => connector.Handle<int>((i) => { }));
        }

        /// <summary>
        /// Registers a complex type with success
        /// </summary>
        [Fact]
        public void RegisterComplexHandler_Success()
        {
            // Setup
            var pluginId = Guid.Parse("{F007FF2A-F237-4443-B937-218CBF6A47F0}");
            var output = new TestType { Value1 = 0, Value2 = 0 };
            var connector = InputConnector.Create<TestType>(DefaultNodeGuid, new ConnectorDescription(
                "test", "test", "test", ComplexContentType.Create<TestType>(pluginId)));
            connector.Handle<TestType>((t) =>
            {
                output = t;
            });

            // Act
            connector.OnNext(Message.Create(new TestType { Value1 = 1, Value2 = 2 }));

            // Assert
            Assert.Equal(1, output.Value1);
            Assert.Equal(2, output.Value2);
        }

        /// <summary>
        /// Registers a custom handler but fails
        /// </summary>
        [Fact]
        public void RegisterComplexHandler_Exception()
        {
            // Setup
            var connector = InputConnector.Create<bool>(DefaultNodeGuid, DefaultDescription);

            // Act
            Assert.Throws<NotSupportedException>(() => connector.Handle<TestType>((i) => { }));
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
