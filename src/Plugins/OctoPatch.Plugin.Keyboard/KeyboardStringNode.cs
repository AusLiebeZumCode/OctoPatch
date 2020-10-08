using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;
using System;

namespace OctoPatch.Plugin.Keyboard
{
    public sealed class KeyboardStringNode : AttachedNode<KeyboardStringConfiguration, EmptyEnvironment, KeyboardNode>
    {
        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription =>
            AttachedNodeDescription.CreateAttached<KeyboardStringNode, KeyboardNode>(
                Guid.Parse(KeyboardPlugin.PluginId),
                "Keyboard String",
                "Blabla")
            .AddOutputDescription(KeyStringPressedOutputDescription)
            .AddOutputDescription(KeyStringReleasedOutputDescription);

        /// <summary>
        /// Description of the keyboard string pressed output connector
        /// </summary>
        public static ConnectorDescription KeyStringPressedOutputDescription => new ConnectorDescription(
            "KeyStringPressedOutput", "key string pressed", "Key char pressed output signal",
            new StringContentType());

        /// <summary>
        /// Description of the keyboard string released output connector
        /// </summary>
        public static ConnectorDescription KeyStringReleasedOutputDescription => new ConnectorDescription(
            "KeyStringReleasedOutput", "key string released", "Key char released output signal",
            new StringContentType());

        protected override KeyboardStringConfiguration DefaultConfiguration => new KeyboardStringConfiguration();

        private readonly KeyboardNode _node;
        private readonly IOutputConnectorHandler _stringPressedOutputConnector;
        private readonly IOutputConnectorHandler _stringReleasedOutputConnector;

        public KeyboardStringNode(Guid nodeId, KeyboardNode parentNode)
            : base(nodeId, parentNode)
        {
            _node = parentNode;
            _node._hook.KeyboardPressed += Hook_KeyboardPressed;

            _stringPressedOutputConnector = RegisterOutputConnector<string>(KeyStringPressedOutputDescription);
            _stringReleasedOutputConnector = RegisterOutputConnector<string>(KeyStringReleasedOutputDescription);
        }

        protected override void OnDispose()
        {
            _node._hook.KeyboardPressed -= Hook_KeyboardPressed;
            base.OnDispose();
        }

        private void Hook_KeyboardPressed(object sender, GlobalKeyboardHook.GlobalKeyboardHookEventArgs e)
        {
            var s = _node._keyboard.GetUnicodeFromVirtualKeyCode((uint)e.KeyboardData.VirtualCode);

            if (Configuration.IgnoreNotPrintable && string.IsNullOrWhiteSpace(s))
                return;

            switch (e.KeyboardState)
            {
                case GlobalKeyboardHook.KeyboardState.KeyDown:
                case GlobalKeyboardHook.KeyboardState.SysKeyDown:
                    _stringPressedOutputConnector.Send(s);
                    break;

                case GlobalKeyboardHook.KeyboardState.KeyUp:
                case GlobalKeyboardHook.KeyboardState.SysKeyUp:
                    _stringReleasedOutputConnector.Send(s);
                    break;
            }
        }
    }
}
