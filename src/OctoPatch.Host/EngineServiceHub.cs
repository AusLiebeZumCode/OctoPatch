using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OctoPatch.Communication;

namespace OctoPatch.Host
{
    /// <summary>
    /// Common implementation of the engine service hub
    /// StreamNote: Yannick: Preise sind ok :D (2020-08-11)
    /// </summary>
    public sealed class EngineServiceHub : Hub<IEngineServiceCallback>, IEngineService
    {
        private readonly IRepository _repository;

        private readonly IEngine _engine;

        public EngineServiceHub(IRepository repository, IEngine engine)
        {
            _repository = repository;
            _engine = engine;
        }

        #region Meta information

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<NodeDescription>> GetNodeDescriptions(CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetNodeDescriptions());
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<MessageDescription>> GetMessageDescriptions(CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetMessageDescriptions());
        }

        #endregion

        #region Engine manipulation

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<NodeInstance>> GetNodes(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            
            // return Task.FromResult(_engine.Nodes.Select(n => n.Instance));
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<IEnumerable<WireInstance>> GetWires(CancellationToken cancellationToken)
        {
            return Task.FromResult(_engine.Wires.Select(w => w.Instance));
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<Grid> GetEngineConfiguration(CancellationToken cancellationToken)
        {
            return Task.FromResult(_engine.Store());
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task SetEngineConfiguration(Grid grid, CancellationToken cancellationToken)
        {
            _engine.Load(grid);
            return Task.CompletedTask;
        }

        #endregion

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<string> GetNodeEnvironment(Guid nodeGuid, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            //var node = _engine.Nodes.FirstOrDefault(n => n.Instance.Guid == nodeGuid);
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

            //var node = _engine.Nodes.FirstOrDefault(n => n.Instance.Guid == nodeGuid);
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

            //var node = _engine.Nodes.FirstOrDefault(n => n.Instance.Guid == nodeGuid);
            //if (node == null)
            //{
            //    throw new ArgumentException("node id does not exist");
            //}

            //return Task.CompletedTask;
            //// TODO: Apply configuration to the node
        }
    }
}
