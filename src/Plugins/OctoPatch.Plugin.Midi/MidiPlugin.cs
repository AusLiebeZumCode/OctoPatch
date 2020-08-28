using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;
using OctoPatch.Server;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Implementation for the MIDI Plugin
    /// </summary>
    public sealed class MidiPlugin : IPlugin
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string Name => "MIDI";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string Description => "Adds MIDI functionality";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Version Version => new Version(1, 0, 0);

        private readonly NodeDescription[] _nodeDescriptions;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<NodeDescription> GetNodeDescriptions() => _nodeDescriptions;

        public MidiPlugin()
        {
            _nodeDescriptions = new[]
            {
                MidiDevice.NodeDescription
            };
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<ComplexTypeDescription> GetTypeDescriptions()
        {
            return new[]
            {
                MidiMessage.TypeDescription
            };
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> CreateNode(Guid nodeDescriptionGuid, Guid nodeId, CancellationToken cancellationToken)
        {
            var local = _nodeDescriptions.FirstOrDefault(d => d.Guid == nodeDescriptionGuid);
            if (local == null)
                throw new ArgumentException("Node with the given GUID does not exist", nameof(nodeDescriptionGuid));

            var type = Type.GetType(local.TypeName);
            return Task.FromResult((INode)Activator.CreateInstance(type, nodeId));
        }
    }
}
