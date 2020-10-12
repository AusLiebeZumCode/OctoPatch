using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OctoPatch.Logging;

namespace OctoPatch
{
    /// <summary>
    /// Implementation of a patch
    /// </summary>
    public sealed class Patch : IPatch
    {
        /// <summary>
        /// Reference to the logger
        /// </summary>
        private static readonly ILogger<Patch> Logger = LogManager.GetLogger<Patch>();

        private readonly SemaphoreSlim _localLock;

        /// <summary>
        /// Internal list of nodes
        /// </summary>
        private readonly HashSet<INode> _nodes;

        /// <summary>
        /// Internal list of wires
        /// </summary>
        private readonly HashSet<IWire> _wires;

        /// <summary>
        /// Internal list of adapters
        /// </summary>
        private readonly Dictionary<IWire, IAdapter> _adapters;

        public Patch()
        {
            _localLock = new SemaphoreSlim(1);

            _nodes = new HashSet<INode>();
            _wires = new HashSet<IWire>();
            _adapters = new Dictionary<IWire, IAdapter>();
        }

        /// <inheritdoc />
        public async Task AddNode(INode node, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                InternalAddNode(node);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Adds the given node to the patch
        /// </summary>
        /// <param name="node">node reference</param>
        /// <returns></returns>
        private void InternalAddNode(INode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Double check for node id collisions
            if (_nodes.Any(n => n.Id == node.Id))
            {
                throw new ArgumentException("node with this id already exists", nameof(node.Id));
            }

            Guid? parentNodeId = null;
            INode parentNode = null;
            IOutputConnector outputConnector = null;
            IInputConnector inputConnector = null;
            switch (node)
            {
                // Make sure parent exists
                case IAttachedNode attachedNode:
                    parentNode = attachedNode.ParentNode;
                    parentNodeId = attachedNode.ParentNode.Id;
                    break;
                case ISplitterNode splitterNode:
                    parentNodeId = splitterNode.Connector.NodeId;
                    outputConnector = splitterNode.Connector;
                    break;
                case ICollectorNode collectorNode:
                    parentNodeId = collectorNode.Connector.NodeId;
                    inputConnector = collectorNode.Connector;
                    break;
            }

            // Lookup parent node
            if (parentNodeId.HasValue && parentNode == null)
            {
                parentNode = _nodes.FirstOrDefault(n => n.Id == parentNodeId.Value);
                if (parentNode == null)
                {
                    throw new ArgumentException("parent node is not part of the system");
                }
            }

            // Compare parent node
            if (parentNode != null && _nodes.All(n => n != parentNode))
            {
                throw new ArgumentException("parent node is not part of the system");
            }

            // compare output connector
            if (outputConnector != null && parentNode.Outputs.All(o => o != outputConnector))
            {
                throw new ArgumentException("output connector does not fit to the discovered parent node");
            }

            // compare input connector
            if (inputConnector != null && parentNode.Inputs.All(o => o != inputConnector))
            {
                throw new ArgumentException("input connector does not fit to the discovered parent node");
            }

            _nodes.Add(node);
            try
            {
                NodeAdded?.Invoke(node);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception during NodeAdded event");
            }
        }

        /// <inheritdoc />
        public async Task RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var node = _nodes.ToArray().FirstOrDefault(n => n.Id == nodeId);
                if (node == null)
                {
                    throw new ArgumentException("node does not exist", nameof(nodeId));
                }

                await InternalRemoveNode(node, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Removes the given node from the system
        /// - Stops and disposes it
        /// - Removes all wires
        /// - removes it
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task InternalRemoveNode(INode node, CancellationToken cancellationToken)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (!_nodes.Contains(node))
            {
                throw new ArgumentException("node does not exist", nameof(node));
            }

            // Shut down and dispose it
            await node.Deinitialize(cancellationToken);
            node.Dispose();

            _nodes.Remove(node);
            NodeRemoved?.Invoke(node);
        }

        /// <inheritdoc />
        public async Task AddWire(IWire wire, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                await InternalAddWire(wire);
            }
            finally
            {
                _localLock.Release();
            }
        }

        private Task InternalAddWire(IWire wire)
        {
            if (wire == null)
            {
                throw new ArgumentNullException(nameof(wire));
            }

            // Check for id collisions
            if (_wires.Any(w => w.Id == wire.Id))
            {
                throw new ArgumentException("same wire is already part of the system");
            }

            // Check if related nodes are part of the patch
            var inputNode = _nodes.FirstOrDefault(n => n.Id == wire.Input.NodeId);
            if (inputNode == null)
            {
                throw new ArgumentException("input node is not part of the system");
            }

            if (inputNode.Inputs.All(i => i != wire.Input))
            {
                throw new ArgumentException("given input connector does not fit to the inner input node");
            }

            var outputNode = _nodes.FirstOrDefault(n => n.Id == wire.Output.NodeId);
            if (outputNode == null)
            {
                throw new ArgumentException("output node is not part of the system");
            }

            if (outputNode.Outputs.All(i => i != wire.Output))
            {
                throw new ArgumentException("given output connector does not fit to the inner output node");
            }

            // Check if there is already a connection between the same connectors
            if (_wires.Any(w => w.Input == wire.Input && w.Output == wire.Output))
            {
                throw new ArgumentException("There is already a wire between the given connectors");
            }

            _wires.Add(wire);

            try
            {
                WireAdded?.Invoke(wire);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception during WireAdded event");
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task RemoveWire(Guid wireId, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var wire = _wires.FirstOrDefault(w => w.Id == wireId);
                if (wire == null)
                {
                    throw new ArgumentException("wire does not exist");
                }

                await InternalRemoveWire(wire);
            }
            finally
            {
                _localLock.Release();
            }
        }

        private Task InternalRemoveWire(IWire wire)
        {
            if (wire == null)
            {
                throw new ArgumentNullException(nameof(wire));
            }

            if (!_wires.Contains(wire))
            {
                throw new ArgumentException("wire does not exist", nameof(wire));
            }

            // Dispose potential existing adapters
            InternalRemoveAdapter(wire.Id);

            wire.Dispose();

            _wires.Remove(wire);
            WireRemoved?.Invoke(wire);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task AddAdapter(Guid wireId, IAdapter adapter, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                InternalAddAdapter(wireId, adapter);
            }
            finally
            {
                _localLock.Release();
            }
        }

        private void InternalAddAdapter(Guid wireId, IAdapter adapter)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            // Lookup wire
            var wire = _wires.FirstOrDefault(w => w.Id == wireId);
            if (wire == null)
            {
                throw new ArgumentException("Wire does not exist");
            }

            // Check if there is already an adapter
            InternalRemoveAdapter(wireId);

            _adapters.Add(wire, adapter);
            AdapterAdded?.Invoke(wire, adapter);
        }

        /// <inheritdoc />
        public async Task RemoveAdapter(Guid wireId, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                InternalRemoveAdapter(wireId);
            }
            finally
            {
                _localLock.Release();
            }
        }

        private void InternalRemoveAdapter(Guid wireId)
        {
            // Find wire
            var wire = _wires.FirstOrDefault(w => w.Id == wireId);
            if (wire == null)
            {
                throw new ArgumentException("Wire does not exist");
            }

            // Find adapter
            if (!_adapters.TryGetValue(wire, out var adapter))
            {
                return;
            }

            // Dispose
            adapter.Dispose();

            AdapterRemoved?.Invoke(wire, adapter);
            _adapters.Remove(wire);
        }

        /// <inheritdoc />
        public event Action<INode> NodeAdded;

        /// <inheritdoc />
        public event Action<INode> NodeRemoved;

        /// <inheritdoc />
        public event Action<IWire> WireAdded;

        /// <inheritdoc />
        public event Action<IWire> WireRemoved;

        /// <inheritdoc />
        public event Action<IWire, IAdapter> AdapterAdded;

        /// <inheritdoc />
        public event Action<IWire, IAdapter> AdapterRemoved;
    }
}
