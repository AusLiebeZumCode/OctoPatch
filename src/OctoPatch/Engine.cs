using OctoPatch.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
{
    /// <summary>
    /// Implementation of the OctoPatch Engine
    /// </summary>
    public sealed class Engine : IEngine
    {
        private readonly SemaphoreSlim _localLock;

        /// <summary>
        /// Reference to the repository
        /// </summary>
        private readonly IRepository _repository;

        /// <summary>
        /// List of all available node descriptions
        /// </summary>
        private readonly NodeDescription[] _descriptions;

        /// <summary>
        /// Internal list of nodes
        /// </summary>
        private readonly ObservableCollection<INode> _nodes;

        /// <summary>
        /// Internal list of wires
        /// </summary>
        private readonly ObservableCollection<IWire> _wires;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public ReadOnlyObservableCollection<INode> Nodes { get; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public ReadOnlyObservableCollection<IWire> Wires { get; }

        public Engine(IRepository repository)
        {
            _localLock = new SemaphoreSlim(1);

            _repository = repository;
            _descriptions = repository.GetNodeDescriptions().ToArray();
            _nodes = new ObservableCollection<INode>();
            _wires = new ObservableCollection<IWire>();
            Nodes = new ReadOnlyObservableCollection<INode>(_nodes);
            Wires = new ReadOnlyObservableCollection<IWire>(_wires);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Load(Grid grid)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            var localNodes = new Dictionary<Guid, INode>();

            // Create new instances of the given grid configuration
            foreach (var nodeInstance in grid.NodeInstances)
            {
                var description = _descriptions.FirstOrDefault(d => d.Guid == nodeInstance.NodeDescription);
                if (description == null)
                {
                    // TODO: Think about unknown node
                    continue;
                }

                INode node;
                try
                {
                    node = _repository.CreateNode(description.Guid);
                }
                catch (Exception)
                {
                    // TODO: Think about possible exception types here
                    continue;
                }

                try
                {
                    node.Initialize(nodeInstance.Guid, nodeInstance.Configuration, CancellationToken.None);
                }
                catch (Exception)
                {
                    // TODO: Think about possible exceptions and what happens in that case
                    continue;
                }

                localNodes.Add(nodeInstance.Guid, node);
                _nodes.Add(node);
            }

            // Wire up all new nodes with wires
            foreach (var wireInstance in grid.WireInstances)
            {
                if (!localNodes.TryGetValue(wireInstance.OutputNode, out var outputNode) ||
                    !localNodes.TryGetValue(wireInstance.InputNode, out var inputNode))
                {
                    continue;
                }

                var outputConnector = outputNode.Outputs
                    .FirstOrDefault(c => c.Guid == wireInstance.OutputConnector);
                if (outputConnector == null)
                {
                    continue;
                }

                var inputConnector = inputNode.Inputs
                    .FirstOrDefault(c => c.Guid == wireInstance.InputConnector);
                if (inputConnector == null)
                {
                    continue;
                }

                var wire = new Wire(wireInstance, inputConnector, outputConnector);

                _wires.Add(wire);
                
                // TODO: Wire up by subscribe
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Grid Store()
        {
            throw new NotImplementedException();
        }

        #region engine lifecycle

        /// <summary>
        /// Gets the current engine state
        /// </summary>
        public EngineState State { get; private set; }

        /// <summary>
        /// Starts the engine
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        public async Task Start(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                State = EngineState.Starting;

                // Trying to start all existing nodes
                foreach (var node in Nodes)
                {
                    try
                    {
                        await node.Start(cancellationToken);
                    }
                    catch (Exception)
                    {
                        // TODO: Log this!
                    }
                }

                State = EngineState.Running;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Stops the engine
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        public async Task Stop(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                State = EngineState.Starting;

                // Trying to stop all existing nodes
                foreach (var node in Nodes)
                {
                    try
                    {
                        await node.Stop(cancellationToken);
                    }
                    catch (Exception)
                    {
                        // TODO: Log this!
                    }
                }

                State = EngineState.Running;
            }
            finally
            {
                _localLock.Release();
            }
        }

        #endregion
    }
}
