using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
{
    /// <summary>
    /// Implementation of a patch
    /// </summary>
    public sealed class Patch : IPatch
    {
        private readonly SemaphoreSlim _localLock;

        /// <summary>
        /// Internal list of nodes
        /// </summary>
        private readonly HashSet<INode> _nodes;

        /// <summary>
        /// Internal list of wires
        /// </summary>
        private readonly HashSet<IWire> _wires;

        public Patch()
        {
            _localLock = new SemaphoreSlim(1);

            _nodes = new HashSet<INode>();
            _wires = new HashSet<IWire>();
        }

        #region node management

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task AddNode(INode node, CancellationToken cancellationToken)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            await _localLock.WaitAsync(cancellationToken);
            try
            {
                await InternalAddNode(node, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task AddNode(INode node, string configuration, CancellationToken cancellationToken)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            await _localLock.WaitAsync(cancellationToken);
            try
            {
                await InternalAddNode(node, cancellationToken, configuration);
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
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="configuration">optional configuration</param>
        /// <returns></returns>
        private async Task InternalAddNode(INode node, CancellationToken cancellationToken, string configuration = null)
        {
            // Double check for node id collisions
            if (_nodes.ToArray().Any(n => n.Id == node.Id))
            {
                throw new ArgumentException("node with this id already exists", nameof(node.Id));
            }

            _nodes.Add(node);
            NodeAdded?.Invoke(node);

            // Try to setup
            if (!string.IsNullOrEmpty(configuration))
            {
                await node.Initialize(configuration, cancellationToken);
            }
            else
            {
                await node.Initialize(cancellationToken);
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
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

            if (!_nodes.ToArray().Contains(node))
            {
                throw new ArgumentException("node does not exist", nameof(node));
            }

            // Shut down and dispose it
            await node.Deinitialize(cancellationToken);

            // TODO: Remove Wires
            // TODO: Remove attached nodes

            _nodes.Remove(node);
            NodeRemoved?.Invoke(node);
        }

        #endregion

        #region wire management

        /// <summary>
        /// <inheritdoc />
        /// </summary>
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
            _wires.Add(wire);
            WireAdded?.Invoke(wire);

            return Task.CompletedTask;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
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

            _wires.Remove(wire);
            WireRemoved?.Invoke(wire);

            return Task.CompletedTask;
        }

        #endregion

        /// <summary>
        /// Gets a call when a new node was added
        /// </summary>
        public event Action<INode> NodeAdded;

        /// <summary>
        /// Gets a call when a node was removed
        /// </summary>
        public event Action<INode> NodeRemoved;

        /// <summary>
        /// Gets a call when a new wire was added
        /// </summary>
        public event Action<IWire> WireAdded;

        /// <summary>
        /// Gets a call when a wire was removed
        /// </summary>
        public event Action<IWire> WireRemoved;
    }
}
