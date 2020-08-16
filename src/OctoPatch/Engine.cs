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
            State = EngineState.Stopped;

            _repository = repository;
            _descriptions = repository.GetNodeDescriptions().ToArray();

            _nodes = new ObservableCollection<INode>();
            _wires = new ObservableCollection<IWire>();
            Nodes = new ReadOnlyObservableCollection<INode>(_nodes);
            Wires = new ReadOnlyObservableCollection<IWire>(_wires);
        }

        #region engine lifecycle

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public EngineState State { get; private set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
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
        /// <inheritdoc />
        /// </summary>
        public async Task Stop(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                State = EngineState.Stopping;

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

                State = EngineState.Stopped;
            }
            finally
            {
                _localLock.Release();
            }
        }

        #endregion

        #region node management

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> AddNode(Type type, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> AddNode(Type type, string configuration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> AddNode(Type type, Guid nodeId, string configuration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> AddNode(Guid descriptionId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> AddNode(Guid descriptionId, string configuration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> AddNode(Guid descriptionId, Guid nodeId, string configuration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> AddNode<T>(CancellationToken cancellationToken) where T : INode
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> AddNode<T>(string configuration, CancellationToken cancellationToken) where T : INode
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> AddNode<T>(Guid nodeId, string configuration, CancellationToken cancellationToken) where T : INode
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task RemoveNode(INode node)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task RemoveNode(Guid nodeId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region wire management

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IWire> AddWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task RemoveWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task RemoveWire(IWire wire)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
