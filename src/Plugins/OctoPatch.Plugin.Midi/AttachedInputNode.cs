using OctoPatch.Descriptions;
using System;
using System.Linq;
using OctoPatch.ContentTypes;

namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Base class for all kind of attached input nodes
    /// </summary>
    public abstract class AttachedInputNode : AttachedNode<AttachedNodeConfiguration, EmptyEnvironment, MidiDeviceNode>
    {
        #region Type description

        /// <summary>
        /// Description of the value input connector
        /// </summary>
        public static ConnectorDescription ValueInputDescription => new ConnectorDescription(
            "Value", "Value Input", "value signal", 
            IntegerContentType.Create(minimumValue: 0, maximumValue: 127));

        /// <summary>
        /// Description of the flag input connector
        /// </summary>
        public static ConnectorDescription FlagInputDescription => new ConnectorDescription(
            "Flag", "Flag", "flag signal", 
            new BoolContentType());

        /// <summary>
        /// Description of the enable trigger
        /// </summary>
        public static ConnectorDescription EnabledInputDescription => new ConnectorDescription(
            "Enabled", "Enabled trigger", "Trigger when enabled", 
            new EmptyContentType());

        /// <summary>
        /// Description of the disable trigger
        /// </summary>
        public static ConnectorDescription DisabledInputDescription => new ConnectorDescription(
            "Disabled", "Disabled trigger", "Trigger when disabled", 
            new EmptyContentType());

        #endregion

        /// <summary>
        /// output connector
        /// </summary>
        private readonly IInputConnector _output;

        protected override AttachedNodeConfiguration DefaultConfiguration => new AttachedNodeConfiguration();

        protected AttachedInputNode(Guid nodeId, MidiDeviceNode parentNode) : base(nodeId, parentNode)
        {
            _output = parentNode.Inputs.First(o => o.Key == MidiDeviceNode.MidiInputDescription.Key);

            RegisterInputConnector<int>(ValueInputDescription).Handle<int>(HandleValue);
            RegisterInputConnector<bool>(FlagInputDescription).Handle<bool>(HandleBool);
            RegisterInputConnector(EnabledInputDescription).Handle(HandleEnabledTrigger);
            RegisterInputConnector(DisabledInputDescription).Handle(HandleDisabledTrigger);
        }

        /// <summary>
        /// Handles a disabled trigger
        /// </summary>
        private void HandleDisabledTrigger()
        {
            Handle(0);
        }

        /// <summary>
        /// Handles an enabled trigger
        /// </summary>
        private void HandleEnabledTrigger()
        {
            Handle(127);
        }

        /// <summary>
        /// Handles a bool value
        /// </summary>
        /// <param name="flag"></param>
        private void HandleBool(bool flag)
        {
            Handle(flag ? 0 : 127);
        }

        /// <summary>
        /// Handles a value
        /// </summary>
        /// <param name="value"></param>
        private void HandleValue(int value)
        {
            Handle(value);
        }

        /// <summary>
        /// Sends out the given value as MIDI message
        /// </summary>
        /// <param name="value"></param>
        private void Handle(int value)
        {
            var message = OnHandle(value);
            _output.OnNext(Message.Create(message));
        }

        protected abstract MidiMessage OnHandle(int value);
    }
}
