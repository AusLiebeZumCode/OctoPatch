using System;
using System.Collections.ObjectModel;
using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public abstract class NodeModel : Model
    {
        public string Key { get; }

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

        protected NodeModel(string key)
        {
            Key = key;
            Items = new ObservableCollection<NodeModel>();
        }

        protected NodeModel(Guid id, NodeDescription description) : this(description.Key)
        {
            Name = description.DisplayName;

            foreach (var inputDescription in description.InputDescriptions)
            {
                Items.Add(new InputNodeModel(id, inputDescription));
            }

            foreach (var outputDescription in description.OutputDescriptions)
            {
                Items.Add(new OutputNodeModel(id, outputDescription));
            }
        }

        protected NodeModel(ConnectorDescription description) : this(description.Key)
        {
            Name = description.DisplayName;
        }
    }
}
