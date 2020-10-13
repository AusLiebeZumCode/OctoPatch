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

        #region RemoveNode

        /// <summary>
        /// Tries to remove a node with Guid.Empty. Should result in false
        /// </summary>
        [Fact]
        public async Task RemoveNode_EmptyGuid_False()
        {
            // Setup
            var count = 0;
            var patch = new Patch();
            patch.NodeRemoved += node =>
            {
                count++;
            };

            // Act
            var success = await patch.RemoveNode(Guid.Empty, CancellationToken.None);

            // Assert
            Assert.False(success);
            Assert.Equal(0, count);
        }

        /// <summary>
        /// Tries to remove a not existing node (random guid)
        /// </summary>
        [Fact]
        public async Task RemoveNode_NotExist_False()
        {
            // Setup
            var count = 0;
            var nodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var patch = new Patch();
            patch.NodeRemoved += node =>
            {
                count++;
            };

            // Act
            var success = await patch.RemoveNode(nodeId, CancellationToken.None);

            // Assert
            Assert.False(success);
            Assert.Equal(0, count);
        }

        /// <summary>
        /// Tries to remove a node which is referenced by an attached node
        /// </summary>
        [Fact]
        public async Task RemoveNode_Referenced_Exception()
        {
            // Setup
            var count = 0;
            var commonNodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var attachedNodeId = Guid.Parse("{D9E2113A-98F3-4278-A2A4-B149B2B34161}");
            var commonInput = new CommonTestNode(commonNodeId);
            var attachedInput = new AttachedTestNode(attachedNodeId, commonInput);
            var patch = new Patch();
            patch.NodeRemoved += node =>
            {
                count++;
            };
            await patch.AddNode(commonInput, CancellationToken.None);
            await patch.AddNode(attachedInput, CancellationToken.None);

            // Act
            await Assert.ThrowsAsync<ArgumentException>(() => patch.RemoveNode(commonNodeId, CancellationToken.None));

            // Assert
            Assert.Equal(0, count);
        }

        /// <summary>
        /// Removes an existing node
        /// </summary>
        [Fact]
        public async Task RemoveNode_Exist_Success()
        {
            // Setup
            var count = 0;
            var nodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var input = new CommonTestNode(nodeId);
            INode output = null;
            var patch = new Patch();
            patch.NodeRemoved += node =>
            {
                count++;
                output = node;
            };
            await patch.AddNode(input, CancellationToken.None);

            // Act
            var success = await patch.RemoveNode(nodeId, CancellationToken.None);

            // Assert
            Assert.True(success);
            Assert.Equal(1, count);
            Assert.Equal(input, output);
        }

        /// <summary>
        /// Removes an existing node with exception during deinitialize
        /// </summary>
        [Fact]
        public async Task RemoveNode_DeinitialzeException_Success()
        {
            // Setup
            var count = 0;
            var nodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var input = new CommonTestNode(nodeId);
            input.OnDeinitializeException = new NotSupportedException();
            INode output = null;
            var patch = new Patch();
            patch.NodeRemoved += node =>
            {
                count++;
                output = node;
            };
            await patch.AddNode(input, CancellationToken.None);

            // Act
            var success = await patch.RemoveNode(nodeId, CancellationToken.None);

            // Assert
            Assert.True(success);
            Assert.Equal(1, count);
            Assert.Equal(input, output);
        }

        /// <summary>
        /// Removes an existing node with exception during dispose
        /// </summary>
        [Fact]
        public async Task RemoveNode_DisposeException_Success()
        {
            // Setup
            var count = 0;
            var nodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var input = new CommonTestNode(nodeId);
            input.OnDisposeException = new NotSupportedException();
            INode output = null;
            var patch = new Patch();
            patch.NodeRemoved += node =>
            {
                count++;
                output = node;
            };
            await patch.AddNode(input, CancellationToken.None);

            // Act
            var success = await patch.RemoveNode(nodeId, CancellationToken.None);

            // Assert
            Assert.True(success);
            Assert.Equal(1, count);
            Assert.Equal(input, output);
        }

        /// <summary>
        /// Removes an existing node with exception during event handler
        /// </summary>
        [Fact]
        public async Task RemoveNode_EventHandlerException_Success()
        {
            // Setup
            var count = 0;
            var nodeId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var input = new CommonTestNode(nodeId);
            INode output = null;
            var patch = new Patch();
            patch.NodeRemoved += node =>
            {
                count++;
                output = node;
                throw new NotSupportedException();
            };
            await patch.AddNode(input, CancellationToken.None);

            // Act
            var success = await patch.RemoveNode(nodeId, CancellationToken.None);

            // Assert
            Assert.True(success);
            Assert.Equal(1, count);
            Assert.Equal(input, output);
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
            var wire = new Wire(wireId, commonNode1Output, commonNode2Input);

            // Act
            await patch.AddNode(commonNode1, CancellationToken.None);
            await patch.AddNode(commonNode2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);
        }

        /// <summary>
        /// Tests to add a wire with a missing connector
        /// </summary>
        [Fact]
        public async Task AddWire_EmptyInputConnector_Exception()
        {
            // Setup
            var patch = new Patch();
            var node = new CommonTestNode(Guid.Parse("{7B48BA51-92B1-48E4-8B9B-76314A0396C4}"));
            var wire = new TestWire
            {
                Id = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}"),
                Input = null,
                Output = node.Inputs.First()
            };

            // Act
            await patch.AddNode(node, CancellationToken.None);
            await Assert.ThrowsAsync<ArgumentNullException>(() => patch.AddWire(wire, CancellationToken.None));
        }

        /// <summary>
        /// Tests to add a wire with a missing connector
        /// </summary>
        [Fact]
        public async Task AddWire_EmptyOutputConnector_Exception()
        {
            // Setup
            var patch = new Patch();
            var node = new CommonTestNode(Guid.Parse("{7B48BA51-92B1-48E4-8B9B-76314A0396C4}"));
            var wire = new TestWire
            {
                Id = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}"),
                Input = node.Outputs.First(),
                Output = null
            };

            // Act
            await patch.AddNode(node, CancellationToken.None);
            await Assert.ThrowsAsync<ArgumentNullException>(() => patch.AddWire(wire, CancellationToken.None));
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
            patch.WireAdded += w =>
            {
                output = w;
            };

            var commonNode1 = new CommonTestNode(nodeId1);
            var commonNode1Output = commonNode1.Outputs.First();
            var commonNode2 = new CommonTestNode(nodeId2);
            var commonNode2Input = commonNode2.Inputs.First();
            var wire = new Wire(wireId, commonNode1Output, commonNode2Input);

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
            patch.WireAdded += w =>
            {
                output = w;
            };

            var commonNode1 = new CommonTestNode(nodeId1);
            var commonNode1Output = commonNode1.Outputs.First();
            var commonNode2 = new CommonTestNode(nodeId2);
            var commonNode2Input = commonNode2.Inputs.First();
            var wire = new Wire(wireId, commonNode1Output, commonNode2Input);

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
            patch.WireAdded += w =>
            {
                output = w;
            };

            var commonNode1 = new CommonTestNode(nodeId1);
            var commonNode1Output = commonNode1.Outputs.First();
            var commonNode2 = new CommonTestNode(nodeId2);
            var commonNode2Input = commonNode2.Inputs.First();
            var wire = new Wire(wireId, commonNode1Output, commonNode2Input);

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
            var wire = new Wire(wireId, commonNode1Output, commonNode2Input);

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
            var wire1 = new Wire(wireId1, commonNode1Output, commonNode2Input);
            var wire2 = new Wire(wireId2, commonNode1Output, commonNode2Input);

            // Act
            await patch.AddNode(commonNode1, CancellationToken.None);
            await patch.AddNode(commonNode2, CancellationToken.None);
            await patch.AddWire(wire1, CancellationToken.None);
            await Assert.ThrowsAsync<ArgumentException>(() => patch.AddWire(wire2, CancellationToken.None));
        }

        #endregion

        #region RemoveWire

        /// <summary>
        /// Tries to remove a wire with guid empty.
        /// </summary>
        [Fact]
        public async Task RemoveWire_EmptyGuid_False()
        {
            // Setup
            var wireCount = 0;
            var patch = new Patch();
            patch.WireRemoved += wire =>
            {
                wireCount++;
            };

            // Act
            var success = await patch.RemoveWire(Guid.Empty, CancellationToken.None);

            // Assert
            Assert.False(success);
            Assert.Equal(0, wireCount);
        }

        /// <summary>
        /// Tries to remove a not existing wire
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RemoveWire_NotExist_False()
        {
            // Setup
            var wireCount = 0;
            var wireId = Guid.Parse("{54E66F6B-7051-40FA-BDDC-F70EEB37D988}");
            var patch = new Patch();
            patch.WireRemoved += wire =>
            {
                wireCount++;
            };

            // Act
            var success = await patch.RemoveWire(wireId, CancellationToken.None);

            // Assert
            Assert.False(success);
            Assert.Equal(0, wireCount);
        }

        /// <summary>
        /// Successful remove an existing wire
        /// </summary>
        [Fact]
        public async Task RemoveWire_Exist_Success()
        {
            // Setup
            var wireCount = 0;
            var adapterCount = 0;
            var patch = new Patch();
            patch.WireRemoved += w =>
            {
                wireCount++;
            };
            patch.AdapterRemoved += (w, adapter) =>
            {
                adapterCount++;
            };

            var node1 = new CommonTestNode(Guid.Parse("{174D4604-B18A-4697-922C-B4D02541FD6E}"));
            var node2 = new CommonTestNode(Guid.Parse("{97993AB5-18F1-4FC8-BFDD-8AC9C0EAB69B}"));
            var wireId = Guid.Parse("{10B523DC-30FE-48A6-ABB8-9F927D167A6E}");
            var wire = new TestWire
            {
                Id = wireId,
                Output = node1.Inputs.First(),
                Input = node2.Outputs.First(),
            };

            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);

            // Act
            var success = await patch.RemoveWire(wireId, CancellationToken.None);

            // Assert
            Assert.True(success);
            Assert.Equal(1, wireCount);
            Assert.Equal(0, adapterCount);
        }

        /// <summary>
        /// Remove a wire with an active adapter
        /// </summary>
        [Fact]
        public async Task RemoveWire_ExistWithAdapter_Success()
        {
            // Setup
            var wireCount = 0;
            var adapterCount = 0;
            var patch = new Patch();
            patch.WireRemoved += w =>
            {
                wireCount++;
            };
            patch.AdapterRemoved += (w, a) =>
            {
                adapterCount++;
            };

            var node1 = new CommonTestNode(Guid.Parse("{174D4604-B18A-4697-922C-B4D02541FD6E}"));
            var node2 = new CommonTestNode(Guid.Parse("{97993AB5-18F1-4FC8-BFDD-8AC9C0EAB69B}"));
            var wireId = Guid.Parse("{10B523DC-30FE-48A6-ABB8-9F927D167A6E}");
            var wire = new TestWire
            {
                Id = wireId,
                Output = node1.Inputs.First(),
                Input = node2.Outputs.First(),
            };
            var adapter = new TestAdapter(wire);


            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);
            await patch.AddAdapter(adapter, CancellationToken.None);

            // Act
            var success = await patch.RemoveWire(wireId, CancellationToken.None);

            // Assert
            Assert.True(success);
            Assert.Equal(1, wireCount);
            Assert.Equal(1, adapterCount);
        }

        /// <summary>
        /// Removes a wire with exception in event handler
        /// </summary>
        [Fact]
        public async Task RemoveWire_EventHandlerException_Success()
        {
            // Setup
            var wireCount = 0;
            var adapterCount = 0;
            var patch = new Patch();
            patch.WireRemoved += w =>
            {
                wireCount++;
                throw new NotSupportedException();
            };
            patch.AdapterRemoved += (w, adapter) =>
            {
                adapterCount++;
            };

            var node1 = new CommonTestNode(Guid.Parse("{174D4604-B18A-4697-922C-B4D02541FD6E}"));
            var node2 = new CommonTestNode(Guid.Parse("{97993AB5-18F1-4FC8-BFDD-8AC9C0EAB69B}"));
            var wireId = Guid.Parse("{10B523DC-30FE-48A6-ABB8-9F927D167A6E}");
            var wire = new TestWire
            {
                Id = wireId,
                Output = node1.Inputs.First(),
                Input = node2.Outputs.First(),
            };

            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);

            // Act
            var success = await patch.RemoveWire(wireId, CancellationToken.None);

            // Assert
            Assert.True(success);
            Assert.Equal(1, wireCount);
            Assert.Equal(0, adapterCount);
        }

        #endregion

        #region AddAdapter

        /// <summary>
        /// Tests to add just null as a parameter
        /// </summary>
        [Fact]
        public async Task AddAdapter_Null_Exception()
        {
            // Setup
            var patch = new Patch();

            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(() => patch.AddAdapter(null, CancellationToken.None));
        }

        /// <summary>
        /// Adds an adapter without any wire reference
        /// </summary>
        [Fact]
        public async Task AddAdapter_WithoutWire_Exception()
        {
            // Setup
            var patch = new Patch();

            // Act
            var adapter = new TestAdapter(null);
            await Assert.ThrowsAsync<ArgumentNullException>(() => patch.AddAdapter(adapter, CancellationToken.None));
        }

        /// <summary>
        /// Adds an adapter which uses a not registered wire
        /// </summary>
        [Fact]
        public async Task AddAdapter_InvalidWire_Exception()
        {
            // Setup
            var patch = new Patch();
            var node1 = new CommonTestNode(Guid.Parse("{46E26FC5-C460-4217-BEE9-28DF4BC377E0}"));
            var node2 = new CommonTestNode(Guid.Parse("{A80AF5F4-FDB3-4996-8D1C-AF49352E6F48}"));
            var wire = new TestWire(Guid.Parse("{A219A7AF-1EE4-41A1-929C-E9C791FED030}"), node1.Outputs.First(),
                node2.Inputs.First());

            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);

            // Act
            var adapter = new TestAdapter(wire);
            await Assert.ThrowsAsync<ArgumentException>(() => patch.AddAdapter(adapter, CancellationToken.None));
        }

        /// <summary>
        /// Adds an adapter successful
        /// </summary>
        [Fact]
        public async Task AddAdapter_Complete_Success()
        {
            // Setup
            var adapterAddCount = 0;
            var adapterRemoveCount = 0;
            var patch = new Patch();
            IAdapter adapterOutput = null;
            IWire wireOutput = null;
            patch.AdapterAdded += (w, a) =>
            {
                adapterAddCount++;
                wireOutput = w;
                adapterOutput = a;
            };
            patch.AdapterRemoved += (w, a) =>
            {
                adapterRemoveCount++;
            };

            var node1 = new CommonTestNode(Guid.Parse("{46E26FC5-C460-4217-BEE9-28DF4BC377E0}"));
            var node2 = new CommonTestNode(Guid.Parse("{A80AF5F4-FDB3-4996-8D1C-AF49352E6F48}"));
            var wire = new TestWire(Guid.Parse("{A219A7AF-1EE4-41A1-929C-E9C791FED030}"), node1.Outputs.First(),
                node2.Inputs.First());

            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);

            // Act
            var adapter = new TestAdapter(wire);
            await patch.AddAdapter(adapter, CancellationToken.None);

            // Assert
            Assert.Equal(1, adapterAddCount);
            Assert.Equal(0, adapterRemoveCount);
            Assert.Equal(adapter, adapterOutput);
            Assert.Equal(wire, wireOutput);
        }

        /// <summary>
        /// Adds an adapter successful with a previous one active
        /// </summary>
        [Fact]
        public async Task AddAdapter_ExistingAdapter_Success()
        {
            // Setup
            var adapterAddCount = 0;
            var adapterRemoveCount = 0;
            var patch = new Patch();
            IAdapter adapterAddOutput = null;
            IWire wireAddOutput = null;
            IAdapter adapterRemoveOutput = null;
            patch.AdapterAdded += (w, a) =>
            {
                adapterAddCount++;
                wireAddOutput = w;
                adapterAddOutput = a;
            };
            patch.AdapterRemoved += (w, a) =>
            {
                adapterRemoveCount++;
                adapterRemoveOutput = a;
            };

            var node1 = new CommonTestNode(Guid.Parse("{46E26FC5-C460-4217-BEE9-28DF4BC377E0}"));
            var node2 = new CommonTestNode(Guid.Parse("{A80AF5F4-FDB3-4996-8D1C-AF49352E6F48}"));
            var wire = new TestWire(Guid.Parse("{A219A7AF-1EE4-41A1-929C-E9C791FED030}"), node1.Outputs.First(),
                node2.Inputs.First());
            var adapter1 = new TestAdapter(wire);

            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);
            await patch.AddAdapter(adapter1, CancellationToken.None);
                
            // Act
            var adapter2 = new TestAdapter(wire);
            await patch.AddAdapter(adapter2, CancellationToken.None);

            // Assert
            Assert.Equal(2, adapterAddCount);
            Assert.Equal(1, adapterRemoveCount);
            Assert.Equal(adapter1, adapterRemoveOutput);
            Assert.Equal(adapter2, adapterAddOutput);
            Assert.Equal(wire, wireAddOutput);
        }

        [Fact]
        public async Task AddAdapter_EventHandlerException_Success()
        {
            // Setup
            var adapterCount = 0;
            var patch = new Patch();
            patch.AdapterAdded += (w, a) =>
            {
                adapterCount++;
                throw new NotSupportedException();
            };
            var node1 = new CommonTestNode(Guid.Parse("{46E26FC5-C460-4217-BEE9-28DF4BC377E0}"));
            var node2 = new CommonTestNode(Guid.Parse("{A80AF5F4-FDB3-4996-8D1C-AF49352E6F48}"));
            var wire = new TestWire(Guid.Parse("{A219A7AF-1EE4-41A1-929C-E9C791FED030}"), node1.Outputs.First(),
                node2.Inputs.First());

            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);

            // Act
            var adapter = new TestAdapter(wire);
            await patch.AddAdapter(adapter, CancellationToken.None);

            // Assert
            Assert.Equal(1, adapterCount);
        }

        #endregion

        #region RemoveAdapter

        /// <summary>
        /// Tries to remove an adapter with Guid.Empty
        /// </summary>
        [Fact]
        public async Task RemoveAdapter_EmptyGuid_False()
        {
            // Setup
            var removeCount = 0;
            var patch = new Patch();
            patch.AdapterRemoved += (w, a) =>
            {
                removeCount++;
            };

            // Act
            var success = await patch.RemoveAdapter(Guid.Empty, CancellationToken.None);

            // Assert
            Assert.False(success);
            Assert.Equal(0, removeCount);
        }

        /// <summary>
        /// Tries to remove an adapter with a non existing wire id
        /// </summary>
        [Fact]
        public async Task RemoveAdapter_NotExisting_False()
        {
            // Setup
            var removeCount = 0;
            var patch = new Patch();
            patch.AdapterRemoved += (w, a) =>
            {
                removeCount++;
            };

            // Act
            var success = await patch.RemoveAdapter(Guid.Parse("{676FF64F-5B14-4809-BD43-BA2E2645CF41}"), CancellationToken.None);

            // Assert
            Assert.False(success);
            Assert.Equal(0, removeCount);
        }

        /// <summary>
        /// Tries to remove an adapter on an empty wire
        /// </summary>
        [Fact]
        public async Task RemoveAdapter_NoAdapter_False()
        {
            // Setup
            var removeCount = 0;
            var patch = new Patch();
            patch.AdapterRemoved += (w, a) =>
            {
                removeCount++;
            };

            var node1 = new CommonTestNode(Guid.Parse("{46E26FC5-C460-4217-BEE9-28DF4BC377E0}"));
            var node2 = new CommonTestNode(Guid.Parse("{A80AF5F4-FDB3-4996-8D1C-AF49352E6F48}"));
            var wire = new TestWire(Guid.Parse("{A219A7AF-1EE4-41A1-929C-E9C791FED030}"), node1.Outputs.First(),
                node2.Inputs.First());

            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);

            // Act
            var success = await patch.RemoveAdapter(wire.Id, CancellationToken.None);

            // Assert
            Assert.False(success);
            Assert.Equal(0, removeCount);
        }

        /// <summary>
        /// Removes an adapter
        /// </summary>
        [Fact]
        public async Task RemoveAdapter_ExistingAdapter_True()
        {
            // Setup
            var removeCount = 0;
            var patch = new Patch();
            patch.AdapterRemoved += (w, a) =>
            {
                removeCount++;
            };

            var node1 = new CommonTestNode(Guid.Parse("{46E26FC5-C460-4217-BEE9-28DF4BC377E0}"));
            var node2 = new CommonTestNode(Guid.Parse("{A80AF5F4-FDB3-4996-8D1C-AF49352E6F48}"));
            var wire = new TestWire(Guid.Parse("{A219A7AF-1EE4-41A1-929C-E9C791FED030}"), node1.Outputs.First(),
                node2.Inputs.First());
            var adapter = new TestAdapter(wire);

            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);
            await patch.AddAdapter(adapter, CancellationToken.None);

            // Act
            var success = await patch.RemoveAdapter(wire.Id, CancellationToken.None);

            // Assert
            Assert.True(success);
            Assert.Equal(1, removeCount);
        }

        /// <summary>
        /// Removes an adapter with exception during dispose
        /// </summary>
        [Fact]
        public async Task RemoveAdapter_DisposeException_True()
        {
            // Setup
            var removeCount = 0;
            var patch = new Patch();
            patch.AdapterRemoved += (w, a) =>
            {
                removeCount++;
            };

            var node1 = new CommonTestNode(Guid.Parse("{46E26FC5-C460-4217-BEE9-28DF4BC377E0}"));
            var node2 = new CommonTestNode(Guid.Parse("{A80AF5F4-FDB3-4996-8D1C-AF49352E6F48}"));
            var wire = new TestWire(Guid.Parse("{A219A7AF-1EE4-41A1-929C-E9C791FED030}"), node1.Outputs.First(),
                node2.Inputs.First());
            var adapter = new TestAdapter(wire);
            adapter.OnDisposeException = new NotSupportedException();

            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);
            await patch.AddAdapter(adapter, CancellationToken.None);

            // Act
            var success = await patch.RemoveAdapter(wire.Id, CancellationToken.None);

            // Assert
            Assert.True(success);
            Assert.Equal(1, removeCount);
        }

        /// <summary>
        /// Removes an adapter with exception during external event handler
        /// </summary>
        [Fact]
        public async Task RemoveAdapter_EventHandlerException_True()
        {
            // Setup
            var removeCount = 0;
            var patch = new Patch();
            patch.AdapterRemoved += (w, a) =>
            {
                removeCount++;
                throw new NotSupportedException();
            };

            var node1 = new CommonTestNode(Guid.Parse("{46E26FC5-C460-4217-BEE9-28DF4BC377E0}"));
            var node2 = new CommonTestNode(Guid.Parse("{A80AF5F4-FDB3-4996-8D1C-AF49352E6F48}"));
            var wire = new TestWire(Guid.Parse("{A219A7AF-1EE4-41A1-929C-E9C791FED030}"), node1.Outputs.First(),
                node2.Inputs.First());
            var adapter = new TestAdapter(wire);

            await patch.AddNode(node1, CancellationToken.None);
            await patch.AddNode(node2, CancellationToken.None);
            await patch.AddWire(wire, CancellationToken.None);
            await patch.AddAdapter(adapter, CancellationToken.None);

            // Act
            var success = await patch.RemoveAdapter(wire.Id, CancellationToken.None);

            // Assert
            Assert.True(success);
            Assert.Equal(1, removeCount);
        }

        #endregion

        /// <summary>
        /// Dummy implementation of a common node
        /// </summary>
        private sealed class CommonTestNode : Node<EmptyConfiguration, EmptyEnvironment>
        {
            /// <summary>
            /// Gets or sets the optional exception when deinitialize gets called
            /// </summary>
            public Exception OnDeinitializeException { get; set; }

            /// <summary>
            /// Gets or sets the optional exception when dispose gets called
            /// </summary>
            public Exception OnDisposeException { get; set; }

            public CommonTestNode(Guid id) : base(id)
            {
                RegisterInputConnector<TestType>(new ConnectorDescription("input", "input", "input", ComplexContentType.Create<TestType>(DefaultPluginGuid)));
                RegisterOutputConnector<TestType>(new ConnectorDescription("output", "output", "output", ComplexContentType.Create<TestType>(DefaultPluginGuid)));
            }

            protected override EmptyConfiguration DefaultConfiguration =>
                new EmptyConfiguration();

            protected override Task OnDeinitialize(CancellationToken cancellationToken)
            {
                if (OnDeinitializeException != null)
                {
                    throw OnDeinitializeException;
                }

                return base.OnDeinitialize(cancellationToken);
            }

            protected override void OnDispose()
            {
                if (OnDisposeException != null)
                {
                    throw OnDisposeException;
                }

                base.OnDispose();
            }
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

        /// <summary>
        /// Dummy implementation of a wire
        /// </summary>
        private sealed class TestWire : IWire
        {
            /// <summary>
            /// Gets or sets the id of a wire
            /// </summary>
            public Guid Id { get; set; }

            public IInputConnector Output { get; set; }

            public IOutputConnector Input { get; set; }

            public TestWire() { }

            public TestWire(Guid id, IOutputConnector input, IInputConnector output)
            {
                Id = id;
                Input = input;
                Output = output;
            }
        }

        /// <summary>
        /// Dummy implementation 
        /// </summary>
        private sealed class TestAdapter : IAdapter
        {
            /// <summary>
            /// Gets or sets the 
            /// </summary>
            public Exception OnDisposeException { get; set; }

            public IWire Wire { get; }

            public TestAdapter(IWire wire)
            {
                Wire = wire;
            }

            public void Dispose()
            {
                if (OnDisposeException != null)
                {
                    throw OnDisposeException;
                }
            }

            public string GetEnvironment()
            {
                throw new NotImplementedException();
            }

            public string GetConfiguration()
            {
                throw new NotImplementedException();
            }

            public Task SetConfiguration(string configuration, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public event Action<IAdapter, string> ConfigurationChanged;

            public event Action<IAdapter, string> EnvironmentChanged;
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
