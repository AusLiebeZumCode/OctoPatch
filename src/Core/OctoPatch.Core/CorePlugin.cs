﻿using System;

namespace OctoPatch.Core
{
    /// <summary>
    /// Plugin for all basic types delivered with octo patch
    /// </summary>
    public sealed class CorePlugin : Server.Plugin
    {
        private const string PluginId = "{598D58EB-756D-4BF7-B04B-AC9603315B6D}";

        public override Guid Id => Guid.Parse(PluginId);

        public override string Name => "Core components";
        
        public override string Description => "Plugin contains all basic types required for octo patch";
        
        public override Version Version => new Version(1, 0, 0);

        protected override INode OnCreateNode(Type type, Guid nodeId, INode parent = null)
        {
            return null;
        }

        protected override IAdapter OnCreateAdapter(Type type)
        {
            return null;
        }
    }
}
