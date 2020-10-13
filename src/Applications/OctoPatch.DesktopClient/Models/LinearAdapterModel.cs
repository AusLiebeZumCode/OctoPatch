using System.Windows.Markup;
using OctoPatch.Core.Adapters;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class LinearAdapterModel : AdapterConfigurationModel<LinearTransformationAdapter.Config, EmptyEnvironment>
    {
        private bool _inverted;

        public bool Inverted
        {
            get => _inverted;
            set
            {
                _inverted = value;
                OnPropertyChanged();
            }
        }

        protected override void OnSetConfiguration(LinearTransformationAdapter.Config configuration)
        {
            Inverted = configuration.Inverted;

        }

        protected override LinearTransformationAdapter.Config OnGetConfiguration()
        {
            return new LinearTransformationAdapter.Config
            {
                Inverted = Inverted
            };
        }
    }
}
