using System;

namespace OctoPatch.Plugin.Rest
{
    /// <summary>
    /// A plugin for REST communication
    /// </summary>
    public sealed class RestPlugin : Server.Plugin
    {
        public const string PluginId = "{40945D30-186D-4AEE-8895-058FB4759EFF}";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override Guid Id => Guid.Parse(PluginId);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override string Name => "REST";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override string Description => "Adds nodes to do REST actions";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override Version Version => new Version(0, 0, 1);

        public RestPlugin()
        {
            RegisterNode<RestGetNode>(RestGetNode.Description);
        }

        protected override IAdapter OnCreateAdapter(Type type)
        {
            return null;
        }
    }
}