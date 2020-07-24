using System;
using System.Collections.Generic;
using System.Linq;
using OctoPatch.Exchange;

namespace OctoPatch
{
    public sealed class Engine : IEngine
    {
        private readonly NodeDescription[] _descriptions;

        private readonly Dictionary<Guid, INode> _nodes;

        private readonly List<Wire> _wires;

        public Engine(IRepository repository)
        {
            _descriptions = repository.GetNodeDescriptions().ToArray();
            _nodes = new Dictionary<Guid, INode>();
            _wires = new List<Wire>();
        }

        public void Load(Grid grid)
        {
            foreach (var nodeInstance in grid.NodeInstances)
            {
                var description = _descriptions.FirstOrDefault(d => d.Guid == nodeInstance.Guid);
                if (description == null)
                {
                    // TODO: Think about unknown node
                    continue;
                }

                var type = Type.GetType(description.TypeName);
                if (type == null)
                {
                    continue;
                }

                var node = (INode)Activator.CreateInstance(type);
                node.Setup(nodeInstance);
                _nodes.Add(nodeInstance.Guid, node);
            }

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
                // TODO: Wire up
            }
        }

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
