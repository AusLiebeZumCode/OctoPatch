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
                "Gets the pressed keys from the keyboard")
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


        /// <summary>
        /// Description of the keyboard char pressed output connector
        /// </summary>
        public static ConnectorDescription KeyCharPressedOutputDescription => new ConnectorDescription(
            "KeyCharPressedOutput", "key char pressed", "Key char pressed output signal",
            new StringContentType());

        /// <summary>
        /// Description of the keyboard char released output connector
        /// </summary>
        public static ConnectorDescription KeyCharReleasedOutputDescription => new ConnectorDescription(
            "KeyCharReleasedOutput", "key char released", "Key char released output signal",
            new StringContentType());

        protected override KeyboardStringConfiguration DefaultConfiguration => new KeyboardStringConfiguration();

        private readonly KeyboardNode _node;
        private readonly IOutput<string> _stringPressedOutputConnector;
        private readonly IOutput<string> _stringReleasedOutputConnector;
        private readonly IOutput<string> _charPressedOutputConnector;
        private readonly IOutput<string> _charReleasedOutputConnector;

        public KeyboardStringNode(Guid nodeId, KeyboardNode parentNode)
            : base(nodeId, parentNode)
        {
            _node = parentNode;
            _node._hook.KeyboardPressed += Hook_KeyboardPressed;

            _stringPressedOutputConnector = RegisterStringOutput(KeyStringPressedOutputDescription);
            _stringReleasedOutputConnector = RegisterStringOutput(KeyStringReleasedOutputDescription);
            _charPressedOutputConnector = RegisterStringOutput(KeyCharPressedOutputDescription);
            _charReleasedOutputConnector = RegisterStringOutput(KeyCharReleasedOutputDescription);
        }

        protected override void OnDispose()
        {
            _node._hook.KeyboardPressed -= Hook_KeyboardPressed;
            base.OnDispose();
        }

        private void Hook_KeyboardPressed(object sender, GlobalKeyboardHook.GlobalKeyboardHookEventArgs e)
        {
            var s = _node._keyboard.GetUnicodeFromVirtualKeyCode((uint)e.KeyboardData.VirtualCode);
            var c = _node._keyboard.GetCharFromVirtualKeyCode((uint)e.KeyboardData.VirtualCode).ToString();

            if (Configuration.IgnoreNotPrintable && string.IsNullOrWhiteSpace(s))
                return;

            switch (e.KeyboardState)
            {
                case GlobalKeyboardHook.KeyboardState.KeyDown:
                case GlobalKeyboardHook.KeyboardState.SysKeyDown:
                    _stringPressedOutputConnector.Send(s);
                    _charPressedOutputConnector.Send(c);
                    break;

                case GlobalKeyboardHook.KeyboardState.KeyUp:
                case GlobalKeyboardHook.KeyboardState.SysKeyUp:
                    _stringReleasedOutputConnector.Send(s);
                    _charReleasedOutputConnector.Send(c);
                    break;
            }
        }
    }
}
