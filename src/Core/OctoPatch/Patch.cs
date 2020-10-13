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
                    Logger.LogError(ex, $"Exception in handler of {nameof(NodeAdded)}");
                }
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var node = _nodes.ToArray().FirstOrDefault(n => n.Id == nodeId);
                if (node == null)
                {
                    return false;
                }

                // Make sure there is no depending node
                if (_nodes.OfType<ISplitterNode>().Any(n => n.Connector.NodeId == node.Id) ||
                    _nodes.OfType<ICollectorNode>().Any(n => n.Connector.NodeId == node.Id) ||
                    _nodes.OfType<IAttachedNode>().Any(n => n.ParentNode == node))
                {
                    throw new ArgumentException("There is at least one node which depends on the given one");
                }

                Logger.LogInformation($"Removing node {node.Id}");

                // Shut down and dispose it
                try
                {
                    await node.Deinitialize(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Could not deinitialize node");
                }

                try
                {
                    node.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Could not dispose node");
                }

                _nodes.Remove(node);

                try
                {
                    NodeRemoved?.Invoke(node);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Exception in handler of {nameof(NodeRemoved)}");
                }

                return true;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <inheritdoc />
        public async Task AddWire(IWire wire, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                if (wire == null)
                {
                    throw new ArgumentNullException(nameof(wire));
                }

                if (wire.Input == null)
                {
                    throw new ArgumentNullException(nameof(wire.Input));
                }

                if (wire.Output == null)
                {
                    throw new ArgumentNullException(nameof(wire.Output));
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

                if (inputNode.Outputs.All(i => i != wire.Input))
                {
                    throw new ArgumentException("given input connector does not fit to the inner input node");
                }

                var outputNode = _nodes.FirstOrDefault(n => n.Id == wire.Output.NodeId);
                if (outputNode == null)
                {
                    throw new ArgumentException("output node is not part of the system");
                }

                if (outputNode.Inputs.All(i => i != wire.Output))
                {
                    throw new ArgumentException("given output connector does not fit to the inner output node");
                }

                // Check if there is already a connection between the same connectors
                if (_wires.Any(w => w.Input == wire.Input && w.Output == wire.Output))
                {
                    throw new ArgumentException("There is already a wire between the given connectors");
                }

                _wires.Add(wire);

                // Automatically add a pass adapter
                var adapter = new PassAdapter(wire);
                _adapters.Add(wire, adapter);

                try
                {
                    WireAdded?.Invoke(wire);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Exception during WireAdded event");
                }
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemoveWire(Guid wireId, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                var wire = _wires.FirstOrDefault(w => w.Id == wireId);
                if (wire == null)
                {
                    return false;
                }

                // Make sure adapter gets removed
                InternalRemoveAdapter(wire);

                _wires.Remove(wire);

                try
                {
                    WireRemoved?.Invoke(wire);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Exception in handler of {nameof(WireRemoved)}");
                }

                return true;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <inheritdoc />
        public async Task AddAdapter(IAdapter adapter, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                if (adapter == null)
                {
                    throw new ArgumentNullException(nameof(adapter));
                }

                if (adapter.Wire == null)
                {
                    throw new ArgumentNullException(nameof(adapter.Wire));
                }

                // Lookup wire
                var wire = _wires.FirstOrDefault(w => w == adapter.Wire);
                if (wire == null)
                {
                    throw new ArgumentException("Wire is not part of the system");
                }

                // Check if there is already an adapter
                InternalRemoveAdapter(wire);

                _adapters.Add(wire, adapter);

                try
                {
                    AdapterAdded?.Invoke(wire, adapter);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Exception in handler of {nameof(AddAdapter)}");
                }
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemoveAdapter(Guid wireId, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                // Lookup wire
                var wire = _wires.FirstOrDefault(w => w.Id == wireId);
                if (wire == null)
                {
                    return false;
                }

                // Stop here if existing adapter is a pass adapter
                if (_adapters.TryGetValue(wire, out var oldAdapter) && oldAdapter is PassAdapter)
                {
                    return false;
                }

                var result = InternalRemoveAdapter(wire);

                // Add a pass adapter to place the empty spot
                var adapter = new PassAdapter(wire);
                _adapters.Add(wire, adapter);

                return result;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Properly shut down existing adapter (Dispose) and fires an event if required
        /// </summary>
        /// <param name="wire">reference to the wire</param>
        /// <returns>success</returns>
        private bool InternalRemoveAdapter(IWire wire)
        {
            // Find adapter
            if (!_adapters.TryGetValue(wire, out var adapter))
            {
                return false;
            }

            try
            {
                adapter.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Could not dispose adapter");
            }

            // Only call the external RemoveAdapter when there was a real adapter
            if (!(adapter is PassAdapter))
            {
                try
                {
                    AdapterRemoved?.Invoke(wire, adapter);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Exception in handler of {nameof(AdapterRemoved)}");
                }
            }

            return _adapters.Remove(wire);
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
