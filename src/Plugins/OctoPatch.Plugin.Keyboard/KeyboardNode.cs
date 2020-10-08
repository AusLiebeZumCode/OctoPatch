using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.Plugin.Keyboard
{
    public sealed class KeyboardNode : Node<EmptyConfiguration, EmptyEnvironment>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => CommonNodeDescription.Create<KeyboardNode>(
                Guid.Parse(KeyboardPlugin.PluginId),
                "Keyboard",
                "Blabla")
            .AddOutputDescription(KeyOutputDescription);

        /// <summary>
        /// Description of the keyboard output connector
        /// </summary>
        public static ConnectorDescription KeyOutputDescription => new ConnectorDescription(
            "KeyOutput", "key Output", "Key output signal",
            ComplexContentType.Create<int>(Guid.Parse(KeyboardPlugin.PluginId)));

        #endregion

        private readonly IOutputConnectorHandler _outputConnector;

        internal readonly GlobalKeyboardHook _hook;
        internal readonly KeyboardTranslator _keyboard;

        protected override EmptyConfiguration DefaultConfiguration => new EmptyConfiguration();

        public KeyboardNode(Guid id) : base(id)
        {
            _hook = new GlobalKeyboardHook();
            _hook.KeyboardPressed += HookOnKeyboardPressed;

            _keyboard = new KeyboardTranslator(CultureInfo.CurrentCulture);

            _outputConnector = RegisterOutputConnector<int>(KeyOutputDescription);
        }

        private void HookOnKeyboardPressed(object sender, GlobalKeyboardHook.GlobalKeyboardHookEventArgs e)
        {
            if (State == NodeState.Running)
            {
                _outputConnector.Send(e.KeyboardData.VirtualCode);
            }
        }

        protected override Task OnInitialize(EmptyConfiguration configuration, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStart(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStop(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnDeinitialize(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnInitializeReset(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnReset(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override void OnDispose()
        {
            _hook.Dispose();
            _keyboard.Dispose();
        }
    }
}