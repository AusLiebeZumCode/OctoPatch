using System.Collections.ObjectModel;
using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public abstract class NodeModel : Model
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<NodeModel> Items { get; }

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

        protected NodeModel()
        {
            Items = new ObservableCollection<NodeModel>();
        }

        protected NodeModel(NodeDescription description) : this()
        {
            Name = description.DisplayName;

            foreach (var inputDescription in description.InputDescriptions)
            {
                Items.Add(new InputNodeModel(inputDescription));
            }

            foreach (var outputDescription in description.OutputDescriptions)
            {
                Items.Add(new OutputNodeModel(outputDescription));
            }
        }

        protected NodeModel(ConnectorDescription description) : this()
        {
            Name = description.DisplayName;
        }
    }
}
