using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Core;
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
            _nodeDescriptions = new []
            {
                new NodeDescription
                {
                    Guid = Guid.Parse("{8AA1AB11-DB28-4098-9999-13A3A47E8A83}"),
                    Name = "MIDI Device",
                    Description = "This is our first plugin to see how it works",
                    Version = Version.ToString(),
                    TypeName = typeof(MidiDevice).FullName,
                }
            };
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<ComplexTypeDescription> GetTypeDescriptions()
        {
            var midiMessage = new ComplexTypeDescription
            {
                Name = nameof(MidiMessage),
                Description = "a MIDI message",
                PropertyDescriptions = new List<PropertyDescription>()
            };

            midiMessage.PropertyDescriptions.Add(new PropertyDescription
            {
                Name = nameof(MidiMessage.Channel),
                Description = "Describes the channel for this message",
                Type = new IntegerMessageDescription
                {
                    MinimumValue = 1,
                    MaximumValue = 16
                }
            });

            midiMessage.PropertyDescriptions.Add(new PropertyDescription
            {
                Name = nameof(MidiMessage.MessageType),
                Description = "Describes type of messsage MIDI sends",
                Type = new IntegerMessageDescription
                {
                    MinimumValue = 1,
                    MaximumValue = 16
                }
            });

            return new[]
            {
                midiMessage
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
