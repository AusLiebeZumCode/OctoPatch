using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch.Core
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

        private async Task InternalAddNode(INode node, CancellationToken cancellationToken, string configuration = null)
        {
            // Double check for node id collisions
            if (_nodes.ToArray().Any(n => n.NodeId == node.NodeId))
            {
                throw new ArgumentException("node with this id already exists", nameof(node.NodeId));
            }

            _nodes.Add(node);

            if (!string.IsNullOrEmpty(configuration))
            {
                await node.Initialize(configuration, cancellationToken);
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task RemoveNode(INode node, CancellationToken cancellationToken)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            await _localLock.WaitAsync(cancellationToken);
            try
            {
                await InternalRemoveNode(node, cancellationToken);
            }
            finally
            {
                _localLock.Release();
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
                var node = _nodes.ToArray().FirstOrDefault(n => n.NodeId == nodeId);
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

            if (_nodes.ToArray().Contains(node))
            {
                throw new ArgumentException("node does not exist", nameof(node));
            }

            // Shut down and dispose it
            await node.Deinitialize(cancellationToken);

            var connectors = node.Inputs.Cast<IConnector>()
                .Union(node.Outputs.Cast<IConnector>()).ToArray();

            // Remove all wires
            foreach (var wire in _wires.ToArray()
                .Where(w => connectors.Contains(w.InputConnector) ||
                            connectors.Contains(w.OutputConnector)))
            {
                await InternalRemoveWire(wire, cancellationToken);
            }

            _nodes.Remove(node);
        }

        #endregion

        #region wire management

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<IWire> AddWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                return await InternalAddWire(outputNode, outputConnector, inputNode, inputConnector, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        private Task<IWire> InternalAddWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector,
            CancellationToken cancellationToken)
        {
            // Identify nodes and outputs
            // CHeck collision
            // Create the wire
            // Attach it
            // Add it

            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task RemoveWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            //await _localLock.WaitAsync(cancellationToken);
            //try
            //{
            //    // Lookup wire
            //    var wire = _wires.ToArray().FirstOrDefault(w =>
            //        w.Instance.OutputNode == outputNode && w.Instance.OutputConnector == outputConnector &&
            //        w.Instance.InputNode == inputNode && w.Instance.InputConnector == inputConnector);

            //    if (wire == null)
            //    {
            //        throw new ArgumentException("there is no wire with this parameter");
            //    }

            //    await InternalRemoveWire(wire, cancellationToken);
            //}
            //finally
            //{
            //    _localLock.Release();
            //}
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task RemoveWire(IWire wire, CancellationToken cancellationToken)
        {
            if (wire == null)
            {
                throw new ArgumentNullException(nameof(wire));
            }

            await _localLock.WaitAsync(cancellationToken);
            try
            {
                await InternalRemoveWire(wire, cancellationToken);
            }
            finally
            {
                _localLock.Release();
            }
        }

        private Task InternalRemoveWire(IWire wire, CancellationToken cancellationToken)
        {
            if (wire == null)
            {
                throw new ArgumentNullException(nameof(wire));
            }

            _wires.Remove(wire);
            return Task.CompletedTask;
        }

        #endregion
    }
}
