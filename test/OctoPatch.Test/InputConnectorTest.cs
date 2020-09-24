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
            "test", "Test connection","Test connection description", new BoolContentType());

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
    }
}
