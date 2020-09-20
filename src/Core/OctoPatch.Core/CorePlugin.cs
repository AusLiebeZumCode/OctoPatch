using System;
using OctoPatch.Core.Nodes;

namespace OctoPatch.Core
{
    /// <summary>
    /// Plugin for all basic types delivered with octo patch
    /// </summary>
    public sealed class CorePlugin : Server.Plugin
    {
        public const string PluginId = "{598D58EB-756D-4BF7-B04B-AC9603315B6D}";

        public override Guid Id => Guid.Parse(PluginId);

        public override string Name => "Core components";
        
        public override string Description => "Plugin contains all basic types required for octo patch";
        
        public override Version Version => new Version(1, 0, 0);

        public CorePlugin()
        {
            RegisterNode<ConsoleNode>(ConsoleNode.Description);
        }

        protected override IAdapter OnCreateAdapter(Type type)
        {
            return null;
        }
    }
}
