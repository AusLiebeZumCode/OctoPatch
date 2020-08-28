using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;
using OctoPatch.Server;

namespace OctoPatch.Core
{
    /// <summary>
    /// Plugin for all basic types delivered with octo patch
    /// </summary>
    public sealed class CorePlugin : IPlugin
    {
        public string Name => "Core components";
        public string Description => "Plugin contains all basic types required for octo patch";
        public Version Version => new Version(1, 0, 0);
        public IEnumerable<NodeDescription> GetNodeDescriptions()
        {
            return Enumerable.Empty<NodeDescription>();
        }

        public IEnumerable<ComplexTypeDescription> GetTypeDescriptions()
        {
            return Enumerable.Empty<ComplexTypeDescription>();
        }

        public Task<INode> CreateNode(Guid nodeDescriptionGuid, Guid nodeId, CancellationToken cancellationToken)
        {
            return Task.FromResult<INode>(null);
        }
    }
}
