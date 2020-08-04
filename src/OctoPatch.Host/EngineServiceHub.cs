using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using OctoPatch.Exchange;

namespace OctoPatch.Host
{
    /// <summary>
    /// Common implementation of the engine service hub
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
        public IEnumerable<NodeDescription> GetNodeDescriptions()
        {
            return _repository.GetNodeDescriptions();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<MessageDescription> GetMessageDescriptions()
        {
            return _repository.GetMessageDescriptions();
        }

        #endregion

        #region Engine manipulation

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<NodeInstance> GetNodes()
        {
            return _engine.Nodes.Select(n => n.Instance);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<WireInstance> GetWires()
        {
            return _engine.Wires.Select(w => w.Instance);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Grid GetEngineConfiguration()
        {
            return _engine.Store();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void SetEngineConfiguration(Grid grid)
        {
            _engine.Load(grid);
        }

        #endregion

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string GetNodeEnvironment(Guid nodeGuid)
        {
            var node = _engine.Nodes.FirstOrDefault(n => n.Instance.Guid == nodeGuid);
            if (node == null)
            {
                throw new ArgumentException("node id does not exist");
            }

            // TODO: What should be returned here?
            return string.Empty;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string GetNodeConfiguration(Guid nodeGuid)
        {
            var node = _engine.Nodes.FirstOrDefault(n => n.Instance.Guid == nodeGuid);
            if (node == null)
            {
                throw new ArgumentException("node id does not exist");
            }

            return node.Instance.Configuration;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void SetNodeConfiguration(Guid nodeGuid, string configuration)
        {
            var node = _engine.Nodes.FirstOrDefault(n => n.Instance.Guid == nodeGuid);
            if (node == null)
            {
                throw new ArgumentException("node id does not exist");
            }

            // TODO: Apply configuration to the node
        }
    }
}
