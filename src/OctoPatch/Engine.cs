using System;
using System.Collections.Generic;
using System.Linq;
using OctoPatch.Exchange;

namespace OctoPatch
{
    /// <summary>
    /// Implementation of the OctoPatch Engine
    /// </summary>
    public sealed class Engine : IEngine
    {
        /// <summary>
        /// Reference to the repository
        /// </summary>
        private readonly IRepository _repository;

        /// <summary>
        /// List of all available node descriptions
        /// </summary>
        private readonly NodeDescription[] _descriptions;

        /// <summary>
        /// List of all node instances
        /// </summary>
        private readonly Dictionary<Guid, INode> _nodes;

        /// <summary>
        /// LIst of all wires
        /// </summary>
        private readonly List<Wire> _wires;

        public Engine(IRepository repository)
        {
            _repository = repository;
            _descriptions = repository.GetNodeDescriptions().ToArray();
            _nodes = new Dictionary<Guid, INode>();
            _wires = new List<Wire>();
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
                    node.Setup(nodeInstance);
                }
                catch (Exception)
                {
                    // TODO: Think about possible exceptions and what happens in that case
                    continue;
                }

                _nodes.Add(nodeInstance.Guid, node);

            }

            // Wire up all new nodes with wires
            foreach (var wireInstance in grid.WireInstances)
            {
                if (!_nodes.TryGetValue(wireInstance.OutputNode, out var outputNode) ||
                    !_nodes.TryGetValue(wireInstance.InputNode, out var inputNode))
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
            return new Grid
            {
                NodeInstances = _nodes.Select(n => n.Value.Instance).ToList(),
                WireInstances = _wires.Select(w => w.Instance).ToList()
            };
        }
    }
}
