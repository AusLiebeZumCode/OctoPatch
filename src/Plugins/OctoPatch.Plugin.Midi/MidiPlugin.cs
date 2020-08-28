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
        ///  Plugin id
        /// </summary>
        internal const string PluginId = "{12EA0035-45AF-4DA8-8B5D-E1B9D9484BA4}";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Guid Id => Guid.Parse(PluginId);

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

        private readonly Dictionary<NodeDescription, Type> _nodeDescriptions;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<NodeDescription> GetNodeDescriptions() => _nodeDescriptions.Keys;

        public MidiPlugin()
        {
            _nodeDescriptions = new Dictionary<NodeDescription, Type>();
            _nodeDescriptions.Add(MidiDevice.NodeDescription, typeof(MidiDevice));
            _nodeDescriptions.Add(MidiMessageFilter.NodeDescription, typeof(MidiMessageFilter));
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<TypeDescription> GetTypeDescriptions()
        {
            return new[]
            {
                MidiMessage.TypeDescription
            };
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<AdapterDescription> GetAdapterDescriptions()
        {
            return Enumerable.Empty<AdapterDescription>();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> CreateNode(string key, Guid nodeId, CancellationToken cancellationToken)
        {
            var local = _nodeDescriptions.Keys.FirstOrDefault(d => d.Key == key);
            if (local == null)
                throw new ArgumentException("Node with the given key does not exist", nameof(key));

            return Task.FromResult((INode)Activator.CreateInstance(_nodeDescriptions[local], nodeId));
        }
    }
}
