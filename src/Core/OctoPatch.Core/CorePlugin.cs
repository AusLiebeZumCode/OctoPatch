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
        public Guid Id => Guid.Parse("{598D58EB-756D-4BF7-B04B-AC9603315B6D}");
        public string Name => "Core components";
        public string Description => "Plugin contains all basic types required for octo patch";
        public Version Version => new Version(1, 0, 0);
        public IEnumerable<NodeDescription> GetNodeDescriptions()
        {
            return Enumerable.Empty<NodeDescription>();
        }

        public IEnumerable<TypeDescription> GetTypeDescriptions()
        {
            return Enumerable.Empty<TypeDescription>();
        }

        public Task<INode> CreateNode(string key, Guid nodeId, CancellationToken cancellationToken)
        {
            return Task.FromResult<INode>(null);
        }
    }
}
