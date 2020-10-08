using OctoPatch.Plugin.Keyboard;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class KeyboardStringModel : NodeConfigurationModel<KeyboardStringConfiguration, EmptyEnvironment>
    {
        private bool _ignoreNotPrintable;

        /// <summary>
        /// Ignores not printable character like whitespace and newline
        /// </summary>
        public bool IgnoreNotPrintable
        {
            get => _ignoreNotPrintable;
            set
            {
                _ignoreNotPrintable = value;
                OnPropertyChanged();
            }
        }

        protected override void OnSetup(EmptyEnvironment environment)
        {

        }

        protected override KeyboardStringConfiguration OnGetConfiguration()
        {
            return new KeyboardStringConfiguration()
            {
                IgnoreNotPrintable = this.IgnoreNotPrintable
            };
        }

        protected override void OnSetConfiguration(KeyboardStringConfiguration configuration)
        {
            IgnoreNotPrintable = configuration.IgnoreNotPrintable;
        }
    }
}