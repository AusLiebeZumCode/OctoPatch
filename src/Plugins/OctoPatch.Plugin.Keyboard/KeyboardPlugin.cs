using System;

namespace OctoPatch.Plugin.Keyboard
{
    public sealed class KeyboardPlugin : Server.Plugin
    {
        /// <summary>
        ///  Plugin id
        /// </summary>
        internal const string PluginId = "{A6FE76D7-5F0E-4763-A3A5-FCAF43C71464}";

        public override Guid Id => Guid.Parse(PluginId);
        public override string Name => "Keyboard";
        public override string Description => "Adds keyboard support to the octo patch";
        public override Version Version => new Version(1, 0, 0);

        public KeyboardPlugin()
        {
            RegisterNode<KeyboardNode>(KeyboardNode.NodeDescription);
        }

        protected override IAdapter OnCreateAdapter(Type type)
        {
            return null;
        }
    }
}
