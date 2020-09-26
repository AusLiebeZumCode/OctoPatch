using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;
using Xunit;

namespace OctoPatch.Test
{
    /// <summary>
    /// Collection of tests around a patch
    /// </summary>
    public sealed class PatchTest
    {
        /// <summary>
        /// default plugin guid
        /// </summary>
        private static readonly Guid DefaultPluginGuid = Guid.Parse("{91AAD0E2-6BE1-496F-A71E-6A6DCC0376B2}");

        #region AddNode

        /// <summary>
        /// AddNode with null parameter
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task AddNode_NullParameter_Exception()
        {
            // Setup
            var patch = new Patch();

            // Act
            return Assert.ThrowsAsync<ArgumentNullException>(() => patch.AddNode(null, CancellationToken.None));
        }

        /// <summary>
        /// AddNode with exception in event handler - ignored but logged
        /// </summary>
        [Fact]
        public async Task AddNode_ExceptionInEventHandler_Success()
        {
            // Setup
            var nodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var patch = new Patch();
            patch.NodeAdded += node => throw new ArgumentException();

            // Act
            var input = new CommonTestNode(nodeId);
            await patch.AddNode(input, CancellationToken.None);
        }

        /// <summary>
        /// AddNode with valid node
        /// </summary>
        [Fact]
        public async Task AddNode_Valid_Success()
        {
            // Setup
            var count = 0;
            var nodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            INode output = null;
            var patch = new Patch();
            patch.NodeAdded += node =>
            {
                count++;
                output = node;
            };

            // Act
            var input = new CommonTestNode(nodeId);
            await patch.AddNode(input, CancellationToken.None);

            // Assert
            Assert.Equal(1, count);
            Assert.NotNull(output);
            Assert.Equal(nodeId, output.Id);
        }

        /// <summary>
        /// Adds the same node two times
        /// </summary>
        [Fact]
        public async Task AddNode_SameTwoTimes_Exception()
        {
            // Setup
            var count = 0;
            var nodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            INode output = null;
            var patch = new Patch();
            patch.NodeAdded += node =>
            {
                count++;
                output = node;
            };

            // Act
            var input = new CommonTestNode(nodeId);
            await patch.AddNode(input, CancellationToken.None);
            await Assert.ThrowsAsync<ArgumentException>(() => patch.AddNode(input, CancellationToken.None));

            // Assert
            Assert.Equal(1, count);
            Assert.NotNull(output);
            Assert.Equal(nodeId, output.Id);

        }

        /// <summary>
        /// Adds an attached node without parent
        /// </summary>
        [Fact]
        public async Task AddNode_AttachedNode_NoParent_Exception()
        {
            // Setup
            var count = 0;
            var commonNodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var attachedNodeId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            INode output = null;
            var patch = new Patch();
            patch.NodeAdded += node =>
            {
                count++;
                output = node;
            };

            // Act
            var commonInput = new CommonTestNode(commonNodeId);
            var attachedInput = new AttachedTestNode(attachedNodeId, commonInput);
            await Assert.ThrowsAsync<ArgumentException>(() => patch.AddNode(attachedInput, CancellationToken.None));

            // Assert
            Assert.Equal(0, count);
            Assert.Null(output);
        }

        /// <summary>
        /// Adds an attached node with parent
        /// </summary>
        [Fact]
        public async Task AddNode_AttachedNode_Parent_Success()
        {
            // Setup
            var count = 0;
            var commonNodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var attachedNodeId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            INode output = null;
            var patch = new Patch();
            patch.NodeAdded += node =>
            {
                count++;
                output = node;
            };

            // Act
            var commonInput = new CommonTestNode(commonNodeId);
            var attachedInput = new AttachedTestNode(attachedNodeId, commonInput);
            await patch.AddNode(commonInput, CancellationToken.None);
            await patch.AddNode(attachedInput, CancellationToken.None);

            // Assert
            Assert.Equal(2, count);
            Assert.NotNull(output);
            Assert.Equal(attachedNodeId, output.Id);
        }

        /// <summary>
        /// Adds a splitter where parent is not available
        /// </summary>
        [Fact]
        public async Task AddNode_SplitterNode_NoParent_Exception()
        {
            // Setup
            var count = 0;
            var commonNodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var splitterNodeId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            INode output = null;
            var patch = new Patch();
            patch.NodeAdded += node =>
            {
                count++;
                output = node;
            };

            // Act
            var commonInput = new CommonTestNode(commonNodeId);
            var commonConnector = commonInput.Outputs.First();
            var attachedInput = new SplitterNode(splitterNodeId, TestType.Description, commonConnector);
            await Assert.ThrowsAsync<ArgumentException>(() => patch.AddNode(attachedInput, CancellationToken.None));

            // Assert
            Assert.Equal(0, count);
            Assert.Null(output);
        }

        /// <summary>
        /// Adds a splitter where parent with existing parent
        /// </summary>
        [Fact]
        public async Task AddNode_SplitterNode_Parent_Success()
        {
            // Setup
            var count = 0;
            var commonNodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var splitterNodeId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            INode output = null;
            var patch = new Patch();
            patch.NodeAdded += node =>
            {
                count++;
                output = node;
            };

            // Act
            var commonInput = new CommonTestNode(commonNodeId);
            var commonConnector = commonInput.Outputs.First();
            var attachedInput = new SplitterNode(splitterNodeId, TestType.Description, commonConnector);
            await patch.AddNode(commonInput, CancellationToken.None);
            await patch.AddNode(attachedInput, CancellationToken.None);

            // Assert
            Assert.Equal(2, count);
            Assert.NotNull(output);
            Assert.Equal(splitterNodeId, output.Id);
        }

         /// <summary>
        /// Adds a collector where parent is not available
        /// </summary>
        [Fact]
        public async Task AddNode_CollectorNode_NoParent_Exception()
        {
            // Setup
            var count = 0;
            var commonNodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var collectorNodeId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            INode output = null;
            var patch = new Patch();
            patch.NodeAdded += node =>
            {
                count++;
                output = node;
            };

            // Act
            var commonInput = new CommonTestNode(commonNodeId);
            var commonConnector = commonInput.Inputs.First();
            var attachedInput = new CollectorNode(collectorNodeId, TestType.Description, commonConnector);
            await Assert.ThrowsAsync<ArgumentException>(() => patch.AddNode(attachedInput, CancellationToken.None));

            // Assert
            Assert.Equal(0, count);
            Assert.Null(output);
        }

        /// <summary>
        /// Adds a collector where parent with existing parent
        /// </summary>
        [Fact]
        public async Task AddNode_CollectorNode_Parent_Success()
        {
            // Setup
            var count = 0;
            var commonNodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var collectorNodeId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            INode output = null;
            var patch = new Patch();
            patch.NodeAdded += node =>
            {
                count++;
                output = node;
            };

            // Act
            var commonInput = new CommonTestNode(commonNodeId);
            var commonConnector = commonInput.Inputs.First();
            var attachedInput = new CollectorNode(collectorNodeId, TestType.Description, commonConnector);
            await patch.AddNode(commonInput, CancellationToken.None);
            await patch.AddNode(attachedInput, CancellationToken.None);

            // Assert
            Assert.Equal(2, count);
            Assert.NotNull(output);
            Assert.Equal(collectorNodeId, output.Id);
        }

        #endregion

        #region AddWire

        /// <summary>
        /// Adds wire with empty parameter
        /// </summary>
        [Fact]
        public Task AddWire_EmptyParameter_Exception()
        {
            // Setup
            var patch = new Patch();
            
            // Act
            return Assert.ThrowsAsync<ArgumentNullException>(() => patch.AddWire(null, CancellationToken.None));
        }

        /// <summary>
        /// Adds a wire but gets exception in event handler. Ignore but log
        /// </summary>
        [Fact]
        public async Task AddWire_ExceptionInEventHandler_Success()
        {
            // Setup
            var nodeId1 = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var nodeId2 = Guid.Parse("{7B48BA51-92B1-48E4-8B9B-76314A0396C4}");
            var wireId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            var patch = new Patch();
            patch.WireAdded += w => throw new ArgumentException();

            var commonNode1 = new CommonTestNode(nodeId1);
            var commonNode1Output = commonNode1.Outputs.First();
            var commonNode2 = new CommonTestNode(nodeId2);
            var commonNode2Input = commonNode2.Inputs.First();
            var wire = new Wire(wireId, commonNode2Input, commonNode1Output);

            // Act
            await patch.AddNode(commonNode1, CancellationToken.None);
            await patch.AddNode(commonNode2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);
        }

        /// <summary>
        /// Adds a wire with the missing input node
        /// </summary>
        [Fact]
        public async Task AddWire_InputNodeMissing_Exception()
        {
            // Setup
            IWire output = null;
            var nodeId1 = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var nodeId2 = Guid.Parse("{7B48BA51-92B1-48E4-8B9B-76314A0396C4}");
            var wireId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            var patch = new Patch();
            patch.WireAdded += wire =>
            {
                output = wire;
            };

            var commonNode1 = new CommonTestNode(nodeId1);
            var commonNode1Output = commonNode1.Outputs.First();
            var commonNode2 = new CommonTestNode(nodeId2);
            var commonNode2Input = commonNode2.Inputs.First();
            var wire = new Wire(wireId, commonNode2Input, commonNode1Output);

            // Act
            await patch.AddNode(commonNode1, CancellationToken.None);
            await Assert.ThrowsAsync<ArgumentException>(() => patch.AddWire(wire, CancellationToken.None));

            // Assert
            Assert.Null(output);
        }

        /// <summary>
        /// Adds a wire with the missing output node
        /// </summary>
        [Fact]
        public async Task AddWire_OutputNodeMissing_Exception()
        {
            // Setup
            IWire output = null;
            var nodeId1 = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var nodeId2 = Guid.Parse("{7B48BA51-92B1-48E4-8B9B-76314A0396C4}");
            var wireId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            var patch = new Patch();
            patch.WireAdded += wire =>
            {
                output = wire;
            };

            var commonNode1 = new CommonTestNode(nodeId1);
            var commonNode1Output = commonNode1.Outputs.First();
            var commonNode2 = new CommonTestNode(nodeId2);
            var commonNode2Input = commonNode2.Inputs.First();
            var wire = new Wire(wireId, commonNode2Input, commonNode1Output);

            // Act
            await patch.AddNode(commonNode2, CancellationToken.None);
            await Assert.ThrowsAsync<ArgumentException>(() => patch.AddWire(wire, CancellationToken.None));

            // Assert
            Assert.Null(output);
        }

        /// <summary>
        /// Adds a wire
        /// </summary>
        [Fact]
        public async Task AddWire_EverythingFine_Success()
        {
            // Setup
            IWire output = null;
            var nodeId1 = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var nodeId2 = Guid.Parse("{7B48BA51-92B1-48E4-8B9B-76314A0396C4}");
            var wireId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            var patch = new Patch();
            patch.WireAdded += wire =>
            {
                output = wire;
            };

            var commonNode1 = new CommonTestNode(nodeId1);
            var commonNode1Output = commonNode1.Outputs.First();
            var commonNode2 = new CommonTestNode(nodeId2);
            var commonNode2Input = commonNode2.Inputs.First();
            var wire = new Wire(wireId, commonNode2Input, commonNode1Output);

            // Act
            await patch.AddNode(commonNode1, CancellationToken.None);
            await patch.AddNode(commonNode2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);

            // Assert
            Assert.NotNull(output);
            Assert.Equal(wireId, output.Id);
        }

        /// <summary>
        /// Test to add the same wire two times
        /// </summary>
        [Fact]
        public async Task AddWire_SameTwoTimes_Exception()
        {
            // Setup
            var nodeId1 = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var nodeId2 = Guid.Parse("{7B48BA51-92B1-48E4-8B9B-76314A0396C4}");
            var wireId = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            var patch = new Patch();

            var commonNode1 = new CommonTestNode(nodeId1);
            var commonNode1Output = commonNode1.Outputs.First();
            var commonNode2 = new CommonTestNode(nodeId2);
            var commonNode2Input = commonNode2.Inputs.First();
            var wire = new Wire(wireId, commonNode2Input, commonNode1Output);

            // Act
            await patch.AddNode(commonNode1, CancellationToken.None);
            await patch.AddNode(commonNode2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);
            await Assert.ThrowsAsync<ArgumentException>(() => patch.AddWire(wire, CancellationToken.None));
        }

        /// <summary>
        /// Adds a second wire between already connected connectors
        /// </summary>
        [Fact]
        public async Task AddWire_SameConnectionTwoTimes_Exception()
        {
            // Setup
            var nodeId1 = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var nodeId2 = Guid.Parse("{7B48BA51-92B1-48E4-8B9B-76314A0396C4}");
            var wireId1 = Guid.Parse("{AE42CC22-63E7-4CD8-9591-7277FFF6248A}");
            var wireId2 = Guid.Parse("{7D7AD082-DECD-42A3-B2E7-8AA5BC26C05D}");
            var patch = new Patch();

            var commonNode1 = new CommonTestNode(nodeId1);
            var commonNode1Output = commonNode1.Outputs.First();
            var commonNode2 = new CommonTestNode(nodeId2);
            var commonNode2Input = commonNode2.Inputs.First();
            var wire1 = new Wire(wireId1, commonNode2Input, commonNode1Output);
            var wire2 = new Wire(wireId2, commonNode2Input, commonNode1Output);

            // Act
            await patch.AddNode(commonNode1, CancellationToken.None);
            await patch.AddNode(commonNode2, CancellationToken.None);
            await patch.AddWire(wire1, CancellationToken.None);
            await Assert.ThrowsAsync<ArgumentException>(() => patch.AddWire(wire2, CancellationToken.None));
        }

        #endregion

        /// <summary>
        /// Dummy implementation of a common node
        /// </summary>
        private sealed class CommonTestNode : Node<EmptyConfiguration, EmptyEnvironment>
        {
            public CommonTestNode(Guid id) : base(id)
            {
                RegisterInputConnector<TestType>(new ConnectorDescription("input", "input", "input", ComplexContentType.Create<TestType>(DefaultPluginGuid)));
                RegisterOutputConnector<TestType>(new ConnectorDescription("output", "output", "output", ComplexContentType.Create<TestType>(DefaultPluginGuid)));
            }

            protected override EmptyConfiguration DefaultConfiguration =>
                new EmptyConfiguration();
        }

        /// <summary>
        /// Dummy implementation of an attached node
        /// </summary>
        private sealed class AttachedTestNode : AttachedNode<EmptyConfiguration, EmptyEnvironment, CommonTestNode>
        {
            public AttachedTestNode(Guid nodeId, CommonTestNode parentNode)
                : base(nodeId, parentNode)
            {
            }

            protected override EmptyConfiguration DefaultConfiguration =>
                new EmptyConfiguration();
        }


        #region custom type

        /// <summary>
        /// Test structure for test purposes
        /// </summary>
        public struct TestType
        {
            public static TypeDescription Description = TypeDescription.Create<TestType>(DefaultPluginGuid, "test", "test", 
                new PropertyDescription("Value1", "Value1", "Value1", new IntegerContentType()), 
                new PropertyDescription("Value1", "Value1", "Value1", new IntegerContentType()));

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
