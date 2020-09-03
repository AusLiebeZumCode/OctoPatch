using OctoPatch.Setup;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class NodeModel : Model
    {
        private NodeSetup _setup;

        public NodeSetup Setup
        {
            get => _setup;
            set
            {
                _setup = value;
                OnPropertyChanged();
            }
        }

        private NodeState _state;

        public NodeState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        private string _environment;

        public string Environment
        {
            get => _environment;
            set
            {
                _environment = value;
                OnPropertyChanged();
            }
        }
    }
}
