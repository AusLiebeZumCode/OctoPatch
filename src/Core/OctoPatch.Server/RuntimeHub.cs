﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OctoPatch.Descriptions;
using OctoPatch.Setup;

namespace OctoPatch.Server
{
    /// <summary>
    /// Common implementation of the runtime hub
    /// StreamNote: Yannick: Preise sind ok :D (2020-08-11)
    /// </summary>
    public sealed class RuntimeHub : Hub<IRuntimeCallbacks>, IRuntimeMethods
    {
        private readonly IRepository _repository;
        private readonly IRuntimeMethods _runtime;
        private readonly ILogger<RuntimeHub> _logger;

        public RuntimeHub(IRepository repository, IRuntimeMethods runtime, ILogger<RuntimeHub> logger)
        {
            _repository = repository;
            _runtime = runtime;
            _logger = logger;
        }

        #region Meta information

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<NodeDescription>> GetNodeDescriptions(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Asked for node descriptions");
            return Task.FromResult(_repository.GetNodeDescriptions());
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<TypeDescription>> GetMessageDescriptions(CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetTypeDescriptions());
        }

        public Task<IEnumerable<AdapterDescription>> GetAdapterDescriptions(CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetAdapterDescriptions());
        }

        #endregion

        #region Patch manipulation

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<NodeSetup>> GetNodes(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            // return Task.FromResult(_patch.Nodes.Select(n => n.Instance));
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<WireSetup>> GetWires(CancellationToken cancellationToken)
        {
            return _runtime.GetWires(cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task SetNodeDescription(Guid nodeId, string name, string description,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<GridSetup> GetConfiguration(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            // return Task.FromResult(_patch.Store());
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task SetConfiguration(GridSetup grid, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //_patch.Load(grid);
            //return Task.CompletedTask;
        }

        public Task<NodeSetup> AddNode(string key, Guid? parentId, string connectorKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public Task<WireSetup> AddWire(Guid outputNodeId, string outputConnectorKey, Guid inputNodeId, string intputConnectorKey,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public Task RemoveWire(Guid wireId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        #endregion

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<string> GetNodeEnvironment(Guid nodeGuid, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            //var node = _patch.Nodes.FirstOrDefault(n => n.Instance.Guid == nodeGuid);
            //if (node == null)
            //{
            //    throw new ArgumentException("node id does not exist");
            //}

            //// TODO: What should be returned here?
            //return Task.FromResult(string.Empty);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<string> GetNodeConfiguration(Guid nodeGuid, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            //var node = _patch.Nodes.FirstOrDefault(n => n.Instance.Guid == nodeGuid);
            //if (node == null)
            //{
            //    throw new ArgumentException("node id does not exist");
            //}

            //return Task.FromResult(node.Instance.Configuration);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task SetNodeConfiguration(Guid nodeGuid, string configuration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            //var node = _patch.Nodes.FirstOrDefault(n => n.Instance.Guid == nodeGuid);
            //if (node == null)
            //{
            //    throw new ArgumentException("node id does not exist");
            //}

            //return Task.CompletedTask;
            //// TODO: Apply configuration to the node
        }

        public Task SetNodePosition(Guid nodeId, int x, int y, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StartNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopNode(Guid nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
